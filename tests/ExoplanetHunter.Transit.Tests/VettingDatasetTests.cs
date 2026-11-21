using System.Text.Json.Nodes;
using ExoplanetHunter.Transit.Mast;
using ExoplanetHunter.Transit.Vetting;

namespace ExoplanetHunter.Transit.Tests;

[TestClass]
public sealed class VettingDatasetTests
{
    private const long Tic = 123456789;
    private const double Start = 2000.0; // BTJD of the first synthetic point

    #region TOI table

    [TestMethod]
    public void Parse_ReadsTheArchiveColumns()
    {
        var rows = JsonNode.Parse("""
            [
              {"toi": "1031.01", "tid": 304021498, "tfopwg_disp": "FP", "pl_orbper": 5.652623, "pl_orbpererr1": 0.0001,
               "pl_tranmid": 2459304.837939, "pl_tranmiderr1": 0.002, "pl_trandurh": 3.434, "pl_trandep": 1250.0,
               "st_rad": 1.92, "st_teff": 6415.4, "st_logg": 3.98},
              {"toi": "1032.01", "tid": 146589986, "tfopwg_disp": "PC", "pl_orbper": null, "pl_orbpererr1": null,
               "pl_tranmid": 2459283.828685, "pl_tranmiderr1": null, "pl_trandurh": 3.8, "pl_trandep": 3491.8,
               "st_rad": null, "st_teff": 10395.0, "st_logg": null},
              {"toi": null, "tid": 1}
            ]
            """)!.AsArray();

        var tois = ToiClient.Parse(rows);

        Assert.HasCount(2, tois);
        var fp = tois[0];
        Assert.AreEqual("1031.01", fp.Toi);
        Assert.AreEqual(304021498L, fp.TicId);
        Assert.AreEqual(0, fp.Label);
        Assert.AreEqual(2304.837939, fp.EpochBtjd, 1e-6);
        Assert.AreEqual(3.434 / 24, fp.DurationDays, 1e-12);
        Assert.IsTrue(fp.HasEphemeris);
        Assert.AreEqual(1.92, fp.StarRadius);

        var pc = tois[1];
        Assert.IsNull(pc.Label);
        Assert.IsFalse(pc.HasEphemeris, "no period");
        Assert.IsNull(pc.StarRadius);
    }

    [TestMethod]
    public void SavedTable_KeepsMissingValues()
    {
        // The archive leaves values out (here the period); they are NaN or null in the entry.
        var entry = Toi(disposition: "PC", period: double.NaN) with { StarRadius = null, PeriodErrorDays = 0.0001 };

        var read = ToiClient.Deserialize(ToiClient.Serialize([entry])).Single();

        Assert.IsTrue(double.IsNaN(read.PeriodDays));
        Assert.IsFalse(read.HasEphemeris);
        Assert.IsNull(read.StarRadius);
        Assert.AreEqual(entry with { PeriodDays = 0 }, read with { PeriodDays = 0 }); // NaN != NaN, so compare the rest
    }

    [TestMethod]
    public void Label_MapsDispositions()
    {
        foreach (var (disposition, label) in new (string, int?)[] { ("CP", 1), ("KP", 1), ("FP", 0), ("FA", 0), ("PC", null), ("APC", null), ("", null) })
            Assert.AreEqual(label, Toi(disposition: disposition).Label, disposition);
    }

    [TestMethod]
    public void PredictionUncertainty_GrowsWithOrbitsSinceTheEpoch()
    {
        var toi = Toi(period: 2.0, epochBtjd: 1000.0) with { PeriodErrorDays = 0.001, EpochErrorDays = 0.002 };

        Assert.AreEqual(0.002, toi.PredictionUncertaintyDays(1000.0), 1e-12);
        Assert.AreEqual(0.002 + 0.001 * 500, toi.PredictionUncertaintyDays(2000.0), 1e-12); // 500 orbits later
    }

    #endregion

    #region Sector selection

    private static TessLightCurveFile[] Files(params int[] sectors) =>
        sectors.Select(s => new TessLightCurveFile(s, 120, $"s{s}_lc.fits", $"mast:s{s}")).ToArray();

    private static int[] Sectors(IReadOnlyList<TessLightCurveFile> files) => files.Select(f => f.Sector).ToArray();

