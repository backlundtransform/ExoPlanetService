using System.Text.Json;

namespace ExoplanetHunter.Api.Transit;

/// <summary>
/// Where the Transit Lab result files are served from: the copy shipped with the deployment
/// (Data/transit) or, once the refresh has run, the copy kept up to date from GitHub.
/// </summary>
public sealed class TransitStore(string directory)
{
    private volatile string _directory = directory;

    public string Directory => _directory;

    public string PathOf(string fileName) => Path.Combine(_directory, fileName);

    public void Use(string directory) => _directory = directory;

    /// <summary>
    /// Picks the directory to serve at startup: the refreshed copy, unless the deployment
    /// ships newer results (e.g. right after a deploy, before the next refresh).
    /// </summary>
    public static TransitStore Create(string bundled, TransitRefreshOptions settings, ILogger logger)
    {
        if (!settings.Enabled || string.IsNullOrEmpty(settings.StoragePath))
            return new TransitStore(bundled);

        try
        {
            var stored = TransitIndex.Read(Path.Combine(settings.StoragePath, TransitIndex.FileName));
            var shipped = TransitIndex.Read(Path.Combine(bundled, TransitIndex.FileName));
            return new TransitStore(stored is not null && TransitIndex.Newest(stored) >= TransitIndex.Newest(shipped)
                ? settings.StoragePath
                : bundled);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not read refreshed Transit Lab results in {Path}; serving the bundled ones", settings.StoragePath);
            return new TransitStore(bundled);
        }
    }
}

/// <summary>The parts of data/transit/index.json the refresh needs.</summary>
public sealed record TransitIndexEntry(int StarId, DateTimeOffset GeneratedAt);

public static class TransitIndex
{
    public const string FileName = "index.json";

    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public static IReadOnlyList<TransitIndexEntry> Parse(string json) =>
        JsonSerializer.Deserialize<TransitIndexEntry[]>(json, Json) ?? throw new InvalidDataException("index.json is null");

    public static IReadOnlyList<TransitIndexEntry>? Read(string path) =>
        File.Exists(path) ? Parse(File.ReadAllText(path)) : null;

    public static DateTimeOffset Newest(IReadOnlyList<TransitIndexEntry>? index) =>
        index is { Count: > 0 } ? index.Max(e => e.GeneratedAt) : DateTimeOffset.MinValue;
}
