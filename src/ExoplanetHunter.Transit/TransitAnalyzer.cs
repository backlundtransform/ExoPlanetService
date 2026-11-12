using CSharpNumerics.Engines.Exoplanet.Data;
using CSharpNumerics.Engines.Exoplanet.Enums;
using CSharpNumerics.Engines.Exoplanet.Pipeline;
using ExoplanetHunter.Domain.Catalog;

namespace ExoplanetHunter.Transit;

public sealed record TransitAnalysisOptions
{
    /// <summary>Points are averaged into bins of this length before the search (TESS 2-min data → 10 min).</summary>
    public double SearchBinMinutes { get; init; } = 10;

    /// <summary>Bin length of the light curve shown in the app.</summary>
    public double DisplayBinMinutes { get; init; } = 30;

    public int PeriodogramPoints { get; init; } = 1500;

    public double MinPeriodDays { get; init; } = 0.3;

    /// <summary>
    /// The BLS grid in CSharpNumerics is linear in period. 20,000 trials over one or a few
    /// sectors keep the phase drift below a transit duration for short periods.
    /// </summary>
    public int TrialPeriods { get; init; } = 20_000;

    public int MaxPlanets { get; init; } = 3;

    /// <summary>Relative period difference within which a candidate counts as a catalogue planet.</summary>
    public double MatchTolerance { get; init; } = 0.01;
}

/// <summary>
/// Runs the CSharpNumerics.Engines exoplanet pipeline (detrend → BLS → fit → validate, repeated
/// for several planets) on TESS sectors of one star and packages the result for the app.
/// </summary>
public sealed class TransitAnalyzer(TransitAnalysisOptions? options = null)
{
    /// <summary>
    /// Bump when a change to the analysis (or to the Engines package) changes the results, so the
    /// next scheduled run re-analyses every star instead of keeping results it would reuse.
    /// </summary>
    public const int Version = 1;

    private readonly TransitAnalysisOptions _options = options ?? new();

    public TransitResult Analyze(Star star, IEnumerable<Planet> knownPlanets, long ticId,
        IReadOnlyList<SectorLightCurve> sectors, DateTimeOffset generatedAt)
    {
        var (time, flux, error) = Join(sectors);
        if (time.Length < 100)
            throw new InvalidDataException($"Only {time.Length} good points in the selected sectors.");

        (time, flux, error) = Bin(time, flux, error, _options.SearchBinMinutes / 1440.0);

        double span = time[^1] - time[0];
        var config = new TransitDetectionConfig
        {
            MinPeriodDays = _options.MinPeriodDays,
            // Two transits must fall inside the observed span.
            MaxPeriodDays = Math.Clamp(span / 2, _options.MinPeriodDays * 2, 100),
            NumTrialPeriods = _options.TrialPeriods,
            MaxPlanets = _options.MaxPlanets,
            DetrendingMethod = DetrendingMethod.MedianFilter,
            // ~17 h at 10-minute bins: wider than any transit we search for, narrower than stellar variability.
            DetrendingWindowSize = OddAtLeast3((int)Math.Round(1000 / _options.SearchBinMinutes)),
        };

        var lightCurve = new LightCurve(time, flux, error, new int[time.Length],
            new LightCurveMetadata($"TIC {ticId}", "TESS", 0, CadenceType.Short));

        var candidates = TransitDetectionPipeline.Detect(lightCurve, config);
        var processed = LightCurvePreprocessor.Preprocess(lightCurve, config);
        var search = PeriodSearcher.Search(processed, config);

        var candidateResults = candidates.Select(c => ToResult(c, processed, config)).ToList();
        var known = MatchKnownPlanets(knownPlanets, candidateResults, config.MaxPeriodDays);
        foreach (var match in known.Where(k => k.CandidateIndex is not null))
            candidateResults[match.CandidateIndex!.Value] = candidateResults[match.CandidateIndex.Value] with { MatchedPlanet = match.Name };
        candidateResults = MarkSecondaryEclipses(candidateResults);

        var (displayTime, displayFlux, _) = Bin(processed.Time, processed.Flux, processed.FluxError, _options.DisplayBinMinutes / 1440.0);

        return new TransitResult
        {
            StarId = star.Id,
            StarName = star.Name,
            TicId = ticId,
            Mission = "TESS",
            Sectors = sectors.Select(s => s.Sector).Order().ToArray(),
            Points = time.Length,
            GeneratedAt = generatedAt,
            AnalysisVersion = Version,
            LightCurve = new TimeSeries(Round(displayTime, 5), Round(displayFlux, 6)),
            Periodogram = ReducePeriodogram(search.Periods.Values, search.Power.Values, _options.PeriodogramPoints),
            Candidates = candidateResults,
            KnownPlanets = known,
        };
    }