    [TestMethod]
    public void ChooseNearest_PicksTheRunAroundTheTarget()
    {
        var files = Files(1, 2, 3, 4, 5, 20, 21, 40);

        CollectionAssert.AreEqual(new[] { 20, 21 }, Sectors(SectorSelection.ChooseNearest(files, 21)));
        CollectionAssert.AreEqual(new[] { 20, 21 }, Sectors(SectorSelection.ChooseNearest(files, 29)));
        CollectionAssert.AreEqual(new[] { 40 }, Sectors(SectorSelection.ChooseNearest(files, 35)));
        CollectionAssert.AreEqual(new[] { 2, 3, 4 }, Sectors(SectorSelection.ChooseNearest(files, 3)), "target in the middle");
        CollectionAssert.AreEqual(new[] { 3, 4, 5 }, Sectors(SectorSelection.ChooseNearest(files, 9)));
        Assert.IsEmpty(SectorSelection.ChooseNearest(Files(), 5));
    }

    [TestMethod]
    public void ApproximateSector_FollowsTheTessSchedule()
    {
        Assert.AreEqual(1, SectorSelection.ApproximateSector(1330.0));
        Assert.AreEqual(11, SectorSelection.ApproximateSector(1325.3 + 27.4 * 10 + 5));
    }

    #endregion

    #region Dataset file

    private static VettingSample Sample(string id, int label, VettingDatasetHeader header, float value) => new(
        id, Tic, label, SampleSource.Toi, 3.5f,
        Enumerable.Repeat(value, header.GlobalBins).ToArray(),
        Enumerable.Repeat(value, header.LocalBins).ToArray(),
        Enumerable.Repeat(value, header.LocalBins).ToArray(),
        Enumerable.Repeat(value, header.LocalBins).ToArray(),
        Enumerable.Repeat(value, header.LocalBins).ToArray(),
        Enumerable.Repeat(value, header.FeatureNames.Length).ToArray());

    [TestMethod]
    public void Dataset_RoundTrips()
    {
        var header = new VettingSampleBuilder().Header;
        var samples = new[] { Sample("TOI-1.01", 1, header, -0.5f), Sample("TIC 9 binary", 0, header, 0.25f) with { Source = SampleSource.EclipsingBinary } };

        using var stream = new MemoryStream();
        VettingDataset.Write(stream, header, samples);
        stream.Position = 0;
        var (readHeader, read) = VettingDataset.Read(stream);

        Assert.IsTrue(header.SameLayout(readHeader));
        Assert.HasCount(2, read);
        Assert.AreEqual("TIC 9 binary", read[1].Id);
        Assert.AreEqual(SampleSource.EclipsingBinary, read[1].Source);
        Assert.AreEqual(0, read[1].Label);
        Assert.AreEqual(Tic, read[0].TicId);
        Assert.AreEqual(3.5f, read[0].PeriodDays);
        CollectionAssert.AreEqual(samples[0].Global, read[0].Global);
        CollectionAssert.AreEqual(samples[1].Features, read[1].Features);
    }

    [TestMethod]
    public void Dataset_SampleWithTheWrongLayout_IsRejected()
    {
        var header = new VettingSampleBuilder().Header;
        var bad = Sample("TOI-1.01", 1, header, 0f) with { Local = new float[3] };

        Assert.ThrowsExactly<ArgumentException>(() => VettingDataset.Write(new MemoryStream(), header, [bad]));
    }

    [TestMethod]
    public void Report_CountsSourcesAndSkipReasons()
    {
        var header = new VettingSampleBuilder().Header;
        var samples = new[] { Sample("TOI-1.01", 1, header, 0f), Sample("TOI-2.01", 0, header, 0f) };
        var skipped = new[]
        {
            new SkippedSample("TOI-3.01", 7, "only 12 good points"),
            new SkippedSample("TOI-4.01", 8, "only 950 good points"),
            new SkippedSample("TIC 8 injected", 8, "The orbit is too small for the star."),
        };

        string report = DatasetReport.Build(samples, skipped, header);

        StringAssert.Contains(report, "| Toi | 1 | 1 | 0 |");
        StringAssert.Contains(report, "| only N good points | 2 | 0 |");
        StringAssert.Contains(report, "| The orbit is too small for the star. | 0 | 1 |");
    }

    #endregion

    #region Building samples

    private static ToiEntry Toi(string toi = "100.01", string disposition = "CP", double period = 3.1,
        double epochBtjd = Start + 1.0, double durationHours = 3.0, double depthPpm = 2000) =>
        new(toi, Tic, disposition, period, epochBtjd + 2457000.0, durationHours, depthPpm, 1.0, 5772, 4.438);

