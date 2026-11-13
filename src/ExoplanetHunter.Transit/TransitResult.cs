namespace ExoplanetHunter.Transit;

/// <summary>
/// Precomputed transit search for one star: what the Transit Lab view shows. Served as JSON
/// by the API at /api/v2/Transits/{starId}. Times are BTJD (BJD − 2457000), flux is
/// normalised to 1, depths are in ppm and durations in hours.
/// </summary>
public sealed record TransitResult
{
    public required int StarId { get; init; }
    public required string StarName { get; init; }
    public required long TicId { get; init; }
    public required string Mission { get; init; }
    public required IReadOnlyList<int> Sectors { get; init; }
    public required int Points { get; init; }
    public required DateTimeOffset GeneratedAt { get; init; }

    /// <summary>
    /// <see cref="TransitAnalyzer.Version"/> that produced the result. 0 for results written
    /// before the field existed.
    /// </summary>
    public int AnalysisVersion { get; init; }

    /// <summary>Detrended light curve, binned for display.</summary>
    public required TimeSeries LightCurve { get; init; }

    /// <summary>BLS power against trial period (days), reduced for display.</summary>
    public required TimeSeries Periodogram { get; init; }

    public required IReadOnlyList<TransitCandidateResult> Candidates { get; init; }

    /// <summary>Catalogue planets of the star and whether the search found them.</summary>
    public required IReadOnlyList<KnownPlanetMatch> KnownPlanets { get; init; }
}

/// <summary>Two equally long arrays, e.g. time and flux, or period and power.</summary>
public sealed record TimeSeries(IReadOnlyList<double> X, IReadOnlyList<double> Y);

public sealed record TransitCandidateResult
{
    public required double PeriodDays { get; init; }
    public required double EpochBtjd { get; init; }
    public required double DepthPpm { get; init; }
    public required double DurationHours { get; init; }
    public required double RadiusRatio { get; init; }
    public required double Snr { get; init; }
    public required double OddEvenDepthRatio { get; init; }
    public required double Score { get; init; }
    public required string Disposition { get; init; }
    public required IReadOnlyList<string> Warnings { get; init; }

    /// <summary>Light curve folded on the period: phase 0–1 (transit at 0) against flux.</summary>
    public required TimeSeries Folded { get; init; }

    /// <summary>Name of the catalogue planet this candidate corresponds to, if any.</summary>
    public string? MatchedPlanet { get; init; }

    /// <summary>
    /// Set when the signal has the period of a found planet but sits half an orbit after its
    /// transits: the planet passing behind its star (secondary eclipse), not another planet.
    /// </summary>
    public string? SecondaryEclipseOf { get; init; }
}

public sealed record KnownPlanetMatch(string Name, double? PeriodDays, string Status, int? CandidateIndex);

public static class MatchStatus
{
    public const string Found = "found";
    public const string FoundAtHarmonic = "found-at-harmonic";
    public const string NotFound = "not-found";

    /// <summary>The period is too long for the observed time span to show two transits.</summary>
    public const string OutOfRange = "out-of-range";
    public const string NoPeriod = "no-period";
}

/// <summary>Item of /api/v2/Transits: one analysed star.</summary>
public sealed record TransitIndexEntry(int StarId, string StarName, long TicId, IReadOnlyList<int> Sectors,
    int Candidates, int KnownPlanetsFound, int KnownPlanets, DateTimeOffset GeneratedAt);
