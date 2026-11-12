using ExoplanetHunter.Domain.Catalog;
using ExoplanetHunter.Transit.Mast;

namespace ExoplanetHunter.Transit.Tests;

[TestClass]
public sealed class TransitAnalyzerTests
{
    private static TransitCandidateResult Candidate(double period) => new()
    {
        PeriodDays = period, EpochBtjd = 0, DepthPpm = 1000, DurationHours = 2, RadiusRatio = 0.03,
        Snr = 20, OddEvenDepthRatio = 1, Score = 0.9, Disposition = "Candidate", Warnings = [],
        Folded = new TimeSeries([], []),
    };

    private static Planet Planet(string name, double? period) => new() { Id = 1, StarId = 1, Name = name, Period = period };

    [TestMethod]
    public void MatchKnownPlanets_ClassifiesEachPlanet()
    {
        var analyzer = new TransitAnalyzer();
        var candidates = new[] { Candidate(0.9415), Candidate(7.4) };

        var matches = analyzer.MatchKnownPlanets(
            [Planet("b", 0.94145), Planet("c", 3.7), Planet("d", 50), Planet("e", null), Planet("f", 3.7 * 2)],
            candidates, maxPeriod: 13);

        Assert.AreEqual(MatchStatus.Found, matches.Single(m => m.Name == "b").Status);
        Assert.AreEqual(0, matches.Single(m => m.Name == "b").CandidateIndex);
        Assert.AreEqual(MatchStatus.FoundAtHarmonic, matches.Single(m => m.Name == "c").Status); // 7.4 = 2 × 3.7
        Assert.AreEqual(MatchStatus.OutOfRange, matches.Single(m => m.Name == "d").Status);
        Assert.AreEqual(MatchStatus.NoPeriod, matches.Single(m => m.Name == "e").Status);
        // The 7.4-day candidate is already used by c, so f (7.4 d) is not found again.
        Assert.AreEqual(MatchStatus.NotFound, matches.Single(m => m.Name == "f").Status);
    }

    [TestMethod]
    public void MarkSecondaryEclipses_HalfPhaseAtSamePeriod_IsTheOccultation()
    {
        var planet = Candidate(0.94145) with { EpochBtjd = 4180.699, MatchedPlanet = "WASP-18 b" };
        var occultation = Candidate(0.94139) with { EpochBtjd = 4180.230 };   // 0.498 of a period later
        var other = Candidate(0.94145) with { EpochBtjd = 4180.9 };           // same period, phase 0.21

        var marked = new TransitAnalyzer().MarkSecondaryEclipses([planet, occultation, other]);

        Assert.AreEqual("WASP-18 b", marked[1].SecondaryEclipseOf);
        Assert.IsNull(marked[2].SecondaryEclipseOf);
        Assert.IsNull(marked[0].SecondaryEclipseOf);
    }

    [TestMethod]
    public void Bin_AveragesPointsPerBin()
    {
        var (time, flux, error) = TransitAnalyzer.Bin([0, 0.1, 0.2, 1.05, 1.1], [1, 2, 3, 4, 6], [1, 1, 1, 1, 1], width: 1);

        CollectionAssert.AreEqual(new[] { 0.1, 1.075 }, time.Select(t => Math.Round(t, 10)).ToArray());
        CollectionAssert.AreEqual(new[] { 2.0, 5.0 }, flux);
        Assert.AreEqual(Math.Sqrt(3) / 3, error[0], 1e-12);
    }

    [TestMethod]
    public void ReducePeriodogram_KeepsPeaks()
    {
        var periods = Enumerable.Range(0, 1000).Select(i => 1 + i * 0.01).ToArray();
        var power = new double[1000];
        power[537] = 9;

        var reduced = TransitAnalyzer.ReducePeriodogram(periods, power, points: 100);

        Assert.HasCount(100, reduced.X);
        Assert.AreEqual(9, reduced.Y.Max());
        Assert.AreEqual(periods[537], reduced.X[reduced.Y.ToList().IndexOf(9)], 1e-9);
    }

    [TestMethod]
    public void SectorSelection_PicksLongestConsecutiveRun()
    {
        TessLightCurveFile File(int sector) => new(sector, 120, $"s{sector}", $"mast:s{sector}");

        var chosen = SectorSelection.Choose([File(1), File(2), File(13), File(27), File(28), File(29), File(30), File(68)], maxSectors: 3);

        CollectionAssert.AreEqual(new[] { 28, 29, 30 }, chosen.Select(f => f.Sector).ToArray());
    }

    [TestMethod]
    public void Analyze_SyntheticTransits_FindsThePeriod()
    {
        // 27 days of 2-minute data with a 1% deep, 2-hour transit every 3.2 days.
        const double period = 3.2, duration = 2.0 / 24, depth = 0.01;
        var rng = new Random(42);
        int n = (int)(27 * 720);
        var time = Enumerable.Range(0, n).Select(i => 1000 + i / 720.0).ToArray();
        var flux = time.Select(t =>
        {
            double phase = ((t - 1001) % period + period) % period;
            bool inTransit = phase < duration / 2 || phase > period - duration / 2;
            return (float)((inTransit ? 1 - depth : 1) * 10_000 + rng.NextDouble() * 20 - 10);
        }).ToArray();
        var fits = FitsBinaryTableTests.BuildFits(time, flux, time.Select(_ => 6f).ToArray(), new int[n]);

        var star = new Star { Id = 9, Name = "Test" };
        var result = new TransitAnalyzer(new TransitAnalysisOptions { TrialPeriods = 8000 })
            .Analyze(star, [Planet("Test b", period)], 1, [SectorLightCurve.FromFits(fits, 1)], DateTimeOffset.UnixEpoch);

        Assert.IsNotEmpty(result.Candidates);
        Assert.AreEqual(period, result.Candidates[0].PeriodDays, 0.02);
        Assert.AreEqual(MatchStatus.Found, result.KnownPlanets[0].Status);
        Assert.AreEqual("Test b", result.Candidates[0].MatchedPlanet);
        Assert.IsLessThanOrEqualTo(1500, result.Periodogram.X.Count);
    }
}
