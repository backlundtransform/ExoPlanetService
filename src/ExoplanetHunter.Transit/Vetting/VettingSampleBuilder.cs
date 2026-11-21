using CSharpNumerics.Engines.Exoplanet.Data;
using CSharpNumerics.Engines.Exoplanet.Enums;
using CSharpNumerics.Engines.Exoplanet.Features;
using CSharpNumerics.Engines.Exoplanet.Pipeline;
using CSharpNumerics.Engines.Exoplanet.Simulation;

namespace ExoplanetHunter.Transit.Vetting;

/// <summary>Which synthetic examples to add for each star.</summary>
[Flags]
public enum SyntheticKinds
{
    None = 0,
    Planets = 1,
    Binaries = 2,
    Inverted = 4,
    Scrambled = 8,
    All = Planets | Binaries | Inverted | Scrambled,
}

public sealed record VettingSampleOptions
{
    public TransitViewConfig Views { get; init; } = new();

    /// <summary>Shortest detrending window; widened to 3 transit durations for long transits.</summary>
    public double DetrendWindowDays { get; init; } = TimeWindowDetrender.DefaultWindowDays;

    /// <summary>
    /// A TOI is skipped when its predicted transit times may have drifted by more than this
    /// fraction of the duration over the data (1σ), because its views would be smeared.
    /// </summary>
    public double MaxDriftDurations { get; init; } = 0.5;

    /// <summary>Synthetic signals get a period between this and a third of the observed span (at most 20 days).</summary>
    public double MinSyntheticPeriodDays { get; init; } = 0.5;

    /// <summary>Bin length before the BLS search on inverted and scrambled light curves.</summary>
    public double SearchBinMinutes { get; init; } = 10;

    public int SearchTrialPeriods { get; init; } = 20_000;

    public SyntheticKinds Synthetic { get; init; } = SyntheticKinds.All;
}

/// <summary>An example that could not be built, and why.</summary>
public sealed record SkippedSample(string Id, long TicId, string Reason);

/// <summary>
/// Turns one star's TESS light curve into vetting examples: one per labelled TOI, at the
/// catalogue ephemeris, plus synthetic planets, eclipsing binaries and false alarms made from
/// the same light curve.
/// </summary>
/// <remarks>
/// The light curve is detrended with the biweight filter while every known TOI of the star is
/// masked, so the trend does not eat into any transit. Synthetic examples use the star's own
/// noise and variability, which keeps the model from telling real and synthetic examples apart
/// by noise alone.
/// </remarks>
public sealed class VettingSampleBuilder(VettingSampleOptions? options = null)
{
    private static readonly StellarProperties SunLike = new(5772, 1.0, 1.0, 4.438, 0.0, SpectralType.G);

    private readonly VettingSampleOptions _options = options ?? new();

    public VettingDatasetHeader Header => VettingDatasetHeader.Current(_options.Views);

    /// <param name="tois">All TOIs of the star (all of them are masked when detrending).</param>
    /// <param name="labelledOnly">Build examples only for TOIs with a planet / not-planet disposition.</param>
    public (List<VettingSample> Samples, List<SkippedSample> Skipped) Build(long ticId,
        IReadOnlyList<SectorLightCurve> sectors, IReadOnlyList<ToiEntry> tois, bool labelledOnly = true)
    {
        var samples = new List<VettingSample>();
        var skipped = new List<SkippedSample>();

        var (time, flux, error) = TransitAnalyzer.Join(sectors);
        if (time.Length < 1000)
        {
            skipped.AddRange(tois.Select(t => new SkippedSample($"TOI-{t.Toi}", ticId, $"only {time.Length} good points")));
            return (samples, skipped);
        }

        var lc = new LightCurve(time, flux, error, new int[time.Length],
            new LightCurveMetadata($"TIC {ticId}", "TESS", 0, CadenceType.Short));
        var star = Star(tois);
        var known = tois.Where(t => t.HasEphemeris).Select(Ephemeris).ToList();
        var detrended = Detrend(lc, known);
        double middle = 0.5 * (time[0] + time[^1]);

        foreach (var toi in tois)
        {
            string id = $"TOI-{toi.Toi}";
            int? label = toi.Label;
            if (labelledOnly && label is null) continue;
            if (!toi.HasEphemeris) { skipped.Add(new(id, ticId, "no usable period, epoch or duration")); continue; }
            if (toi.PredictionUncertaintyDays(middle) > _options.MaxDriftDurations * toi.DurationDays)
            {
                skipped.Add(new(id, ticId, "ephemeris too uncertain for these sectors"));
                continue;
            }

            Add(samples, skipped, id, ticId, label ?? -1, SampleSource.Toi, detrended, Ephemeris(toi), star);
        }

        if (_options.Synthetic != SyntheticKinds.None)
            AddSynthetic(samples, skipped, ticId, lc, detrended, known, star);

        return (samples, skipped);
    }

