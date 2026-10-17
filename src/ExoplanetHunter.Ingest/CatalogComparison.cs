using System.Text;
using ExoplanetHunter.Domain.Catalog;

namespace ExoplanetHunter.Ingest;

/// <summary>Summarises how a new catalogue differs from the previous one, for review before publishing.</summary>
public static class CatalogComparison
{
    public static string Report(CatalogSnapshot previous, CatalogSnapshot next)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Previous: {previous.Source}, {previous.GeneratedAt:u}");
        sb.AppendLine($"New:      {next.Source}, {next.GeneratedAt:u}");
        sb.AppendLine();

        Counts(sb, "Stars", previous.Stars.Select(s => s.Name), next.Stars.Select(s => s.Name));
        Counts(sb, "Planets", previous.Planets.Select(p => p.Name), next.Planets.Select(p => p.Name));
        sb.AppendLine($"Habitable planets: {previous.Planets.Count(p => p.Habitable == true)} -> {next.Planets.Count(p => p.Habitable == true)}");
        sb.AppendLine();

        var oldPlanets = previous.Planets.ToDictionary(p => p.Name);
        var matched = next.Planets.Where(p => oldPlanets.ContainsKey(p.Name)).Select(p => (Old: oldPlanets[p.Name], New: p)).ToArray();
        var oldStars = previous.Stars.ToDictionary(s => s.Name);
        var matchedStars = next.Stars.Where(s => oldStars.ContainsKey(s.Name)).Select(s => (Old: oldStars[s.Name], New: s)).ToArray();

        sb.AppendLine($"Agreement for {matched.Length} planets and {matchedStars.Length} stars present in both:");
        Same(sb, "planet.massClass", matched, x => x.MassClass);
        Same(sb, "planet.zoneClass", matched, x => x.ZoneClass);
        Same(sb, "planet.habitable", matched, x => x.Habitable == true);
        Close(sb, "planet.esi", matched, x => x.Esi);
        Close(sb, "planet.sFluxMean", matched, x => x.SFluxMean);
        Close(sb, "planet.teqMean", matched, x => x.TeqMean);
        Close(sb, "planet.meanDistance", matched, x => x.MeanDistance);
        Close(sb, "planet.radius", matched, x => x.Radius);
        Same(sb, "star.constellation", matchedStars, x => x.Constellation);
        Close(sb, "star.luminosity", matchedStars, x => x.Luminosity);
        Close(sb, "star.habZoneMin", matchedStars, x => x.HabZoneMin);
        Close(sb, "star.habZoneMax", matchedStars, x => x.HabZoneMax);
        Close(sb, "star.ra", matchedStars, x => x.Ra);

        var lostHabitable = matched.Where(x => x.Old.Habitable == true && x.New.Habitable != true).Select(x => x.New.Name).ToArray();
        var newHabitable = next.Planets.Where(p => p.Habitable == true && !(oldPlanets.GetValueOrDefault(p.Name)?.Habitable ?? false)).Select(p => p.Name).ToArray();
        sb.AppendLine();
        sb.AppendLine($"No longer habitable ({lostHabitable.Length}): {string.Join(", ", lostHabitable)}");
        sb.AppendLine($"Newly habitable ({newHabitable.Length}): {string.Join(", ", newHabitable)}");
        return sb.ToString();
    }

    private static void Counts(StringBuilder sb, string label, IEnumerable<string> before, IEnumerable<string> after)
    {
        var a = before.ToHashSet();
        var b = after.ToHashSet();
        sb.AppendLine($"{label}: {a.Count} -> {b.Count} (kept {a.Intersect(b).Count()}, added {b.Except(a).Count()}, removed {a.Except(b).Count()})");
    }

    private static void Same<T, TValue>(StringBuilder sb, string label, IReadOnlyList<(T Old, T New)> pairs, Func<T, TValue> value)
    {
        var comparable = pairs.Where(x => value(x.Old) is not null).ToArray();
        if (comparable.Length == 0) return;
        int same = comparable.Count(x => Equals(value(x.Old), value(x.New)));
        sb.AppendLine($"  {label,-22} identical {Percent(same, comparable.Length)} of {comparable.Length}");
    }

    private static void Close<T>(StringBuilder sb, string label, IReadOnlyList<(T Old, T New)> pairs, Func<T, double?> value)
    {
        var diffs = pairs
            .Where(x => value(x.Old) is { } o && o != 0 && value(x.New) is not null)
            .Select(x => Math.Abs(value(x.New)!.Value - value(x.Old)!.Value) / Math.Abs(value(x.Old)!.Value))
            .ToArray();
        if (diffs.Length == 0) return;
        sb.AppendLine($"  {label,-22} within 5% {Percent(diffs.Count(d => d <= 0.05), diffs.Length)}, within 20% {Percent(diffs.Count(d => d <= 0.20), diffs.Length)} of {diffs.Length}");
    }

    private static string Percent(int part, int whole) => $"{100.0 * part / whole,5:F1}%";
}
