using System.Reflection;
using System.Text.Json;

namespace ExoplanetHunter.Ingest.Classification;

/// <summary>
/// Finds the IAU constellation containing a position, using the boundary table of
/// Roman (1987, PASP 99, 695). The boundaries are defined for equinox B1875, so J2000
/// coordinates are precessed to 1875 before the lookup.
/// </summary>
public sealed class ConstellationLocator
{
    // Rows: RA lower bound (h), RA upper bound (h), Dec lower bound (deg), abbreviation.
    // Ordered so the first row that contains a point is its constellation.
    private readonly (double RaLow, double RaHigh, double DecLow, string Name)[] _boundaries;

    private ConstellationLocator((double, double, double, string)[] boundaries) => _boundaries = boundaries;

    public static ConstellationLocator Default { get; } = Load();

    /// <param name="raDegrees">J2000 right ascension in degrees.</param>
    /// <param name="decDegrees">J2000 declination in degrees.</param>
    public string Locate(double raDegrees, double decDegrees)
    {
        var (ra, dec) = Precess(raDegrees, decDegrees, fromEpoch: 2000.0, toEpoch: 1875.0);
        double raHours = ra / 15.0;

        foreach (var (raLow, raHigh, decLow, name) in _boundaries)
        {
            if (dec >= decLow && raHours >= raLow && raHours < raHigh)
                return name;
        }

        throw new InvalidOperationException($"No constellation found for RA {raDegrees}°, Dec {decDegrees}°.");
    }

    /// <summary>
    /// Rigorous precession of equatorial coordinates between two Besselian/Julian epochs
    /// (Lieske 1977 angles as given in the Roman 1987 reference program). Degrees in, degrees out.
    /// </summary>
    public static (double RaDegrees, double DecDegrees) Precess(double raDegrees, double decDegrees, double fromEpoch, double toEpoch)
    {
        const double deg = Math.PI / 180.0;
        const double arcsec = deg / 3600.0;

        double ra1 = raDegrees * deg, dec1 = decDegrees * deg;
        double cosDec = Math.Cos(dec1);
        double[] x1 = [cosDec * Math.Cos(ra1), cosDec * Math.Sin(ra1), Math.Sin(dec1)];

        double t = 0.001 * (toEpoch - fromEpoch);
        double st = 0.001 * (fromEpoch - 1900.0);
        double a = arcsec * t * (23042.53 + st * (139.75 + 0.06 * st) + t * (30.23 - 0.27 * st + 18.0 * t));
        double b = arcsec * t * t * (79.27 + 0.66 * st + 0.32 * t) + a;
        double c = arcsec * t * (20046.85 - st * (85.33 + 0.37 * st) + t * (-42.67 - 0.37 * st - 41.8 * t));

        double sa = Math.Sin(a), sb = Math.Sin(b), sc = Math.Sin(c);
        double ca = Math.Cos(a), cb = Math.Cos(b), cc = Math.Cos(c);
        double[,] r =
        {
            { ca * cb * cc - sa * sb, -ca * sb - sa * cb * cc, -cb * sc },
            { sa * cb + ca * sb * cc, ca * cb - sa * sb * cc, -sb * sc },
            { ca * sc, -sa * sc, cc },
        };

        var x2 = new double[3];
        for (int i = 0; i < 3; i++)
            x2[i] = r[i, 0] * x1[0] + r[i, 1] * x1[1] + r[i, 2] * x1[2];

        double ra2 = Math.Atan2(x2[1], x2[0]);
        if (ra2 < 0)
            ra2 += 2 * Math.PI;

        return (ra2 / deg, Math.Asin(x2[2]) / deg);
    }

    private static ConstellationLocator Load()
    {
        using var stream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("ExoplanetHunter.Ingest.Classification.constellation-boundaries.json")
            ?? throw new InvalidOperationException("Constellation boundary table is not embedded.");

        var rows = JsonSerializer.Deserialize<JsonElement[][]>(stream)!;
        return new ConstellationLocator(rows
            .Select(r => (r[0].GetDouble(), r[1].GetDouble(), r[2].GetDouble(), r[3].GetString()!))
            .ToArray());
    }
}
