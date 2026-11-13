using System.Diagnostics;
using System.Text.Json;
using ExoplanetHunter.Domain.Catalog;
using ExoplanetHunter.Transit;
using ExoplanetHunter.Transit.Mast;

// Precomputes Transit Lab results for the stars in a target list.
//
//   transit [--targets data/transit/targets.json] [--catalog data/catalog.json.gz]
//           [--output data/transit] [--cache .cache/mast] [--only <star name>] [--max-sectors 3]
//           [--force true] [--summary <markdown file>]
//
// Each star gets <output>/<starId>.json; <output>/index.json lists them all. A star that fails
// (e.g. MAST is down) keeps its previous result. A star whose previous result was made from the
// same sectors, planets and analysis version is skipped (ResultReuse) unless --force is given.
// --summary writes a Markdown report of the run, used as the description of the weekly PR.

var options = ParseArgs(args);
string targetsPath = options.GetValueOrDefault("targets") ?? "data/transit/targets.json";
string catalogPath = options.GetValueOrDefault("catalog") ?? "data/catalog.json.gz";
string outputDir = options.GetValueOrDefault("output") ?? "data/transit";
string cacheDir = options.GetValueOrDefault("cache") ?? ".cache/mast";
int maxSectors = int.Parse(options.GetValueOrDefault("max-sectors") ?? "3");
string? only = options.GetValueOrDefault("only");
bool force = bool.Parse(options.GetValueOrDefault("force") ?? "false");
string? summaryPath = options.GetValueOrDefault("summary");

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
var analyzer = new TransitAnalyzer();

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
        if (!force && ResultReuse.CanReuse(previous, tic, files.Select(f => f.Sector).ToArray(), catalog.PlanetsOf(star).Select(p => p.Name)))
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
                          $"{result.Candidates.Count} candidate(s) in {clock.Elapsed.TotalSeconds:F0}s; {found}");

        int knownFound = result.KnownPlanets.Count(k => k.Status is MatchStatus.Found or MatchStatus.FoundAtHarmonic);
        int unmatched = result.Candidates.Count(c => c.MatchedPlanet is null && c.SecondaryEclipseOf is null);
        report.Add($"| {name} | {string.Join(", ", result.Sectors)} | {knownFound} of {result.KnownPlanets.Count} | " +
                   $"{result.Candidates.Count} | {unmatched} | {(previous is null ? "new" : "updated")} |");
    }
    catch (Exception ex) when (ex is not OperationCanceledException)
    {
        failures++;
        Console.WriteLine($"FAIL  {name}: {ex.Message}");
        report.Add($"| {name} | | | | | failed: {ex.Message.Replace('|', '/')} |");
    }
}

// Rebuild the index from every result on disk, so stars that failed this run keep theirs.
var index = Directory.EnumerateFiles(outputDir, "*.json")
    .Where(f => int.TryParse(Path.GetFileNameWithoutExtension(f), out _)) // <starId>.json, not targets.json or index.json
    .Select(f => JsonSerializer.Deserialize<TransitResult>(File.ReadAllText(f), json)!)
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
        summary.AddRange(["| Star | Sectors | Known planets found | Candidates | Unmatched signals | Result |", "|---|---|---|---|---|---|", .. report]);
    await File.WriteAllLinesAsync(summaryPath, summary);
}

return failures == targets.Length && targets.Length > 0 ? 1 : 0;

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
