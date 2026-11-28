using System.Diagnostics;
using System.Text.Json;
using ExoplanetHunter.Domain.Catalog;
using ExoplanetHunter.Transit;
using ExoplanetHunter.Transit.Mast;
using ExoplanetHunter.Transit.Vetting;
using CSharpNumerics.Engines.Exoplanet.Features;
using CSharpNumerics.Engines.Exoplanet.Vetting;

// Precomputes Transit Lab results for the stars in a target list.
//
//   transit [--targets data/transit/targets.json] [--catalog data/catalog.json.gz]
//           [--output data/transit] [--cache .cache/mast] [--only <star name>] [--max-sectors 3]
//           [--force true] [--summary <markdown file>] [--vetter data/transit/model/vetter.bin]
//
// Each star gets <output>/<starId>.json; <output>/index.json lists them all. A star that fails
// (e.g. MAST is down) keeps its previous result. A star whose previous result was made from the
// same sectors, planets and analysis version is skipped (ResultReuse) unless --force is given.
// --summary writes a Markdown report of the run, used as the description of the weekly PR.
// --vetter is the vetting model that scores each candidate (docs/transit-vetting-model.md); without
// the file, candidates are not scored. A model that no longer loads stops the run.
//
// The vetting dataset commands (toi, dataset, dataset-merge) are in DatasetCommands.cs.

if (args.Length > 0 && !args[0].StartsWith("--", StringComparison.Ordinal))
    return await DatasetCommands.RunAsync(args[0], ParseArgs(args[1..]));

var options = ParseArgs(args);
string targetsPath = options.GetValueOrDefault("targets") ?? "data/transit/targets.json";
string catalogPath = options.GetValueOrDefault("catalog") ?? "data/catalog.json.gz";
string outputDir = options.GetValueOrDefault("output") ?? "data/transit";
string cacheDir = options.GetValueOrDefault("cache") ?? ".cache/mast";
int maxSectors = int.Parse(options.GetValueOrDefault("max-sectors") ?? "3");
string? only = options.GetValueOrDefault("only");
bool force = bool.Parse(options.GetValueOrDefault("force") ?? "false");
string? summaryPath = options.GetValueOrDefault("summary");
string vetterPath = options.GetValueOrDefault("vetter") ?? "data/transit/model/vetter.bin";

var json = new JsonSerializerOptions(JsonSerializerDefaults.Web) { WriteIndented = false };
var targets = JsonSerializer.Deserialize<TargetList>(await File.ReadAllTextAsync(targetsPath), json)?.Stars
    ?? throw new InvalidDataException($"{targetsPath} has no stars.");
if (only is not null)
    targets = targets.Where(t => t == only).ToArray();

var catalog = new Catalog(await CatalogSnapshot.LoadAsync(catalogPath));
Directory.CreateDirectory(outputDir);

using var http = new HttpClient { Timeout = TimeSpan.FromMinutes(3) };
http.DefaultRequestHeaders.UserAgent.ParseAdd("ExoplanetHunter-TransitLab/1.0 (+https://exoplanethunter.com)");
var mast = new MastClient(http, cacheDir);
var tics = await new TicLookup(http).FindAsync(targets);
CandidateVetter? vetter = null;
if (File.Exists(vetterPath))
{
    // Loading trains the forest again and checks it against the saved fingerprint.
    vetter = CandidateVetter.Deserialize(await File.ReadAllBytesAsync(vetterPath));
    Console.WriteLine($"Vetting model {vetter.Name} ({vetter.TrainingExamples} training examples) from {vetterPath}.");
    if (vetter.FeatureSchemaVersion != VettingFeatures.SchemaVersion)
    {
        // An old model after a feature change: run without scores rather than fail every star.
        Console.WriteLine($"WARNING: {vetter.Name} uses feature schema {vetter.FeatureSchemaVersion}, this build computes " +
                          $"schema {VettingFeatures.SchemaVersion}. Candidates are not scored; train a new model.");
        vetter = null;
    }
}
else
{
    Console.WriteLine($"No vetting model at {vetterPath}: candidates are not scored.");
}
var analyzer = new TransitAnalyzer(vetter: vetter);