    private void AddSynthetic(List<VettingSample> samples, List<SkippedSample> skipped, long ticId,
        LightCurve lc, LightCurve detrended, List<TransitParameters> known, StellarProperties? star)
    {
        var rng = new Random(unchecked((int)(ticId * 2654435761L)));
        double span = lc.Time[^1] - lc.Time[0];
        double maxPeriod = Math.Min(20.0, span / 3.0);
        double sigma = RobustSigma(detrended.Flux);
        var host = star is { Radius: > 0 } && Mass(star) > 0
            ? new StellarProperties(star.EffectiveTemp, star.Radius, Mass(star), star.SurfaceGravity, 0, SpectralType.G)
            : SunLike;

        if (maxPeriod > _options.MinSyntheticPeriodDays)
        {
            if (_options.Synthetic.HasFlag(SyntheticKinds.Planets))
            {
                string id = $"TIC {ticId} injected";
                try
                {
                    double period = LogUniform(rng, _options.MinSyntheticPeriodDays, maxPeriod);
                    double b = 0.8 * rng.NextDouble();
                    double duration = TransitInjector.Duration(period, 0.05, b, host);
                    // Depth for a signal-to-noise between 8 and 40 over the whole light curve.
                    double inTransitPoints = lc.Length * duration / period;
                    double depth = Uniform(rng, 8, 40) * sigma / Math.Sqrt(Math.Max(inTransitPoints, 1));
                    double k = Math.Sqrt(depth);
                    duration = TransitInjector.Duration(period, k, b, host);
                    var transit = new TransitParameters(period, lc.Time[0] + period * rng.NextDouble(), depth, duration, k, b, 0);

                    var injected = TransitInjector.Inject(lc, transit, host);
                    Add(samples, skipped, id, ticId, 1, SampleSource.InjectedPlanet,
                        Detrend(injected, [.. known, transit]), transit, star);
                }
                catch (Exception ex) when (ex is not OperationCanceledException) { skipped.Add(new(id, ticId, ex.Message)); }
            }

            if (_options.Synthetic.HasFlag(SyntheticKinds.Binaries))
            {
                string id = $"TIC {ticId} binary";
                try
                {
                    double period = LogUniform(rng, _options.MinSyntheticPeriodDays, maxPeriod);
                    double duration = Math.Min(TransitInjector.Duration(period, 0.3, 0, host) * Uniform(rng, 0.8, 1.5), 0.2 * period);
                    double inTransitPoints = lc.Length * duration / period;
                    double primary = Math.Min(0.5, Uniform(rng, 15, 80) * sigma / Math.Sqrt(Math.Max(inTransitPoints, 1)) * 2);
                    double secondary = primary * 0.5 * rng.NextDouble();
                    double oddRatio = rng.NextDouble() < 0.5 ? 1.0 : Uniform(rng, 0.4, 0.9);
                    double epoch = lc.Time[0] + period * rng.NextDouble();

                    var binary = SyntheticNegatives.InjectEclipsingBinary(lc, period, epoch, duration, primary, secondary, oddRatio);
                    var ephemeris = TransitCandidate.FromEphemeris(period, epoch, duration, primary / 2).Parameters;
                    Add(samples, skipped, id, ticId, 0, SampleSource.EclipsingBinary,
                        Detrend(binary, [.. known, ephemeris]), ephemeris, star);
                }
                catch (Exception ex) when (ex is not OperationCanceledException) { skipped.Add(new(id, ticId, ex.Message)); }
            }
        }

        if (_options.Synthetic.HasFlag(SyntheticKinds.Inverted))
            AddFalseAlarm(samples, skipped, $"TIC {ticId} inverted", ticId, SampleSource.Inverted,
                Detrend(SyntheticNegatives.Invert(lc), []), star);

        if (_options.Synthetic.HasFlag(SyntheticKinds.Scrambled))
            AddFalseAlarm(samples, skipped, $"TIC {ticId} scrambled", ticId, SampleSource.Scrambled,
                Detrend(SyntheticNegatives.Scramble(lc, 2.0, rng.Next()), []), star);
    }

