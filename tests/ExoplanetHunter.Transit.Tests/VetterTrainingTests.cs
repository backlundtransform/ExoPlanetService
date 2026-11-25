using CSharpNumerics.Engines.Exoplanet.Features;
using CSharpNumerics.Engines.Exoplanet.Vetting;
using ExoplanetHunter.Transit.Vetting;

namespace ExoplanetHunter.Transit.Tests;

[TestClass]
public sealed class VetterTrainingTests
{
    private static readonly VettingDatasetHeader Header = new VettingSampleBuilder().Header;

    private static readonly VetterTrainingOptions Small = new()
    {
        Config = new VetterConfig { NumTrees = 30, MaxDepth = 6, MinSamplesLeaf = 3 },
        Folds = 4,
        Name = "test-vetter",
    };

    /// <summary>
    /// Fake dataset: 120 stars, each with one TOI and one example of every synthetic source.
    /// Planets have the first three features raised; views are flat.
    /// </summary>
    private static List<VettingSample> Samples(int stars = 120, int seed = 1)
    {
        var rng = new Random(seed);
        var samples = new List<VettingSample>();
        for (int star = 0; star < stars; star++)
        {
            long tic = 1_000_000 + star;
            int toiLabel = rng.NextDouble() < 0.55 ? 1 : 0;
            samples.Add(Sample($"TOI-{star}.01", tic, toiLabel, SampleSource.Toi, rng));
            samples.Add(Sample($"TIC {tic} injected", tic, 1, SampleSource.InjectedPlanet, rng));
            samples.Add(Sample($"TIC {tic} binary", tic, 0, SampleSource.EclipsingBinary, rng));
            samples.Add(Sample($"TIC {tic} inverted", tic, 0, SampleSource.Inverted, rng));
            samples.Add(Sample($"TIC {tic} scrambled", tic, 0, SampleSource.Scrambled, rng));
        }
        return samples;
    }

    private static VettingSample Sample(string id, long tic, int label, SampleSource source, Random rng)
    {
        var features = new float[Header.FeatureNames.Length];
        for (int j = 0; j < features.Length; j++)
            features[j] = (float)(rng.NextDouble() * 2 - 1 + (j < 3 ? 1.5 * label : 0));
        return new VettingSample(id, tic, label, source, 3.0f,
            new float[Header.GlobalBins], new float[Header.LocalBins], new float[Header.LocalBins],
            new float[Header.LocalBins], new float[Header.LocalBins], features);
    }

    [TestMethod]
    public void Train_ByDefault_UsesOnlyRealToisAndScoresThem()
    {
        var samples = Samples();

        var result = VetterTraining.Train(Header, samples, Small);

        Assert.IsTrue(result.Samples.All(s => s.Source == SampleSource.Toi));
        Assert.HasCount(120, result.Examples);
        Assert.AreEqual(120, result.Evaluation.EvaluatedCount);
        Assert.IsGreaterThan(0.85, result.Evaluation.RocAuc, $"AUC {result.Evaluation.RocAuc:F3}");
        Assert.AreEqual("test-vetter", result.Model.Name);
        Assert.AreEqual(VettingFeatures.SchemaVersion, result.Model.FeatureSchemaVersion);
    }

    [TestMethod]
    public void Train_WithSyntheticSources_WeighsThemAndEvaluatesOnlyTois()
    {
        var options = Small with
        {
            Sources = [SampleSource.Toi, SampleSource.EclipsingBinary, SampleSource.Inverted],
            SyntheticWeight = 0.25,
        };

        var result = VetterTraining.Train(Header, Samples(), options);

        Assert.HasCount(360, result.Examples);
        Assert.AreEqual(120, result.Evaluation.EvaluatedCount, "only real TOIs are measured");
        Assert.IsTrue(result.Examples.Where(e => e.Synthetic).All(e => e.Weight == 0.25));
        Assert.IsTrue(result.Examples.Where(e => !e.Synthetic).All(e => e.Weight == 1.0));
    }

    [TestMethod]
    public void Train_WithoutTois_Throws()
    {
        var options = Small with { Sources = [SampleSource.InjectedPlanet, SampleSource.EclipsingBinary] };
        Assert.ThrowsExactly<ArgumentException>(() => VetterTraining.Train(Header, Samples(), options));
    }

    [TestMethod]
    public void Train_DatasetWithAnotherFeatureSchema_IsRefused()
    {
        var old = Header with { FeatureSchemaVersion = VettingFeatures.SchemaVersion + 1 };
        Assert.ThrowsExactly<InvalidDataException>(() => VetterTraining.Train(old, Samples(), Small));
    }

    [TestMethod]
    public void SavedModel_LoadsBackToTheSameScores()
    {
        var result = VetterTraining.Train(Header, Samples(), Small);

        var loaded = CandidateVetter.Deserialize(result.Model.Serialize());

        var probe = result.Examples.Take(20).Select(e => e.Features).ToArray();
        CollectionAssert.AreEqual(result.Model.Probabilities(probe), loaded.Probabilities(probe));
    }

    [TestMethod]
    public void ReportAndCsv_DescribeTheModel()
    {
        var result = VetterTraining.Train(Header, Samples(), Small);

        string report = VetterTraining.Report(result, Small, "dataset-test", modelBytes: 4096);
        var csv = VetterTraining.OutOfFoldCsv(result).ToList();

        StringAssert.Contains(report, "**test-vetter**");
        StringAssert.Contains(report, "`dataset-test`");
        StringAssert.Contains(report, "| ROC-AUC |");
        StringAssert.Contains(report, "**120 real TOIs**");
        StringAssert.Contains(report, "Model file: 4 KB");
        Assert.AreEqual("id,tic,label,source,fold,probability", csv[0]);
        Assert.HasCount(121, csv);
        StringAssert.StartsWith(csv[1], "TOI-0.01,1000000,");
    }
}
