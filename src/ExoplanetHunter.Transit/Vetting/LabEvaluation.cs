using System.Globalization;
using System.Text;

namespace ExoplanetHunter.Transit.Vetting;

/// <summary>A scored Transit Lab candidate that matches no known planet.</summary>
public sealed record UnmatchedSignal(string Star, double PeriodDays, int? Transits, double Score);

/// <summary>
/// How the vetting model scores Transit Lab's own candidates, a second test set beside the TOIs
/// it was trained on: candidates that match a known planet count as planets, secondary eclipses
/// and unmatched signals as not.
/// </summary>
/// <remarks>
/// Unmatched signals on these well-studied stars are almost all noise, stellar activity or
/// instrument effects, the false positives the TOI catalogue holds few of. A new planet among
/// them would count against the model, so read the numbers as a comparison between models
/// rather than as exact rates.
/// </remarks>
public sealed record LabEvaluation(
    IReadOnlyList<string> Models,
    int Planets,
    int SecondaryEclipses,
    int Unmatched,
    int Unscored,
    double? PlanetMean,
    double? SecondaryMean,
    double? UnmatchedMean,
    int UnmatchedAboveHalf,
    double? RocAuc,
    IReadOnlyList<UnmatchedSignal> HighestUnmatched)
{
    public static LabEvaluation Evaluate(IEnumerable<TransitResult> results, int highest = 5)
    {
        var planets = new List<double>();
        var secondaries = new List<double>();
        var unmatched = new List<UnmatchedSignal>();
        var models = new SortedSet<string>(StringComparer.Ordinal);
        int unscored = 0;

        foreach (var result in results)
        {
            if (result.VettingModel is null) continue;
            models.Add(result.VettingModel);
            foreach (var c in result.Candidates)
            {
                if (c.VettingScore is not { } score) { unscored++; continue; }
                if (c.SecondaryEclipseOf is not null) secondaries.Add(score);
                else if (c.MatchedPlanet is not null) planets.Add(score);
                else unmatched.Add(new UnmatchedSignal(result.StarName, c.PeriodDays, c.TransitsObserved, score));
            }
        }

        var negatives = secondaries.Concat(unmatched.Select(u => u.Score)).ToList();
        return new LabEvaluation(
            models.ToArray(),
            planets.Count, secondaries.Count, unmatched.Count, unscored,
            Mean(planets), Mean(secondaries), Mean(unmatched.Select(u => u.Score).ToList()),
            unmatched.Count(u => u.Score >= 0.5),
            planets.Count > 0 && negatives.Count > 0 ? Auc(planets, negatives) : null,
            unmatched.OrderByDescending(u => u.Score).Take(highest).ToArray());
    }

    /// <summary>A Markdown section for the weekly summary.</summary>
    public string ToMarkdown()
    {
        var sb = new StringBuilder();
        void Line(FormattableString s) => sb.AppendLine(s.ToString(CultureInfo.InvariantCulture));

        Line($"### Planet scores on Transit Lab's own candidates ({(Models.Count == 0 ? "no model" : string.Join(", ", Models))})");
        sb.AppendLine();
        if (Planets + SecondaryEclipses + Unmatched == 0)
        {
            sb.AppendLine("No scored candidates.");
            return sb.ToString();
        }

        sb.AppendLine("Known planets should score high; secondary eclipses and unmatched signals, mostly noise and instrument effects on these well-studied stars, low.");
        sb.AppendLine();
        sb.AppendLine("| Candidates | Scored | Mean score |");
        sb.AppendLine("|---|---:|---:|");
        Line($"| Known planets | {Planets} | {Format(PlanetMean)} |");
        Line($"| Secondary eclipses | {SecondaryEclipses} | {Format(SecondaryMean)} |");
        Line($"| Unmatched signals | {Unmatched} | {Format(UnmatchedMean)} |");
        sb.AppendLine();
        Line($"ROC-AUC, known planets against the rest: {Format(RocAuc, "0.000")}. Unmatched signals at 0.5 or more: {UnmatchedAboveHalf} of {Unmatched}. Not scored (too few transits): {Unscored}.");
        if (HighestUnmatched.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("Highest-scored unmatched signals: " + string.Join(", ", HighestUnmatched.Select(u =>
                string.Create(CultureInfo.InvariantCulture, $"{u.Star} {u.PeriodDays:0.00} d ({u.Transits} transits) {u.Score:0.00}"))) + ".");
        }
        return sb.ToString();
    }

    private static double? Mean(List<double> values) => values.Count > 0 ? values.Average() : null;

    private static string Format(double? value, string format = "0.00") =>
        value is { } v ? v.ToString(format, CultureInfo.InvariantCulture) : "–";

    /// <summary>Probability that a random positive scores above a random negative; ties count half.</summary>
    internal static double Auc(IReadOnlyCollection<double> positives, IReadOnlyCollection<double> negatives)
    {
        double wins = 0;
        foreach (double p in positives)
            foreach (double n in negatives)
                wins += p > n ? 1 : p == n ? 0.5 : 0;
        return wins / ((double)positives.Count * negatives.Count);
    }
}