    /// <summary>The strongest BLS signal in a light curve with no real transit at that period, labelled "not a planet".</summary>
    private void AddFalseAlarm(List<VettingSample> samples, List<SkippedSample> skipped, string id, long ticId,
        SampleSource source, LightCurve detrended, StellarProperties? star)
    {
        try
        {
            var (time, flux, error) = TransitAnalyzer.Bin(detrended.Time, detrended.Flux, detrended.FluxError,
                _options.SearchBinMinutes / 1440.0);
            var binned = LightCurve.FromArrays(time, flux, error);
            double span = time[^1] - time[0];
            var config = new TransitDetectionConfig
            {
                MinPeriodDays = _options.MinSyntheticPeriodDays,
                MaxPeriodDays = Math.Clamp(span / 2, _options.MinSyntheticPeriodDays * 2, 100),
                NumTrialPeriods = _options.SearchTrialPeriods,
            };

            var search = PeriodSearcher.Search(binned, config);
            var fit = TransitFitter.Fit(binned, search.BestPeriod, search.BestEpoch).Parameters;
            if (!(fit.Period > 0) || !(fit.Duration > 0) || fit.Duration >= fit.Period / 2)
            {
                skipped.Add(new(id, ticId, "the period search found no usable signal"));
                return;
            }

            Add(samples, skipped, id, ticId, 0, source, detrended, fit, star);
        }
        catch (Exception ex) when (ex is not OperationCanceledException) { skipped.Add(new(id, ticId, ex.Message)); }
    }

    private void Add(List<VettingSample> samples, List<SkippedSample> skipped, string id, long ticId, int label,
        SampleSource source, LightCurve detrended, TransitParameters p, StellarProperties? star)
    {
        try
        {
            var features = VettingFeatureExtractor.Extract(detrended, p, star);
            if (features[VettingFeatures.TransitCount] < 1)
            {
                skipped.Add(new(id, ticId, "no transit falls in the selected sectors"));
                return;
            }

            var views = TransitViewBuilder.Build(detrended, p, _options.Views);
            samples.Add(new VettingSample(id, ticId, label, source, (float)p.Period,
                Floats(views.Global), Floats(views.Local), Floats(views.LocalOdd), Floats(views.LocalEven),
                Floats(views.Secondary), Floats(VettingFeatures.ToVector(features))));
        }
        catch (Exception ex) when (ex is not OperationCanceledException) { skipped.Add(new(id, ticId, ex.Message)); }
    }

    private LightCurve Detrend(LightCurve lc, IReadOnlyCollection<TransitParameters> mask)
    {
        double longest = mask.Count == 0 ? 0 : mask.Max(p => p.Duration);
        double window = Math.Max(_options.DetrendWindowDays, 3 * longest);
        var masked = mask.Count == 0 ? null : TransitMask.Create(lc.Time, mask);
        return TimeWindowDetrender.Detrend(lc, window, DetrendingMethod.Biweight, masked);
    }

    private static TransitParameters Ephemeris(ToiEntry toi) =>
        TransitCandidate.FromEphemeris(toi.PeriodDays, toi.EpochBtjd, toi.DurationDays,
            double.IsFinite(toi.DepthPpm) && toi.DepthPpm > 0 ? toi.DepthPpm * 1e-6 : 0).Parameters;

    /// <summary>The host star from the TOI table (radius, Teff, log g; mass follows from log g).</summary>
    internal static StellarProperties? Star(IReadOnlyList<ToiEntry> tois)
    {
        var t = tois.FirstOrDefault(t => t.StarRadius is > 0) ?? tois.FirstOrDefault();
        if (t is null || (t.StarRadius is null && t.StarTeff is null && t.StarLogg is null)) return null;
        return new StellarProperties(t.StarTeff ?? 0, t.StarRadius ?? 0, 0, t.StarLogg ?? 0, 0, SpectralType.G);
    }

    private static double Mass(StellarProperties star) =>
        star.Mass > 0 ? star.Mass
        : star.SurfaceGravity > 0 && star.Radius > 0 ? Math.Pow(10, star.SurfaceGravity - 4.438) * star.Radius * star.Radius
        : 0;

    private static double RobustSigma(double[] flux)
    {
        var finite = flux.Where(double.IsFinite).ToArray();
        double median = SectorLightCurve.Median(finite);
        return 1.4826 * SectorLightCurve.Median(finite.Select(f => Math.Abs(f - median)));
    }

    private static double Uniform(Random rng, double low, double high) => low + (high - low) * rng.NextDouble();

    private static double LogUniform(Random rng, double low, double high) =>
        Math.Exp(Uniform(rng, Math.Log(low), Math.Log(high)));

    private static float[] Floats(double[] values) => Array.ConvertAll(values, v => (float)v);
}
