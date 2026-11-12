namespace ExoplanetHunter.Transit.Tests;

[TestClass]
public sealed class ResultReuseTests
{
    private static TransitResult Previous(int version = TransitAnalyzer.Version) => new()
    {
        StarId = 1, StarName = "WASP-18", TicId = 100100827, Mission = "TESS", Sectors = [104, 105, 106], Points = 1,
        GeneratedAt = DateTimeOffset.UnixEpoch, AnalysisVersion = version,
        LightCurve = new TimeSeries([], []), Periodogram = new TimeSeries([], []), Candidates = [],
        KnownPlanets = [new KnownPlanetMatch("WASP-18 b", 0.94145, MatchStatus.Found, 0)],
    };

    [TestMethod]
    public void SameSectorsPlanetsAndVersion_IsReused() =>
        Assert.IsTrue(ResultReuse.CanReuse(Previous(), 100100827, [104, 105, 106], ["WASP-18 b"]));

    [TestMethod]
    public void AnythingChanged_IsAnalysedAgain()
    {
        Assert.IsFalse(ResultReuse.CanReuse(null, 100100827, [104, 105, 106], ["WASP-18 b"]));
        Assert.IsFalse(ResultReuse.CanReuse(Previous(), 100100827, [105, 106, 107], ["WASP-18 b"]), "new sector");
        Assert.IsFalse(ResultReuse.CanReuse(Previous(), 100100827, [104, 105, 106], ["WASP-18 b", "WASP-18 c"]), "new planet");
        Assert.IsFalse(ResultReuse.CanReuse(Previous(version: 0), 100100827, [104, 105, 106], ["WASP-18 b"]), "older analysis");
        Assert.IsFalse(ResultReuse.CanReuse(Previous(), 1, [104, 105, 106], ["WASP-18 b"]), "other TIC");
    }
}
