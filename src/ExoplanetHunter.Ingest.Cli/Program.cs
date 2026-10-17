using ExoplanetHunter.Domain.Catalog;
using ExoplanetHunter.Ingest;
using ExoplanetHunter.Ingest.Nasa;

// Builds a catalogue snapshot from the NASA Exoplanet Archive.
//
//   ingest [--previous <snapshot>] [--output <snapshot>] [--input <pscomppars.csv>] [--report <file>]
//
// --previous  snapshot whose ids are kept (default: data/catalog.json.gz, else data/catalog.legacy.json.gz)
// --output    where to write the new snapshot (default: data/catalog.json.gz)
// --input     read pscomppars from a CSV file instead of downloading it
// --report    also write the comparison report to this file

var options = ParseArgs(args);
string previousPath = options.GetValueOrDefault("previous")
    ?? (File.Exists("data/catalog.json.gz") ? "data/catalog.json.gz" : "data/catalog.legacy.json.gz");
string outputPath = options.GetValueOrDefault("output") ?? "data/catalog.json.gz";

var previous = await CatalogSnapshot.LoadAsync(previousPath);
Console.WriteLine($"Previous snapshot: {previousPath} ({previous.Stars.Count} stars, {previous.Planets.Count} planets)");

IReadOnlyList<NasaPlanetRow> rows;
if (options.GetValueOrDefault("input") is { } input)
{
    using var reader = File.OpenText(input);
    rows = NasaArchiveClient.Parse(reader);
    Console.WriteLine($"Read {rows.Count} planets from {input}");
}
else
{
    using var http = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };
    http.DefaultRequestHeaders.UserAgent.ParseAdd("ExoplanetHunter-Ingest/1.0 (+https://exoplanethunter.com)");
    rows = await new NasaArchiveClient(http).GetConfirmedPlanetsAsync();
    Console.WriteLine($"Downloaded {rows.Count} planets from {NasaArchiveClient.TapEndpoint}");
}

var next = CatalogBuilder.Build(rows, previous, DateTimeOffset.UtcNow);
await next.SaveAsync(outputPath);
Console.WriteLine($"Wrote {outputPath}: {next.Stars.Count} stars, {next.Planets.Count} planets");
Console.WriteLine();

var report = CatalogComparison.Report(previous, next);
Console.WriteLine(report);
if (options.GetValueOrDefault("report") is { } reportPath)
    await File.WriteAllTextAsync(reportPath, report);

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
