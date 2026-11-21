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

    /// <summary>
    /// Up to <paramref name="maxSectors"/> consecutive sectors as close as possible to
    /// <paramref name="targetSector"/>: the sectors around a TOI's epoch, where its catalogue
    /// ephemeris is most accurate. Ties go to the longer window, then to the window whose middle
    /// is closest to the target, then to the earlier one.
    /// </summary>
    public static IReadOnlyList<TessLightCurveFile> ChooseNearest(IReadOnlyList<TessLightCurveFile> files,
        int targetSector, int maxSectors = 3)
    {
        var sorted = files.OrderBy(f => f.Sector).ToArray();
        var best = Array.Empty<TessLightCurveFile>();
        int bestDistance = int.MaxValue;
        double bestCentre = double.MaxValue;

        int start = 0;
        while (start < sorted.Length)
        {
            int end = start + 1;
            while (end < sorted.Length && sorted[end].Sector == sorted[end - 1].Sector + 1)
                end++;

            // Every window of at most maxSectors inside the run.
            int length = Math.Min(maxSectors, end - start);
            for (int s = start; s + length <= end; s++)
            {
                var window = sorted[s..(s + length)];
                int distance = window.Min(f => Math.Abs(f.Sector - targetSector));
                double centre = Math.Abs((window[0].Sector + window[^1].Sector) / 2.0 - targetSector);
                bool better = distance < bestDistance
                    || (distance == bestDistance && window.Length > best.Length)
                    || (distance == bestDistance && window.Length == best.Length && centre < bestCentre);
                if (better)
                {
                    best = window;
                    bestDistance = distance;
                    bestCentre = centre;
                }
            }

            start = end;
        }

        return best;
    }

    /// <summary>
    /// The TESS sector observing at <paramref name="btjd"/>, approximately (sectors are about
    /// 27.4 days long from sector 1 at BTJD 1325.3). Good enough to pick the nearest data.
    /// </summary>
    public static int ApproximateSector(double btjd) => (int)Math.Floor((btjd - 1325.3) / 27.4) + 1;
}
