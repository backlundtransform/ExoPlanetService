using System.Diagnostics;
using System.Globalization;
using ExoplanetHunter.Transit;
using ExoplanetHunter.Transit.Mast;
using ExoplanetHunter.Transit.Vetting;

/// <summary>
/// Commands that build the vetting dataset (docs/transit-vetting-dataset.md):
///
///   transit toi --output toi.json
///   transit dataset --toi toi.json --output shard.bin --skipped shard.tsv [--shard 0/16] [--limit 0]
///                   [--synthetic all|none|planets,binaries,inverted,scrambled] [--include-undecided false]
///                   [--parallel 1] [--max-minutes 0] [--max-sectors 3] [--cache .cache/dataset] [--keep-cache false]
///   transit dataset-merge --input shards/ --output dataset.bin [--report report.md]
/// </summary>
static class DatasetCommands
{
    public static async Task<int> RunAsync(string command, Dictionary<string, string> options)
    {
        return command switch
        {
            "toi" => await FetchToisAsync(options),
            "dataset" => await BuildAsync(options),
            "dataset-merge" => Merge(options),
            _ => throw new ArgumentException($"Unknown command '{command}'. Commands: toi, dataset, dataset-merge."),
        };
    }

    private static HttpClient Http()
    {
        var http = new HttpClient { Timeout = TimeSpan.FromMinutes(3) };
        http.DefaultRequestHeaders.UserAgent.ParseAdd("ExoplanetHunter-TransitLab/1.0 (+https://exoplanethunter.com)");
        return http;
    }

    private static async Task<int> FetchToisAsync(Dictionary<string, string> options)
    {
        string output = options.GetValueOrDefault("output") ?? "toi.json";
        if (File.Exists(output)) File.Delete(output);

        using var http = Http();
        var tois = await new ToiClient(http).LoadOrFetchAsync(output);
        Console.WriteLine($"{tois.Count} TOIs on {tois.Select(t => t.TicId).Distinct().Count()} stars written to {output}.");
        foreach (var group in tois.GroupBy(t => t.Disposition).OrderByDescending(g => g.Count()))
            Console.WriteLine($"  {(group.Key == "" ? "(none)" : group.Key),-4} {group.Count()}");
        return 0;
    }