    private static TransitCandidateResult ToResult(TransitCandidate candidate, LightCurve processed, TransitDetectionConfig config)
    {
        // The pipeline's own validation, made with earlier signals masked; it is what the candidate was accepted on.
        var validation = candidate.Validation ?? TransitValidator.Validate(candidate, processed, config);
        var p = candidate.Parameters;

        // The engine reports the epoch modulo the period; report the first transit in the data.
        double firstTransit = p.Period > 0
            ? p.Epoch + Math.Ceiling((processed.Time[0] - p.Epoch) / p.Period) * p.Period
            : p.Epoch;

        // Centre the folded curve on the transit: phase 0.75 → −0.25.
        var folded = candidate.PhaseFoldedCurve.Time
            .Zip(candidate.PhaseFoldedCurve.Flux)
            .Where(x => double.IsFinite(x.Second))
            .Select(x => (Phase: x.First >= 0.5 ? x.First - 1 : x.First, Flux: x.Second))
            .OrderBy(x => x.Phase)
            .ToArray();

        return new TransitCandidateResult
        {
            PeriodDays = Finite(p.Period),
            EpochBtjd = Finite(firstTransit),
            DepthPpm = Finite(p.Depth * 1e6),
            DurationHours = Finite(p.Duration * 24),
            RadiusRatio = Finite(p.RadiusRatio),
            Snr = Finite(validation.Snr),
            OddEvenDepthRatio = Finite(validation.OddEvenDepthRatio),
            Score = Finite(validation.Score),
            Disposition = candidate.Disposition.ToString(),
            Warnings = validation.Warnings,
            Folded = new TimeSeries(Round(folded.Select(x => x.Phase).ToArray(), 5), Round(folded.Select(x => x.Flux).ToArray(), 6)),
        };
    }

    internal IReadOnlyList<KnownPlanetMatch> MatchKnownPlanets(IEnumerable<Planet> planets,
        IReadOnlyList<TransitCandidateResult> candidates, double maxPeriod)
    {
        var matches = new List<KnownPlanetMatch>();
        var used = new HashSet<int>();

        foreach (var planet in planets.OrderBy(p => p.Period ?? double.MaxValue))
        {
            if (planet.Period is not { } period || period <= 0)
            {
                matches.Add(new KnownPlanetMatch(planet.Name, null, MatchStatus.NoPeriod, null));
                continue;
            }

            if (period > maxPeriod)
            {
                matches.Add(new KnownPlanetMatch(planet.Name, period, MatchStatus.OutOfRange, null));
                continue;
            }

            int exact = FindCandidate(candidates, used, c => Math.Abs(c.PeriodDays / period - 1) < _options.MatchTolerance);
            if (exact >= 0)
            {
                used.Add(exact);
                matches.Add(new KnownPlanetMatch(planet.Name, period, MatchStatus.Found, exact));
                continue;
            }

            // BLS often locks on to half or twice the true period.
            int harmonic = FindCandidate(candidates, used, c =>
                Math.Abs(c.PeriodDays / period - 2) < 2 * _options.MatchTolerance ||
                Math.Abs(c.PeriodDays / period - 0.5) < _options.MatchTolerance / 2);
            if (harmonic >= 0)
            {
                used.Add(harmonic);
                matches.Add(new KnownPlanetMatch(planet.Name, period, MatchStatus.FoundAtHarmonic, harmonic));
                continue;
            }

            matches.Add(new KnownPlanetMatch(planet.Name, period, MatchStatus.NotFound, null));
        }

        return matches;
    }

