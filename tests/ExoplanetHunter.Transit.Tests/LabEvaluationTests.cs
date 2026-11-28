using ExoplanetHunter.Transit.Vetting;

namespace ExoplanetHunter.Transit.Tests;

[TestClass]
public sealed class LabEvaluationTests
{
    private static TransitCandidateResult Candidate(double period, double? score, string? planet = null, string? secondaryOf = null, int? transits = 5) => new()
    {
        PeriodDays = period, EpochBtjd = 0, DepthPpm = 1000, DurationHours = 2, RadiusRatio = 0.03,
        Snr = 20, OddEvenDepthRatio = 1, Score = 0.9, Disposition = "Candidate", Warnings = [],
        Folded = new TimeSeries([], []), VettingScore = score, TransitsObserved = transits,
        MatchedPlanet = planet, SecondaryEclipseOf = secondaryOf,
    };

    private static TransitResult Result(string star, string? model, params TransitCandidateResult[] candidates) => new()
    {
        StarId = 1, StarName = star, TicId = 1, Mission = "TESS", Sectors = [1], Points = 1,
        GeneratedAt = DateTimeOffset.UnixEpoch, AnalysisVersion = TransitAnalyzer.Version, VettingModel = model,
        LightCurve = new TimeSeries([], []), Periodogram = new TimeSeries([], []), Candidates = candidates, KnownPlanets = [],
    };

    [TestMethod]
    public void Evaluate_SplitsPlanetsSecondariesAndUnmatched()
    {
        var results = new[]
        {
            Result("A", "vetter-2",
                Candidate(1.0, 0.9, planet: "A b"),
                Candidate(1.0, 0.1, secondaryOf: "A b"),
                Candidate(2.3, 0.6)),
            Result("B", "vetter-2",
                Candidate(3.0, 0.8, planet: "B b"),
                Candidate(5.1, 0.2),
                Candidate(9.0, null, transits: 2)),
            Result("C", null, Candidate(4.0, 0.99)), // not scored by a model: ignored
        };

        var e = LabEvaluation.Evaluate(results);

        CollectionAssert.AreEqual(new[] { "vetter-2" }, e.Models.ToArray());
        Assert.AreEqual(2, e.Planets);
        Assert.AreEqual(1, e.SecondaryEclipses);
        Assert.AreEqual(2, e.Unmatched);
        Assert.AreEqual(1, e.Unscored);
        Assert.AreEqual(0.85, e.PlanetMean!.Value, 1e-9);
        Assert.AreEqual(0.4, e.UnmatchedMean!.Value, 1e-9);
        Assert.AreEqual(1, e.UnmatchedAboveHalf);
        Assert.AreEqual(1.0, e.RocAuc!.Value, 1e-9, "both planets above every negative");
        Assert.AreEqual("A", e.HighestUnmatched[0].Star);

        string markdown = e.ToMarkdown();
        StringAssert.Contains(markdown, "vetter-2");
        StringAssert.Contains(markdown, "1 of 2");
        StringAssert.Contains(markdown, "A 2.30 d (5 transits) 0.60");
    }

    [TestMethod]
    public void Auc_CountsTiesAsHalf()
    {
        Assert.AreEqual(0.5, LabEvaluation.Auc([0.5], [0.5]), 1e-12);
        Assert.AreEqual(0.75, LabEvaluation.Auc([0.9, 0.5], [0.5]), 1e-12, "one win, one tie");
    }

    [TestMethod]
    public void Evaluate_WithoutScores_SaysSo()
    {
        var e = LabEvaluation.Evaluate([Result("A", null, Candidate(1.0, null))]);

        Assert.IsNull(e.RocAuc);
        StringAssert.Contains(e.ToMarkdown(), "No scored candidates");
    }
}