int failures = 0, skipped = 0;
var report = new List<string>();
foreach (string name in targets)
{
    var clock = Stopwatch.StartNew();
    try
    {
        var star = catalog.FindStar(name) ?? throw new InvalidDataException("not in the catalogue");
        if (!tics.TryGetValue(name, out long tic))
            throw new InvalidDataException("no TIC id in the NASA Exoplanet Archive");

        var files = SectorSelection.Choose(await mast.FindSpocLightCurvesAsync(tic), maxSectors);
        if (files.Count == 0)
            throw new InvalidDataException("no 2-minute SPOC light curves in MAST");

        string resultPath = Path.Combine(outputDir, $"{star.Id}.json");
        var previous = File.Exists(resultPath)
            ? JsonSerializer.Deserialize<TransitResult>(await File.ReadAllTextAsync(resultPath), json)
            : null;
        if (!force && ResultReuse.CanReuse(previous, tic, files.Select(f => f.Sector).ToArray(), catalog.PlanetsOf(star).Select(p => p.Name), vetter?.Name))
        {
            skipped++;
            Console.WriteLine($"SAME  {name}: sectors {string.Join(",", previous!.Sectors)} already analysed");
            continue;
        }

        var sectors = new List<SectorLightCurve>();
        foreach (var file in files)
            sectors.Add(SectorLightCurve.FromFits(await mast.DownloadAsync(file), file.Sector));

        var result = analyzer.Analyze(star, catalog.PlanetsOf(star), tic, sectors, DateTimeOffset.UtcNow);
        await File.WriteAllTextAsync(resultPath, JsonSerializer.Serialize(result, json));

        string found = string.Join(", ", result.KnownPlanets.Select(k => $"{k.Name}: {k.Status}"));
        Console.WriteLine($"OK    {name} (TIC {tic}) sectors {string.Join(",", result.Sectors)}: " +
                          $"{result.Candidates.Count} candidate(s) in {clock.Elapsed.TotalSeconds:F0}s; {found}" +
                          (vetter is null ? "" : $"; planet scores {Scores(result)}"));

        int knownFound = result.KnownPlanets.Count(k => k.Status is MatchStatus.Found or MatchStatus.FoundAtHarmonic);
        int unmatched = result.Candidates.Count(c => c.MatchedPlanet is null && c.SecondaryEclipseOf is null);
        report.Add($"| {name} | {string.Join(", ", result.Sectors)} | {knownFound} of {result.KnownPlanets.Count} | " +
                   $"{result.Candidates.Count} | {unmatched} | {Scores(result)} | {(previous is null ? "new" : "updated")} |");
    }
    catch (Exception ex) when (ex is not OperationCanceledException)
    {
        failures++;
        Console.WriteLine($"FAIL  {name}: {ex.Message}");
        report.Add($"| {name} | | | | | | failed: {ex.Message.Replace('|', '/')} |");
    }
}

// Rebuild the index from every result on disk, so stars that failed this run keep theirs.
var allResults = Directory.EnumerateFiles(outputDir, "*.json")
    .Where(f => int.TryParse(Path.GetFileNameWithoutExtension(f), out _)) // <starId>.json, not targets.json or index.json
    .Select(f => JsonSerializer.Deserialize<TransitResult>(File.ReadAllText(f), json)!)
    .ToList();
var index = allResults
    .OrderBy(r => r.StarName, StringComparer.OrdinalIgnoreCase)
    .Select(r => new TransitIndexEntry(r.StarId, r.StarName, r.TicId, r.Sectors, r.Candidates.Count,
        r.KnownPlanets.Count(k => k.Status is MatchStatus.Found or MatchStatus.FoundAtHarmonic), r.KnownPlanets.Count, r.GeneratedAt))
    .ToArray();
await File.WriteAllTextAsync(Path.Combine(outputDir, "index.json"), JsonSerializer.Serialize(index, json));

int analysed = targets.Length - failures - skipped;
Console.WriteLine($"\n{analysed} analysed, {skipped} unchanged, {failures} failed of {targets.Length} stars; index lists {index.Length}.");

if (summaryPath is not null)
{
    var summary = new List<string>
    {
        $"Transit Lab run: {analysed} star(s) analysed, {skipped} unchanged (no new TESS sectors), {failures} failed.",
        "",
    };
    if (report.Count > 0)
        summary.AddRange(["| Star | Sectors | Known planets found | Candidates | Unmatched signals | Planet scores | Result |", "|---|---|---|---|---|---|---|", .. report]);
    if (vetter is not null)
        summary.AddRange(["", LabEvaluation.Evaluate(allResults).ToMarkdown()]);
    await File.WriteAllLinesAsync(summaryPath, summary);
}

return failures == targets.Length && targets.Length > 0 ? 1 : 0;

static string Scores(TransitResult result) =>
    string.Join(", ", result.Candidates.Select(c =>
        c.VettingScore is { } p ? p.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)
        : c.TransitsObserved is { } n ? $"– ({n} transits)"
        : "–"));

static Dictionary<string, string> ParseArgs(string[] args)
{
    var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    for (int i = 0; i < args.Length; i++)
    {
        if (!args[i].StartsWith("--") || i + 1 >= args.Length)
            throw new ArgumentException($"Expected '--name value', got '{args[i]}'.");
        result[args[i][2..]] = args[++i];
    }

    return result;
}

sealed record TargetList(string[] Stars);