    /// <summary>
    /// Marks unmatched candidates that repeat a matched planet's period with a phase offset of
    /// one half: the planet's occultation behind the star, common for hot Jupiters in TESS data.
    /// </summary>
    internal List<TransitCandidateResult> MarkSecondaryEclipses(List<TransitCandidateResult> candidates)
    {
        var planets = candidates.Where(c => c.MatchedPlanet is not null).ToArray();
        return candidates.Select(c =>
        {
            if (c.MatchedPlanet is not null)
                return c;

            var primary = planets.FirstOrDefault(p =>
            {
                if (Math.Abs(c.PeriodDays / p.PeriodDays - 1) > _options.MatchTolerance)
                    return false;
                double offset = ((c.EpochBtjd - p.EpochBtjd) / p.PeriodDays % 1 + 1) % 1;
                return Math.Abs(offset - 0.5) < 0.05;
            });

            return primary is null ? c : c with { SecondaryEclipseOf = primary.MatchedPlanet };
        }).ToList();
    }

    private static int FindCandidate(IReadOnlyList<TransitCandidateResult> candidates, HashSet<int> used, Func<TransitCandidateResult, bool> predicate)
    {
        for (int i = 0; i < candidates.Count; i++)
        {
            if (!used.Contains(i) && predicate(candidates[i]))
                return i;
        }

        return -1;
    }

    private static (double[] Time, double[] Flux, double[] Error) Join(IEnumerable<SectorLightCurve> sectors)
    {
        var points = sectors
            .Select(s => s.Normalized())
            .SelectMany(n => n.Time.Select((t, i) => (Time: t, Flux: n.Flux[i], Error: n.FluxError[i])))
            .OrderBy(p => p.Time)
            .ToArray();
        return (points.Select(p => p.Time).ToArray(), points.Select(p => p.Flux).ToArray(), points.Select(p => p.Error).ToArray());
    }

    /// <summary>Averages consecutive points into bins of the given width (days); empty bins are dropped.</summary>
    internal static (double[] Time, double[] Flux, double[] Error) Bin(double[] time, double[] flux, double[] error, double width)
    {
        var bins = new List<(double Time, double Flux, double Error)>();
        int start = 0;
        while (start < time.Length)
        {
            // Compare bin numbers rather than edges, so rounding can never stall the loop.
            long bin = (long)Math.Floor((time[start] - time[0]) / width);
            int end = start;
            double sumT = 0, sumF = 0, sumE2 = 0;
            while (end < time.Length && (end == start || (long)Math.Floor((time[end] - time[0]) / width) == bin))
            {
                sumT += time[end];
                sumF += flux[end];
                sumE2 += error[end] * error[end];
                end++;
            }

            int n = end - start;
            bins.Add((sumT / n, sumF / n, Math.Sqrt(sumE2) / n));
            start = end;
        }

        return (bins.Select(b => b.Time).ToArray(), bins.Select(b => b.Flux).ToArray(), bins.Select(b => b.Error).ToArray());
    }

    /// <summary>Keeps the highest power in each of <paramref name="points"/> period buckets, so peaks survive.</summary>
    internal static TimeSeries ReducePeriodogram(double[] periods, double[] power, int points)
    {
        if (periods.Length <= points)
            return new TimeSeries(Round(periods, 5), Round(power.Select(Finite).ToArray(), 6));

        var x = new double[points];
        var y = new double[points];
        double perBucket = (double)periods.Length / points;
        for (int b = 0; b < points; b++)
        {
            int from = (int)(b * perBucket), to = Math.Min(periods.Length, (int)((b + 1) * perBucket));
            int best = from;
            for (int i = from; i < to; i++)
            {
                if (Finite(power[i]) > Finite(power[best]))
                    best = i;
            }

            x[b] = periods[best];
            y[b] = Finite(power[best]);
        }

        return new TimeSeries(Round(x, 5), Round(y, 6));
    }

    private static double Finite(double value) => double.IsFinite(value) ? value : 0;

    private static double[] Round(double[] values, int digits) => values.Select(v => Math.Round(Finite(v), digits)).ToArray();

    private static int OddAtLeast3(int n) => Math.Max(3, n % 2 == 0 ? n + 1 : n);
}
