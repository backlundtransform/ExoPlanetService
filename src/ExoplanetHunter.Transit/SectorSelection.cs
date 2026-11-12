using ExoplanetHunter.Transit.Mast;

namespace ExoplanetHunter.Transit;

public static class SectorSelection
{
    /// <summary>
    /// The longest run of consecutive sectors, at most <paramref name="maxSectors"/> long, preferring
    /// the most recent run on ties. Sectors years apart would make the linear BLS grid far too
    /// coarse, so only contiguous data is searched.
    /// </summary>
    public static IReadOnlyList<TessLightCurveFile> Choose(IReadOnlyList<TessLightCurveFile> files, int maxSectors = 3)
    {
        var sorted = files.OrderBy(f => f.Sector).ToArray();
        var best = Array.Empty<TessLightCurveFile>();

        int start = 0;
        while (start < sorted.Length)
        {
            int end = start + 1;
            while (end < sorted.Length && sorted[end].Sector == sorted[end - 1].Sector + 1)
                end++;

            var run = sorted[start..end];
            // Within a long run, keep its most recent part.
            if (run.Length > maxSectors)
                run = run[^maxSectors..];
            if (run.Length >= best.Length)
                best = run;

            start = end;
        }

        return best;
    }
}