    /// <summary>Two sectors of 10-minute data with box transits of the CP TOI and Gaussian noise.</summary>
    private static SectorLightCurve[] Sectors(ToiEntry planet, double noise = 0.0005)
    {
        var rng = new Random(3);
        return [MakeSector(1, Start, rng), MakeSector(2, Start + 27.5, rng)];

        SectorLightCurve MakeSector(int sector, double start, Random rng)
        {
            int n = (int)(26.0 / 0.007);
            var time = new double[n];
            var flux = new double[n];
            for (int i = 0; i < n; i++)
            {
                time[i] = start + i * 0.007;
                double since = time[i] - planet.EpochBtjd;
                double offset = since - Math.Round(since / planet.PeriodDays) * planet.PeriodDays;
                double gaussian = Math.Sqrt(-2 * Math.Log(1 - rng.NextDouble())) * Math.Cos(2 * Math.PI * rng.NextDouble());
                flux[i] = 5000.0 * ((Math.Abs(offset) <= planet.DurationDays / 2 ? 1 - planet.DepthPpm * 1e-6 : 1.0) + noise * gaussian);
            }
            return new SectorLightCurve(sector, time, flux, Enumerable.Repeat(5000 * noise, n).ToArray(), new int[n]);
        }
    }

    [TestMethod]
    public void Build_MakesToiAndSyntheticExamples()
    {
        var planet = Toi();
        var falsePositive = Toi(toi: "100.02", disposition: "FP", period: 7.3, epochBtjd: Start + 2.2, durationHours: 2.0, depthPpm: 500);
        var undecided = Toi(toi: "100.03", disposition: "PC", period: 11.0, epochBtjd: Start + 4.0);
        var builder = new VettingSampleBuilder();

        var (samples, skipped) = builder.Build(Tic, Sectors(planet), [planet, falsePositive, undecided]);

        var header = builder.Header;
        var toi = samples.Single(s => s.Id == "TOI-100.01");
        Assert.AreEqual(1, toi.Label);
        Assert.AreEqual(SampleSource.Toi, toi.Source);
        Assert.AreEqual(-1.0, toi.Local[header.LocalBins / 2], 0.25, "transit bottom in the local view");
        Assert.AreEqual(0, samples.Single(s => s.Id == "TOI-100.02").Label);
        Assert.IsFalse(samples.Any(s => s.Id == "TOI-100.03"), "undecided TOIs are left out by default");

        Assert.AreEqual(1, samples.Single(s => s.Source == SampleSource.InjectedPlanet).Label);
        Assert.AreEqual(0, samples.Single(s => s.Source == SampleSource.EclipsingBinary).Label);
        foreach (var source in new[] { SampleSource.Inverted, SampleSource.Scrambled })
            Assert.IsTrue(samples.Any(s => s.Source == source && s.Label == 0) || skipped.Any(s => s.Id.EndsWith(source.ToString().ToLowerInvariant())),
                source.ToString());

        foreach (var s in samples)
        {
            Assert.HasCount(header.GlobalBins, s.Global);
            Assert.HasCount(header.FeatureNames.Length, s.Features);
            foreach (var array in new[] { s.Global, s.Local, s.LocalOdd, s.LocalEven, s.Secondary, s.Features })
                Assert.IsTrue(array.All(float.IsFinite), s.Id);
        }
    }

    [TestMethod]
    public void Build_IsRepeatable()
    {
        var planet = Toi();
        var builder = new VettingSampleBuilder();

        var first = builder.Build(Tic, Sectors(planet), [planet]).Samples;
        var second = builder.Build(Tic, Sectors(planet), [planet]).Samples;

        CollectionAssert.AreEqual(first.Select(s => s.Id).ToArray(), second.Select(s => s.Id).ToArray());
        for (int i = 0; i < first.Count; i++)
            CollectionAssert.AreEqual(first[i].Features, second[i].Features, first[i].Id);
    }

    [TestMethod]
    public void Build_UncertainEphemeris_IsSkipped()
    {
        // The catalogue epoch is 1000 days before the data; a 0.01-day period error drifts by ~3 days.
        var planet = Toi(epochBtjd: Start - 1000 + 1.0) with { PeriodErrorDays = 0.01 };
        var builder = new VettingSampleBuilder(new VettingSampleOptions { Synthetic = SyntheticKinds.None });

        var (samples, skipped) = builder.Build(Tic, Sectors(Toi()), [planet]);

        Assert.IsEmpty(samples);
        Assert.AreEqual("ephemeris too uncertain for these sectors", skipped.Single().Reason);
    }

    [TestMethod]
    public void Build_TooFewPoints_SkipsEveryToi()
    {
        var planet = Toi();
        var tiny = new SectorLightCurve(1, [Start, Start + 0.01], [1.0, 1.0], [0.001, 0.001], [0, 0]);

        var (samples, skipped) = new VettingSampleBuilder().Build(Tic, [tiny], [planet]);

        Assert.IsEmpty(samples);
        StringAssert.StartsWith(skipped.Single().Reason, "only 2 good points");
    }

    #endregion
}
