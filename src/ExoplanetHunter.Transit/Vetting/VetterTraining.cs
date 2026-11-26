using System.Globalization;
using System.Text;
using CSharpNumerics.Engines.Exoplanet.Features;
using CSharpNumerics.Engines.Exoplanet.Vetting;

namespace ExoplanetHunter.Transit.Vetting;

public sealed record VetterTrainingOptions
{
    /// <summary>
    /// Which examples to train on. Default: real TOIs only. Cross-validation showed that synthetic
    /// examples lower the score on real TOIs (docs/transit-vetting-model.md).
    /// </summary>
    public IReadOnlyCollection<SampleSource> Sources { get; init; } = [SampleSource.Toi];

    /// <summary>Training weight of synthetic examples (real TOIs weigh 1).</summary>
    public double SyntheticWeight { get; init; } = 1.0;

    public int Folds { get; init; } = 5;

    public VetterConfig Config { get; init; } = new();

    /// <summary>Model name, reported with every score.</summary>
    public string Name { get; init; } = "vetter";
}

/// <summary>A trained vetting model with its cross-validation and the examples it came from.</summary>
public sealed record VetterTrainingResult(
    CandidateVetter Model,
    VetterEvaluation Evaluation,
    IReadOnlyList<VettingSample> Samples,
    IReadOnlyList<VettingExample> Examples);

/// <summary>
/// Trains a <see cref="CandidateVetter"/> on a vetting dataset: grouped cross-validation over the
/// stars, measured on real TOIs, then a final model on all chosen examples.
/// </summary>
public static class VetterTraining
{
    public static VetterTrainingResult Train(VettingDatasetHeader header, IReadOnlyList<VettingSample> samples,
        VetterTrainingOptions? options = null)
    {
        options ??= new VetterTrainingOptions();
        if (header.FeatureSchemaVersion != VettingFeatures.SchemaVersion || !header.FeatureNames.SequenceEqual(VettingFeatures.Names))
            throw new InvalidDataException(
                $"The dataset has feature schema {header.FeatureSchemaVersion}; this build computes schema {VettingFeatures.SchemaVersion}. Build the dataset again.");

        var used = Select(samples, options).ToList();
        if (!used.Any(s => s.Source == SampleSource.Toi))
            throw new ArgumentException("Training needs real TOIs: they are what the model is evaluated on.", nameof(options));

        var examples = used.Select(s => ToExample(s, options.SyntheticWeight)).ToList();
        var evaluation = CandidateVetter.CrossValidate(examples, options.Config, options.Folds);
        var model = CandidateVetter.Train(examples, options.Config, options.Name);
        return new VetterTrainingResult(model, evaluation, used, examples);
    }

    /// <summary>Labelled samples from the chosen sources.</summary>
    internal static IEnumerable<VettingSample> Select(IEnumerable<VettingSample> samples, VetterTrainingOptions options) =>
        samples.Where(s => s.Label is 0 or 1 && options.Sources.Contains(s.Source));

    internal static VettingExample ToExample(VettingSample s, double syntheticWeight)
    {
        bool synthetic = s.Source != SampleSource.Toi;
        return new VettingExample(s.Id, s.TicId, s.Label, Array.ConvertAll(s.Features, v => (double)v),
            synthetic, synthetic ? syntheticWeight : 1.0);
    }

    /// <summary>Out-of-fold probability of every example: id, TIC, label, source, fold, probability.</summary>
    public static IEnumerable<string> OutOfFoldCsv(VetterTrainingResult result)
    {
        yield return "id,tic,label,source,fold,probability";
        for (int i = 0; i < result.Samples.Count; i++)
        {
            var s = result.Samples[i];
            yield return string.Join(",",
                Quote(s.Id), s.TicId.ToString(CultureInfo.InvariantCulture), s.Label.ToString(CultureInfo.InvariantCulture),
                s.Source.ToString(), result.Evaluation.Fold[i].ToString(CultureInfo.InvariantCulture),
                result.Evaluation.OutOfFold[i].ToString("0.0000", CultureInfo.InvariantCulture));
        }
    }

