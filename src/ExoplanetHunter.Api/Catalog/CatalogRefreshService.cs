using ExoplanetHunter.Domain.Catalog;
using ExoplanetHunter.Ingest;
using ExoplanetHunter.Ingest.Nasa;
using Microsoft.Extensions.Options;

namespace ExoplanetHunter.Api.Catalog;

public sealed class CatalogRefreshOptions
{
    public const string Section = "Catalog:Refresh";

    public bool Enabled { get; set; }

    /// <summary>How old the served catalogue may get before it is rebuilt.</summary>
    public TimeSpan Interval { get; set; } = TimeSpan.FromDays(7);

    /// <summary>
    /// Where rebuilt catalogues are saved so they survive restarts. On Azure App Service use
    /// a path under %HOME%, e.g. <c>D:\home\data\catalog.json.gz</c>.
    /// </summary>
    public string? StoragePath { get; set; }

    /// <summary>A rebuilt catalogue with fewer planets than this share of the current one is rejected.</summary>
    public double MinimumPlanetRatio { get; set; } = 0.95;
}

/// <summary>
/// Rebuilds the catalogue from the NASA Exoplanet Archive on a schedule, keeping ids from the
/// catalogue currently served, and swaps it in without a restart.
/// </summary>
public sealed class CatalogRefreshService(
    CatalogStore store,
    NasaArchiveClient nasa,
    IOptions<CatalogRefreshOptions> options,
    TimeProvider time,
    ILogger<CatalogRefreshService> logger) : BackgroundService
{
    private static readonly TimeSpan RetryDelay = TimeSpan.FromHours(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var settings = options.Value;
        if (!settings.Enabled)
            return;

        while (!stoppingToken.IsCancellationRequested)
        {
            var due = store.Current.Snapshot.GeneratedAt + settings.Interval;
            var wait = due - time.GetUtcNow();
            if (wait > TimeSpan.Zero)
            {
                logger.LogInformation("Next catalogue refresh at {Due:u}", due);
                await Task.Delay(wait, time, stoppingToken);
            }

            try
            {
                await RefreshAsync(settings, stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Catalogue refresh failed; retrying in {Delay}", RetryDelay);
                await Task.Delay(RetryDelay, time, stoppingToken);
            }
        }
    }

    public async Task RefreshAsync(CatalogRefreshOptions settings, CancellationToken cancellationToken)
    {
        var previous = store.Current.Snapshot;
        var rows = await nasa.GetConfirmedPlanetsAsync(cancellationToken);
        var next = CatalogBuilder.Build(rows, previous, time.GetUtcNow());

        if (next.Planets.Count < previous.Planets.Count * settings.MinimumPlanetRatio)
        {
            // Protects against a partial or broken download replacing a good catalogue. The
            // served catalogue keeps its timestamp, so the refresh is retried after RetryDelay.
            throw new InvalidDataException(
                $"Rebuilt catalogue has {next.Planets.Count} planets, the current one {previous.Planets.Count}; not replacing it.");
        }

        if (!string.IsNullOrEmpty(settings.StoragePath))
        {
            var directory = Path.GetDirectoryName(Path.GetFullPath(settings.StoragePath))!;
            Directory.CreateDirectory(directory);
            // Same file name with a leading dot, so the extension (and gzip compression) is kept.
            var temp = Path.Combine(directory, "." + Path.GetFileName(settings.StoragePath));
            await next.SaveAsync(temp, cancellationToken);
            File.Move(temp, settings.StoragePath, overwrite: true);
        }

        store.Replace(next);
        logger.LogInformation("Catalogue refreshed: {Stars} stars, {Planets} planets\n{Report}",
            next.Stars.Count, next.Planets.Count, CatalogComparison.Report(previous, next));
    }

    /// <summary>
    /// Picks the catalogue to serve at startup: the last refreshed one if it is newer than
    /// the one shipped with the deployment.
    /// </summary>
    public static async Task<CatalogSnapshot> LoadInitialAsync(string bundledPath, CatalogRefreshOptions settings, ILogger logger)
    {
        var bundled = await CatalogSnapshot.LoadAsync(bundledPath);
        if (!settings.Enabled || string.IsNullOrEmpty(settings.StoragePath) || !File.Exists(settings.StoragePath))
            return bundled;

        try
        {
            var stored = await CatalogSnapshot.LoadAsync(settings.StoragePath);
            return stored.GeneratedAt > bundled.GeneratedAt ? stored : bundled;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not read refreshed catalogue {Path}; serving the bundled one", settings.StoragePath);
            return bundled;
        }
    }
}