    private static async Task<int> BuildAsync(Dictionary<string, string> options)
    {
        string toiPath = options.GetValueOrDefault("toi") ?? "toi.json";
        string output = options.GetValueOrDefault("output") ?? "dataset.bin";
        string skippedPath = options.GetValueOrDefault("skipped") ?? Path.ChangeExtension(output, ".tsv");
        string cacheDir = options.GetValueOrDefault("cache") ?? ".cache/dataset";
        bool keepCache = bool.Parse(options.GetValueOrDefault("keep-cache") ?? "false");
        bool includeUndecided = bool.Parse(options.GetValueOrDefault("include-undecided") ?? "false");
        int limit = int.Parse(options.GetValueOrDefault("limit") ?? "0", CultureInfo.InvariantCulture);
        int parallel = int.Parse(options.GetValueOrDefault("parallel") ?? "1", CultureInfo.InvariantCulture);
        int maxSectors = int.Parse(options.GetValueOrDefault("max-sectors") ?? "3", CultureInfo.InvariantCulture);
        double maxMinutes = double.Parse(options.GetValueOrDefault("max-minutes") ?? "0", CultureInfo.InvariantCulture);
        var (shard, shards) = ParseShard(options.GetValueOrDefault("shard") ?? "0/1");
        var synthetic = ParseSynthetic(options.GetValueOrDefault("synthetic") ?? "all");

        using var http = Http();
        var tois = await new ToiClient(http).LoadOrFetchAsync(toiPath);
        var toisByTic = tois.GroupBy(t => t.TicId).ToDictionary(g => g.Key, g => (IReadOnlyList<ToiEntry>)g.ToArray());

        var tics = toisByTic
            .Where(kv => kv.Value.Any(t => includeUndecided || t.Label is not null))
            .Select(kv => kv.Key)
            .Order()
            .Where((_, i) => i % shards == shard)
            .ToArray();
        if (limit > 0) tics = tics.Take(limit).ToArray();
        Console.WriteLine($"Shard {shard}/{shards}: {tics.Length} stars.");

        var mast = new MastClient(http, cacheDir);
        var builder = new VettingSampleBuilder(new VettingSampleOptions { Synthetic = synthetic });
        var samples = new List<VettingSample>();
        var skipped = new List<SkippedSample>();
        var gate = new object();
        var clock = Stopwatch.StartNew();
        int done = 0;

        await Parallel.ForEachAsync(tics, new ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, parallel) }, async (tic, ct) =>
        {
            var starTois = toisByTic[tic];
            var wanted = starTois.Where(t => includeUndecided || t.Label is not null).ToArray();
            List<VettingSample> starSamples = [];
            List<SkippedSample> starSkipped = [];

            if (maxMinutes > 0 && clock.Elapsed.TotalMinutes > maxMinutes)
            {
                starSkipped.AddRange(wanted.Select(t => new SkippedSample($"TOI-{t.Toi}", tic, "not processed: time limit")));
            }
            else
            {
                var star = Stopwatch.StartNew();
                try
                {
                    var files = await mast.FindSpocLightCurvesAsync(tic, cancellationToken: ct);
                    if (files.Count == 0)
                        throw new InvalidDataException("no 2-minute SPOC light curves in MAST");

                    var epochs = wanted.Where(t => t.HasEphemeris).Select(t => SectorSelection.ApproximateSector(t.EpochBtjd)).Order().ToArray();
                    var chosen = epochs.Length > 0
                        ? SectorSelection.ChooseNearest(files, epochs[epochs.Length / 2], maxSectors)
                        : SectorSelection.Choose(files, maxSectors);

                    var sectors = new List<SectorLightCurve>();
                    foreach (var file in chosen)
                    {
                        sectors.Add(SectorLightCurve.FromFits(await mast.DownloadAsync(file, ct), file.Sector));
                        if (!keepCache) File.Delete(Path.Combine(cacheDir, file.FileName));
                    }

                    (starSamples, starSkipped) = builder.Build(tic, sectors, starTois, labelledOnly: !includeUndecided);
                    Console.WriteLine($"OK    TIC {tic} sectors {string.Join(",", chosen.Select(f => f.Sector))}: " +
                                      $"{starSamples.Count} example(s), {starSkipped.Count} skipped in {star.Elapsed.TotalSeconds:F0}s");
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    starSkipped = wanted.Select(t => new SkippedSample($"TOI-{t.Toi}", tic, ex.Message)).ToList();
                    Console.WriteLine($"FAIL  TIC {tic}: {ex.Message}");
                }
            }

            lock (gate)
            {
                samples.AddRange(starSamples);
                skipped.AddRange(starSkipped);
                // Checkpoint, so a job that is cancelled at its time limit still leaves its work.
                if (++done % 25 == 0)
                {
                    Save(output, skippedPath, builder.Header, samples, skipped);
                    Console.WriteLine($"----  {done} of {tics.Length} stars, {samples.Count} examples, {clock.Elapsed.TotalMinutes:F0} min");
                }
            }
        });

        Save(output, skippedPath, builder.Header, samples, skipped);
        Console.WriteLine($"\n{samples.Count} examples and {skipped.Count} skipped from {tics.Length} stars in {clock.Elapsed.TotalMinutes:F0} min.");
        return samples.Count == 0 && tics.Length > 0 ? 1 : 0;
    }

    private static int Merge(Dictionary<string, string> options)
    {
        string input = options.GetValueOrDefault("input") ?? "shards";
        string output = options.GetValueOrDefault("output") ?? "dataset.bin";
        string? reportPath = options.GetValueOrDefault("report");

        VettingDatasetHeader? header = null;
        var samples = new List<VettingSample>();
        var ids = new HashSet<string>(StringComparer.Ordinal);
        foreach (string file in Directory.EnumerateFiles(input, "*.bin", SearchOption.AllDirectories).Order())
        {
            var (h, part) = VettingDataset.ReadFile(file);
            if (header is null) header = h;
            else if (!header.SameLayout(h)) throw new InvalidDataException($"{file} has a different layout from the other files.");
            samples.AddRange(part.Where(s => ids.Add(s.Id)));
        }
        if (header is null) throw new InvalidDataException($"No dataset files in {input}.");

        var skipped = Directory.EnumerateFiles(input, "*.tsv", SearchOption.AllDirectories)
            .SelectMany(ReadSkipped)
            .ToList();

        VettingDataset.WriteFile(output, header, samples);
        string report = DatasetReport.Build(samples, skipped, header);
        Console.WriteLine(report);
        if (reportPath is not null) File.WriteAllText(reportPath, report);
        return 0;
    }

    private static void Save(string output, string skippedPath, VettingDatasetHeader header,
        List<VettingSample> samples, List<SkippedSample> skipped)
    {
        VettingDataset.WriteFile(output, header, samples);
        File.WriteAllLines(skippedPath, skipped.Select(s =>
            $"{s.Id}\t{s.TicId.ToString(CultureInfo.InvariantCulture)}\t{s.Reason.Replace('\t', ' ').Replace('\n', ' ').Replace('\r', ' ')}"));
    }

    private static IEnumerable<SkippedSample> ReadSkipped(string path) =>
        File.ReadLines(path)
            .Select(line => line.Split('\t'))
            .Where(parts => parts.Length == 3)
            .Select(parts => new SkippedSample(parts[0], long.Parse(parts[1], CultureInfo.InvariantCulture), parts[2]));

    private static (int Shard, int Shards) ParseShard(string value)
    {
        var parts = value.Split('/');
        if (parts.Length != 2 || !int.TryParse(parts[0], out int shard) || !int.TryParse(parts[1], out int shards)
            || shards < 1 || shard < 0 || shard >= shards)
            throw new ArgumentException($"--shard must look like 3/16, got '{value}'.");
        return (shard, shards);
    }

    private static SyntheticKinds ParseSynthetic(string value) => value.ToLowerInvariant() switch
    {
        "all" => SyntheticKinds.All,
        "none" => SyntheticKinds.None,
        _ => value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(v => Enum.Parse<SyntheticKinds>(v, ignoreCase: true))
            .Aggregate(SyntheticKinds.None, (all, kind) => all | kind),
    };
}
