using System.Globalization;

namespace ExoplanetHunter.Ingest.Nasa;

/// <summary>Downloads confirmed planets from the NASA Exoplanet Archive TAP service.</summary>
public sealed class NasaArchiveClient(HttpClient http)
{
    public static readonly Uri TapEndpoint = new("https://exoplanetarchive.ipac.caltech.edu/TAP/sync");

    private const string Columns =
        "pl_name,hostname,hd_name,hip_name,pl_rade,pl_bmasse,pl_insol,pl_orbsmax,pl_orbeccen,pl_orbper,pl_orbincl,pl_orblper," +
        "st_spectype,st_teff,st_rad,st_mass,st_lum,st_met,st_age,sy_dist,sy_vmag,ra,dec,discoverymethod,disc_year";

    public async Task<IReadOnlyList<NasaPlanetRow>> GetConfirmedPlanetsAsync(CancellationToken cancellationToken = default)
    {
        var query = Uri.EscapeDataString($"select {Columns} from pscomppars");
        using var response = await http.GetAsync(new Uri(TapEndpoint, $"?query={query}&format=csv"), HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);
        return Parse(reader);
    }

    public static IReadOnlyList<NasaPlanetRow> Parse(TextReader reader) =>
        Csv.ReadRecords(reader).Select(r => new NasaPlanetRow
        {
            PlanetName = r["pl_name"],
            HostName = r["hostname"],
            HdName = Text(r["hd_name"]),
            HipName = Text(r["hip_name"]),
            PlanetRadius = Number(r["pl_rade"]),
            PlanetMass = Number(r["pl_bmasse"]),
            Insolation = Number(r["pl_insol"]),
            SemiMajorAxis = Number(r["pl_orbsmax"]),
            Eccentricity = Number(r["pl_orbeccen"]),
            Period = Number(r["pl_orbper"]),
            Inclination = Number(r["pl_orbincl"]),
            ArgumentOfPeriastron = Number(r["pl_orblper"]),
            SpectralType = Text(r["st_spectype"]),
            StellarTeff = Number(r["st_teff"]),
            StellarRadius = Number(r["st_rad"]),
            StellarMass = Number(r["st_mass"]),
            StellarLogLuminosity = Number(r["st_lum"]),
            StellarMetallicity = Number(r["st_met"]),
            StellarAge = Number(r["st_age"]),
            Distance = Number(r["sy_dist"]),
            VMagnitude = Number(r["sy_vmag"]),
            RaDegrees = Number(r["ra"]),
            DecDegrees = Number(r["dec"]),
            DiscoveryMethod = Text(r["discoverymethod"]),
            DiscoveryYear = Number(r["disc_year"]) is { } year ? (int)year : null,
        }).ToArray();

    private static string? Text(string value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static double? Number(string value) =>
        double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var d) && double.IsFinite(d) ? d : null;
}
