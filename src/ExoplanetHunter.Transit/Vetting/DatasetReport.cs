using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ExoplanetHunter.Transit.Vetting;

/// <summary>Markdown summary of a vetting dataset: examples per source and label, and why examples were skipped.</summary>
public static partial class DatasetReport
{
    public static string Build(IReadOnlyCollection<VettingSample> samples, IReadOnlyCollection<SkippedSample> skipped,
        VettingDatasetHeader header)
    {
        var sb = new StringBuilder();
        int stars = samples.Select(s => s.TicId).Concat(skipped.Select(s => s.TicId)).Distinct().Count();
        sb.AppendLine(Invariant($"Vetting dataset: {samples.Count} examples from {samples.Select(s => s.TicId).Distinct().Count()} stars ({stars} stars tried), {skipped.Count} skipped."));
        sb.AppendLine();
        sb.AppendLine(Invariant($"Layout: feature schema {header.FeatureSchemaVersion} ({header.FeatureNames.Length} features), global view {header.GlobalBins} bins, local views {header.LocalBins} bins over ±{header.LocalHalfWidthDurations} durations."));
        sb.AppendLine();

        sb.AppendLine("| Source | Planet | Not a planet | Unknown |");
        sb.AppendLine("|---|---:|---:|---:|");
        foreach (var group in samples.GroupBy(s => s.Source).OrderBy(g => g.Key))
            sb.AppendLine(Invariant($"| {group.Key} | {group.Count(s => s.Label == 1)} | {group.Count(s => s.Label == 0)} | {group.Count(s => s.Label < 0)} |"));
        sb.AppendLine(Invariant($"| **Total** | **{samples.Count(s => s.Label == 1)}** | **{samples.Count(s => s.Label == 0)}** | **{samples.Count(s => s.Label < 0)}** |"));

        if (skipped.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("Skipped examples by reason (numbers replaced by N):");
            sb.AppendLine();
            sb.AppendLine("| Reason | TOIs | Synthetic |");
            sb.AppendLine("|---|---:|---:|");
            foreach (var group in skipped.GroupBy(s => Numbers().Replace(s.Reason, "N")).OrderByDescending(g => g.Count()).Take(15))
            {
                int tois = group.Count(s => s.Id.StartsWith("TOI-", StringComparison.Ordinal));
                sb.AppendLine(Invariant($"| {group.Key.Replace('|', '/')} | {tois} | {group.Count() - tois} |"));
            }
        }

        return sb.ToString();
    }

    private static string Invariant(FormattableString s) => s.ToString(CultureInfo.InvariantCulture);

    [GeneratedRegex(@"\d+(\.\d+)?")]
    private static partial Regex Numbers();
}
