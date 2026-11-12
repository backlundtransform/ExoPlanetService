using ExoplanetHunter.Transit.Fits;

namespace ExoplanetHunter.Transit;

/// <summary>One TESS sector of a SPOC light curve: PDCSAP flux, with quality flags.</summary>
public sealed record SectorLightCurve(int Sector, double[] Time, double[] Flux, double[] FluxError, int[] Quality)
{
    /// <summary>Reads TIME, PDCSAP_FLUX, PDCSAP_FLUX_ERR and QUALITY from a SPOC light-curve file.</summary>
    public static SectorLightCurve FromFits(byte[] fits, int sector)
    {
        var table = FitsBinaryTable.Read(fits);
        return new SectorLightCurve(
            sector,
            table.Column("TIME"),
            table.Column("PDCSAP_FLUX"),
            table.Column("PDCSAP_FLUX_ERR"),
            table.Column("QUALITY").Select(q => (int)q).ToArray());
    }

    /// <summary>
    /// Good points only (quality 0, finite values), with flux and error divided by the
    /// sector's median flux so sectors can be joined.
    /// </summary>
    public (double[] Time, double[] Flux, double[] FluxError) Normalized()
    {
        var good = Enumerable.Range(0, Time.Length)
            .Where(i => Quality[i] == 0 && double.IsFinite(Time[i]) && double.IsFinite(Flux[i]) && Flux[i] > 0)
            .ToArray();
        if (good.Length == 0)
            return ([], [], []);

        double median = Median(good.Select(i => Flux[i]));
        return (
            good.Select(i => Time[i]).ToArray(),
            good.Select(i => Flux[i] / median).ToArray(),
            good.Select(i => double.IsFinite(FluxError[i]) ? FluxError[i] / median : 0).ToArray());
    }

    internal static double Median(IEnumerable<double> values)
    {
        var sorted = values.Order().ToArray();
        int mid = sorted.Length / 2;
        return sorted.Length % 2 == 1 ? sorted[mid] : (sorted[mid - 1] + sorted[mid]) / 2;
    }
}
