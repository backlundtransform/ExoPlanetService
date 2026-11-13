using System.Text.Json;
using Microsoft.Extensions.Options;

namespace ExoplanetHunter.Api.Transit;

public sealed class TransitRefreshOptions
{
    public const string Section = "Transit:Refresh";

    public bool Enabled { get; set; }

    /// <summary>How often to look for new results. A check is one small request when nothing changed.</summary>
    public TimeSpan Interval { get; set; } = TimeSpan.FromDays(1);

    /// <summary>The data/transit directory of the default branch, as raw files.</summary>
    public string SourceUrl { get; set; } = "https://raw.githubusercontent.com/backlundtransform/ExoPlanetService/master/data/transit/";

    /// <summary>
    /// Directory the results are kept in, so they survive restarts and deploys. On Azure App
    /// Service use a path under %HOME%, e.g. <c>D:\home\data\transit</c>.
    /// </summary>
    public string? StoragePath { get; set; }
}

/// <summary>
/// Keeps the Transit Lab results in step with data/transit on GitHub, so a merged results PR
/// (from the weekly Transit Lab workflow) reaches the site without a deploy. Only files whose
/// generatedAt changed are downloaded; the index is replaced last.
/// </summary>
public sealed class TransitRefreshService(
    TransitStore store,
    HttpClient http,
    IOptions<TransitRefreshOptions> options,
    TimeProvider time,
    ILogger<TransitRefreshService> logger) : BackgroundService
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var settings = options.Value;
        if (!settings.Enabled || string.IsNullOrEmpty(settings.StoragePath))
            return;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RefreshAsync(settings, stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Transit Lab refresh failed; trying again in {Interval}", settings.Interval);
            }

            await Task.Delay(settings.Interval, time, stoppingToken);
        }
    }

    /// <summary>Brings <see cref="TransitRefreshOptions.StoragePath"/> up to date and serves it. Returns the number of files downloaded.</summary>
    public async Task<int> RefreshAsync(TransitRefreshOptions settings, CancellationToken cancellationToken)
    {
        var target = settings.StoragePath!;
        var source = settings.SourceUrl.EndsWith('/') ? settings.SourceUrl : settings.SourceUrl + "/";

        string indexJson = await http.GetStringAsync(source + TransitIndex.FileName, cancellationToken);
        var remote = TransitIndex.Parse(indexJson);
        if (remote.Count == 0)
            throw new InvalidDataException("The published index is empty; keeping the current results.");

        var servedIndex = TransitIndex.Read(store.PathOf(TransitIndex.FileName));
        if (store.Directory != target && TransitIndex.Newest(remote) < TransitIndex.Newest(servedIndex))
        {
            // The deployment ships newer results than GitHub (e.g. deployed from a branch); keep them.
            logger.LogInformation("Bundled Transit Lab results are newer than the published ones; keeping them");
            return 0;
        }

        System.IO.Directory.CreateDirectory(target);
        var local = (TransitIndex.Read(Path.Combine(target, TransitIndex.FileName)) ?? [])
            .ToDictionary(e => e.StarId, e => e.GeneratedAt);
        var served = (servedIndex ?? [])
            .ToDictionary(e => e.StarId, e => e.GeneratedAt);

        int downloaded = 0;
        foreach (var entry in remote)
        {
            string fileName = $"{entry.StarId}.json";
            string destination = Path.Combine(target, fileName);
            if (local.GetValueOrDefault(entry.StarId) == entry.GeneratedAt && File.Exists(destination))
                continue;

            string temp = Path.Combine(target, "." + fileName);
            if (store.Directory != target && served.GetValueOrDefault(entry.StarId) == entry.GeneratedAt && File.Exists(store.PathOf(fileName)))
            {
                // The served (bundled) copy is already this version.
                File.Copy(store.PathOf(fileName), temp, overwrite: true);
            }
            else
            {
                string json = await http.GetStringAsync(source + fileName, cancellationToken);
                Validate(json, entry.StarId);
                await File.WriteAllTextAsync(temp, json, cancellationToken);
                downloaded++;
            }

            File.Move(temp, destination, overwrite: true);
        }

        // Stars dropped from the published index are dropped here too.
        var keep = remote.Select(e => $"{e.StarId}.json").ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var file in System.IO.Directory.EnumerateFiles(target, "*.json"))
        {
            string name = Path.GetFileName(file);
            if (name != TransitIndex.FileName && !keep.Contains(name))
                File.Delete(file);
        }

        string indexTemp = Path.Combine(target, "." + TransitIndex.FileName);
        await File.WriteAllTextAsync(indexTemp, indexJson, cancellationToken);
        File.Move(indexTemp, Path.Combine(target, TransitIndex.FileName), overwrite: true);

        if (store.Directory != target)
            store.Use(target);
        if (downloaded > 0)
            logger.LogInformation("Transit Lab results refreshed: {Downloaded} of {Stars} stars downloaded", downloaded, remote.Count);
        return downloaded;
    }

    /// <summary>Rejects a truncated or wrong file before it replaces a good one.</summary>
    private static void Validate(string json, int starId)
    {
        using var document = JsonDocument.Parse(json);
        if (!document.RootElement.TryGetProperty("starId", out var id) || id.GetInt32() != starId)
            throw new InvalidDataException($"{starId}.json does not hold star {starId}.");
    }
}
