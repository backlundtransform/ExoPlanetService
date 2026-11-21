using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ExoplanetHunter.Transit.Vetting;

/// <summary>One TESS Object of Interest from the NASA Exoplanet Archive <c>toi</c> table.</summary>
/// <param name="Toi">TOI number with planet suffix, e.g. "1031.01".</param>
/// <param name="TicId">TESS Input Catalog id of the host star.</param>
/// <param name="Disposition">TFOPWG disposition: CP, KP, PC, APC, FP, FA, or empty.</param>
/// <param name="EpochBjd">Mid-transit time in BJD (TDB).</param>
/// <param name="DepthPpm">Transit depth in ppm.</param>
public sealed record ToiEntry(
    string Toi,
    long TicId,
    string Disposition,
    double PeriodDays,
    double EpochBjd,
    double DurationHours,
    double DepthPpm,
    double? StarRadius,
    double? StarTeff,
    double? StarLogg,
    double? PeriodErrorDays = null,
    double? EpochErrorDays = null)
{
    /// <summary>TESS light curves use BTJD = BJD − 2457000.</summary>
    public double EpochBtjd => EpochBjd - 2457000.0;

    public double DurationDays => DurationHours / 24.0;

    /// <summary>1 = planet (CP confirmed, KP known), 0 = not a planet (FP false positive, FA false alarm), null = undecided.</summary>
    public int? Label => Disposition switch
    {
        "CP" or "KP" => 1,
        "FP" or "FA" => 0,
        _ => null,
    };

    /// <summary>Period, epoch and duration are all usable.</summary>
    public bool HasEphemeris =>
        PeriodDays > 0 && DurationHours > 0 && DurationDays < PeriodDays && double.IsFinite(EpochBjd) && EpochBjd > 2_400_000;

    /// <summary>
    /// How far (days, 1σ) the predicted transit times may have drifted at <paramref name="btjd"/>:
    /// the epoch error plus the period error times the number of orbits since the epoch. Unknown
    /// errors count as 0.
    /// </summary>
    public double PredictionUncertaintyDays(double btjd) =>
        (EpochErrorDays ?? 0) + (PeriodErrorDays ?? 0) * Math.Abs(btjd - EpochBtjd) / PeriodDays;
}

/// <summary>Reads the TOI table from the NASA Exoplanet Archive TAP service.</summary>
public sealed class ToiClient(HttpClient http)
{
    private static readonly Uri TapEndpoint = new("https://exoplanetarchive.ipac.caltech.edu/TAP/sync");

    private const string Query =
        "select toi,tid,tfopwg_disp,pl_orbper,pl_orbpererr1,pl_tranmid,pl_tranmiderr1,pl_trandurh,pl_trandep,st_rad,st_teff,st_logg from toi";

    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    /// <summary>Every TOI in the archive (about 8,000 rows).</summary>
    public async Task<IReadOnlyList<ToiEntry>> FetchAsync(CancellationToken cancellationToken = default)
    {
        var rows = await http.GetFromJsonAsync<JsonArray>(
            new Uri(TapEndpoint, $"?query={Uri.EscapeDataString(Query)}&format=json"), cancellationToken) ?? [];
        return Parse(rows);
    }

    /// <summary>
    /// The TOIs saved in <paramref name="path"/>, or fetched and saved there when the file does
    /// not exist. A run that is split into shards fetches once, so every shard sees the same table.
    /// </summary>
    public async Task<IReadOnlyList<ToiEntry>> LoadOrFetchAsync(string path, CancellationToken cancellationToken = default)
    {
        if (File.Exists(path))
            return JsonSerializer.Deserialize<ToiEntry[]>(await File.ReadAllTextAsync(path, cancellationToken), Json)
                ?? throw new InvalidDataException($"{path} holds no TOIs.");

        var entries = await FetchAsync(cancellationToken);
        if (Path.GetDirectoryName(Path.GetFullPath(path)) is { } dir) Directory.CreateDirectory(dir);
        await File.WriteAllTextAsync(path, JsonSerializer.Serialize(entries, Json), cancellationToken);
        return entries;
    }

    internal static IReadOnlyList<ToiEntry> Parse(JsonArray rows)
    {
        var entries = new List<ToiEntry>();
        foreach (var row in rows.OfType<JsonObject>())
        {
            string? toi = Text(row["toi"]);
            long? tic = Number(row["tid"]) is { } t ? (long)t : null;
            if (toi is null || tic is null) continue;

            entries.Add(new ToiEntry(
                toi,
                tic.Value,
                Text(row["tfopwg_disp"]) ?? "",
                Number(row["pl_orbper"]) ?? double.NaN,
                Number(row["pl_tranmid"]) ?? double.NaN,
                Number(row["pl_trandurh"]) ?? double.NaN,
                Number(row["pl_trandep"]) ?? double.NaN,
                Number(row["st_rad"]),
                Number(row["st_teff"]),
                Number(row["st_logg"]),
                Number(row["pl_orbpererr1"]),
                Number(row["pl_tranmiderr1"])));
        }

        return entries;
    }

    private static string? Text(JsonNode? node) => node is JsonValue value
        ? value.TryGetValue(out string? s) ? s?.Trim()
          : value.TryGetValue(out double d) ? d.ToString(CultureInfo.InvariantCulture)
          : null
        : null;

    private static double? Number(JsonNode? node)
    {
        if (node is not JsonValue value) return null;
        if (value.TryGetValue(out double d)) return double.IsFinite(d) ? d : null;
        if (value.TryGetValue(out string? s)
            && double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out d) && double.IsFinite(d))
            return d;
        return null;
    }
}