    /// <summary>Markdown report: data, settings, cross-validation metrics and calibration.</summary>
    public static string Report(VetterTrainingResult result, VetterTrainingOptions options, string datasetName, int modelBytes)
    {
        var e = result.Evaluation;
        var config = result.Model.Config;
        var sb = new StringBuilder();
        void Line(FormattableString s) => sb.AppendLine(s.ToString(CultureInfo.InvariantCulture));

        Line($"Vetting model **{result.Model.Name}**: a random forest on {result.Model.FeatureCount} vetting features (schema {result.Model.FeatureSchemaVersion}), trained on {result.Examples.Count} examples from {result.Samples.Select(s => s.TicId).Distinct().Count()} stars of dataset `{datasetName}`.");
        sb.AppendLine();
        Line($"Cross-validation: {e.Folds} folds, each star in one fold. Measured on the **{e.EvaluatedCount} real TOIs** ({e.EvaluatedPlanets} planets, {e.EvaluatedCount - e.EvaluatedPlanets} not), each scored by a model that did not see its star.");
        sb.AppendLine();
        sb.AppendLine("| Metric | Value |");
        sb.AppendLine("|---|---:|");
        Line($"| ROC-AUC | {e.RocAuc:F3} |");
        Line($"| Average precision | {e.AveragePrecision:F3} |");
        Line($"| Precision at 90 % recall | {e.PrecisionAt90Recall:F3} (threshold {e.ThresholdAt90Recall:F2}) |");
        Line($"| Accuracy at 0.5 | {e.Accuracy:F3} |");
        Line($"| Brier score | {e.BrierScore:F3} |");
        Line($"| Log loss | {e.LogLoss:F3} |");
        sb.AppendLine();

        sb.AppendLine("Calibration on the real TOIs: of the TOIs given a probability in a bin, the share that are planets.");
        sb.AppendLine();
        sb.AppendLine("| Mean probability | Share planets | TOIs |");
        sb.AppendLine("|---:|---:|---:|");
        for (int i = 0; i < e.Calibration.Counts.Length; i++)
            Line($"| {e.Calibration.MeanPredicted[i]:F2} | {e.Calibration.FractionPositive[i]:F2} | {e.Calibration.Counts[i]} |");
        sb.AppendLine();

        sb.AppendLine("Training examples and their mean out-of-fold probability:");
        sb.AppendLine();
        sb.AppendLine("| Source | Label | Examples | Weight | Mean probability |");
        sb.AppendLine("|---|---|---:|---:|---:|");
        foreach (var group in Enumerable.Range(0, result.Samples.Count)
                     .GroupBy(i => (result.Samples[i].Source, result.Samples[i].Label))
                     .OrderBy(g => g.Key.Source).ThenByDescending(g => g.Key.Label))
        {
            double weight = result.Examples[group.First()].Weight;
            Line($"| {group.Key.Source} | {(group.Key.Label == 1 ? "planet" : "not a planet")} | {group.Count()} | {weight:0.###} | {group.Average(i => e.OutOfFold[i]):F3} |");
        }
        sb.AppendLine();

        Line($"Settings: {config.NumTrees} trees, max depth {config.MaxDepth}, at least {config.MinSamplesLeaf} examples per leaf, {(config.MaxFeatures > 0 ? config.MaxFeatures.ToString(CultureInfo.InvariantCulture) : "√features")} features per split, seed {config.Seed}, classes balanced: {(config.BalanceClasses ? "yes" : "no")}. Sources: {string.Join(", ", options.Sources)}{(options.Sources.Any(x => x != SampleSource.Toi) ? FormattableString.Invariant($"; synthetic weight {options.SyntheticWeight:0.###}") : "")}.");
        sb.AppendLine();
        Line($"Model file: {modelBytes / 1024.0:F0} KB (the training examples; the forest is trained again when the file is loaded).");
        return sb.ToString();
    }

    private static string Quote(string value) =>
        value.Contains(',') || value.Contains('"') ? $"\"{value.Replace("\"", "\"\"")}\"" : value;
}
