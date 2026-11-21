using System.Text;
using CSharpNumerics.Engines.Exoplanet.Features;

namespace ExoplanetHunter.Transit.Vetting;

/// <summary>Where a training example came from.</summary>
public enum SampleSource : byte
{
    /// <summary>A TOI with a TFOPWG disposition, at its catalogue ephemeris.</summary>
    Toi = 0,

    /// <summary>A synthetic limb-darkened planet transit injected into the star's light curve.</summary>
    InjectedPlanet = 1,

    /// <summary>A synthetic V-shaped eclipsing binary injected into the star's light curve.</summary>
    EclipsingBinary = 2,

    /// <summary>The strongest BLS signal in the inverted light curve.</summary>
    Inverted = 3,

    /// <summary>The strongest BLS signal in the light curve with 2-day chunks shuffled.</summary>
    Scrambled = 4,
}

/// <summary>One training example: the views and features of one transit-like signal, with its label.</summary>
/// <param name="Id">"TOI-1031.01", or "TIC 304021498 binary" for a synthetic example.</param>
/// <param name="Label">1 = planet, 0 = not a planet, −1 = unknown.</param>
public sealed record VettingSample(
    string Id,
    long TicId,
    int Label,
    SampleSource Source,
    float PeriodDays,
    float[] Global,
    float[] Local,
    float[] LocalOdd,
    float[] LocalEven,
    float[] Secondary,
    float[] Features);

/// <summary>What the samples in a dataset file were built with; files can only be merged when these match.</summary>
public sealed record VettingDatasetHeader(int FeatureSchemaVersion, int GlobalBins, int LocalBins,
    double LocalHalfWidthDurations, string[] FeatureNames)
{
    public static VettingDatasetHeader Current(TransitViewConfig views) => new(
        VettingFeatures.SchemaVersion, views.GlobalBins, views.LocalBins, views.LocalHalfWidthDurations,
        [.. VettingFeatures.Names]);

    public bool SameLayout(VettingDatasetHeader other) =>
        FeatureSchemaVersion == other.FeatureSchemaVersion && GlobalBins == other.GlobalBins
        && LocalBins == other.LocalBins && LocalHalfWidthDurations == other.LocalHalfWidthDurations
        && FeatureNames.SequenceEqual(other.FeatureNames);
}

/// <summary>
/// Compact binary file of vetting samples (about 2 KB each): a header describing the layout,
/// then the samples with views and features as 32-bit floats.
/// </summary>
public static class VettingDataset
{
    private const int Magic = 0x44565845; // "EXVD"
    private const int FormatVersion = 1;

    public static void Write(Stream stream, VettingDatasetHeader header, IReadOnlyCollection<VettingSample> samples)
    {
        using var w = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);
        w.Write(Magic);
        w.Write(FormatVersion);
        w.Write(header.FeatureSchemaVersion);
        w.Write(header.GlobalBins);
        w.Write(header.LocalBins);
        w.Write(header.LocalHalfWidthDurations);
        w.Write(header.FeatureNames.Length);
        foreach (string name in header.FeatureNames) w.Write(name);

        w.Write(samples.Count);
        foreach (var s in samples)
        {
            if (s.Global.Length != header.GlobalBins
                || s.Local.Length != header.LocalBins || s.LocalOdd.Length != header.LocalBins
                || s.LocalEven.Length != header.LocalBins || s.Secondary.Length != header.LocalBins
                || s.Features.Length != header.FeatureNames.Length)
                throw new ArgumentException($"Sample {s.Id} does not match the dataset layout.", nameof(samples));

            w.Write(s.Id);
            w.Write(s.TicId);
            w.Write((sbyte)s.Label);
            w.Write((byte)s.Source);
            w.Write(s.PeriodDays);
            foreach (var array in new[] { s.Global, s.Local, s.LocalOdd, s.LocalEven, s.Secondary, s.Features })
                foreach (float v in array) w.Write(v);
        }
    }

    public static (VettingDatasetHeader Header, List<VettingSample> Samples) Read(Stream stream)
    {
        using var r = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);
        if (r.ReadInt32() != Magic) throw new InvalidDataException("Not a vetting dataset file.");
        int version = r.ReadInt32();
        if (version != FormatVersion) throw new InvalidDataException($"Unsupported dataset format version {version}.");

        int schema = r.ReadInt32();
        int globalBins = r.ReadInt32();
        int localBins = r.ReadInt32();
        double halfWidth = r.ReadDouble();
        var names = new string[r.ReadInt32()];
        for (int i = 0; i < names.Length; i++) names[i] = r.ReadString();
        var header = new VettingDatasetHeader(schema, globalBins, localBins, halfWidth, names);

        int count = r.ReadInt32();
        var samples = new List<VettingSample>(count);
        for (int i = 0; i < count; i++)
        {
            string id = r.ReadString();
            long tic = r.ReadInt64();
            int label = r.ReadSByte();
            var source = (SampleSource)r.ReadByte();
            float period = r.ReadSingle();
            samples.Add(new VettingSample(id, tic, label, source, period,
                Floats(r, globalBins), Floats(r, localBins), Floats(r, localBins), Floats(r, localBins),
                Floats(r, localBins), Floats(r, names.Length)));
        }

        return (header, samples);
    }

    public static void WriteFile(string path, VettingDatasetHeader header, IReadOnlyCollection<VettingSample> samples)
    {
        if (Path.GetDirectoryName(Path.GetFullPath(path)) is { } dir) Directory.CreateDirectory(dir);
        string temp = path + ".part";
        using (var stream = File.Create(temp)) Write(stream, header, samples);
        File.Move(temp, path, overwrite: true);
    }

    public static (VettingDatasetHeader Header, List<VettingSample> Samples) ReadFile(string path)
    {
        using var stream = File.OpenRead(path);
        return Read(stream);
    }

    private static float[] Floats(BinaryReader r, int count)
    {
        var values = new float[count];
        for (int i = 0; i < count; i++) values[i] = r.ReadSingle();
        return values;
    }
}
