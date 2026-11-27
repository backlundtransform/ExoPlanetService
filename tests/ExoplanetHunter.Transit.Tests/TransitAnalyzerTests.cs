using CSharpNumerics.Engines.Exoplanet.Features;
using CSharpNumerics.Engines.Exoplanet.Vetting;
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
    public void HasSignal_CandidateWithoutDepthOrSnr_IsDropped()
    {
        Assert.IsTrue(TransitAnalyzer.HasSignal(Candidate(3)));
        // Seen in the weekly run (GJ 1252, K2-138): depth and SNR 0, NaN mapped to 0 in the result.
        Assert.IsFalse(TransitAnalyzer.HasSignal(Candidate(0.312) with { DepthPpm = 0, Snr = 0 }));
        Assert.IsFalse(TransitAnalyzer.HasSignal(Candidate(3) with { Snr = 0 }));
        Assert.IsFalse(TransitAnalyzer.HasSignal(Candidate(3) with { DepthPpm = -50 }));
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

    /// <summary>27 days of 2-minute data with a 1% deep, 2-hour transit every <paramref name="period"/> days.</summary>
    private static SectorLightCurve SyntheticSector(double period = 3.2)
    {
        const double duration = 2.0 / 24, depth = 0.01;
        var rng = new Random(42);
        int n = (int)(27 * 720);
        var time = Enumerable.Range(0, n).Select(i => 1000 + i / 720.0).ToArray();
        var flux = time.Select(t =>
        {
            double phase = ((t - 1001) % period + period) % period;
            bool inTransit = phase < duration / 2 || phase > period - duration / 2;
            return (float)((inTransit ? 1 - depth : 1) * 10_000 + rng.NextDouble() * 20 - 10);
        }).ToArray();
        return SectorLightCurve.FromFits(FitsBinaryTableTests.BuildFits(time, flux, time.Select(_ => 6f).ToArray(), new int[n]), 1);
    }

    [TestMethod]
    public void Analyze_WithAVettingModel_ScoresEveryCandidate()
    {
        // A small model on random features: the point is that scores are computed and stored.
        var rng = new Random(5);
        var examples = Enumerable.Range(0, 60).Select(i => new VettingExample(
            $"x{i}", i, i % 2, Enumerable.Range(0, VettingFeatures.Names.Length).Select(_ => rng.NextDouble() + i % 2).ToArray()))
            .ToList();
        var vetter = CandidateVetter.Train(examples, new VetterConfig { NumTrees = 10 }, "test-vetter");
        var star = new Star { Id = 9, Name = "Test", Radius = 1.0, Mass = 1.0, Teff = 5772 };

        var scored = new TransitAnalyzer(new TransitAnalysisOptions { TrialPeriods = 8000 }, vetter)
            .Analyze(star, [Planet("Test b", 3.2)], 1, [SyntheticSector()], DateTimeOffset.UnixEpoch);
        var unscored = new TransitAnalyzer(new TransitAnalysisOptions { TrialPeriods = 8000 })
            .Analyze(star, [Planet("Test b", 3.2)], 1, [SyntheticSector()], DateTimeOffset.UnixEpoch);

        Assert.AreEqual("test-vetter", scored.VettingModel);
        Assert.IsNotEmpty(scored.Candidates);
        Assert.IsTrue(scored.Candidates.All(c => c.VettingScore is >= 0 and <= 1));
        Assert.IsNull(unscored.VettingModel);
        Assert.IsTrue(unscored.Candidates.All(c => c.VettingScore is null && c.TransitsObserved is null));

        // The planet: 27 days at a 3.2-day period gives 8 or 9 transits.
        Assert.IsTrue(scored.Candidates[0].TransitsObserved is >= 8 and <= 9, $"{scored.Candidates[0].TransitsObserved} transits");

        var tooFew = new TransitAnalyzer(new TransitAnalysisOptions { TrialPeriods = 8000, MinTransitsToScore = 20 }, vetter)
            .Analyze(star, [Planet("Test b", 3.2)], 1, [SyntheticSector()], DateTimeOffset.UnixEpoch);
        Assert.IsTrue(tooFew.Candidates.All(c => c.VettingScore is null && c.TransitsObserved is not null));
        Assert.AreEqual("test-vetter", tooFew.VettingModel);
    }

    [TestMethod]
    public void Analyze_SyntheticTransits_FindsThePeriod()
    {
        const double period = 3.2;
        var star = new Star { Id = 9, Name = "Test" };
        var result = new TransitAnalyzer(new TransitAnalysisOptions { TrialPeriods = 8000 })
            .Analyze(star, [Planet("Test b", period)], 1, [SyntheticSector(period)], DateTimeOffset.UnixEpoch);

        Assert.IsNotEmpty(result.Candidates);
        Assert.AreEqual(period, result.Candidates[0].PeriodDays, 0.02);
        Assert.AreEqual(MatchStatus.Found, result.KnownPlanets[0].Status);
        Assert.AreEqual("Test b", result.Candidates[0].MatchedPlanet);
        Assert.IsLessThanOrEqualTo(1500, result.Periodogram.X.Count);
    }
}
