namespace ExoplanetHunter.Transit;

/// <summary>
/// Decides whether a star's previous result can be kept instead of being analysed again. The
/// scheduled run uses it so that a week without new TESS data downloads nothing and leaves the
/// result files, and therefore git, unchanged.
/// </summary>
public static class ResultReuse
{
    public static bool CanReuse(TransitResult? previous, long ticId, IReadOnlyList<int> sectors, IEnumerable<string> knownPlanets) =>
        previous is not null
        && previous.AnalysisVersion == TransitAnalyzer.Version
        && previous.TicId == ticId
        && previous.Sectors.SequenceEqual(sectors)
        && previous.KnownPlanets.Select(k => k.Name).Order(StringComparer.Ordinal)
            .SequenceEqual(knownPlanets.Order(StringComparer.Ordinal));
}
