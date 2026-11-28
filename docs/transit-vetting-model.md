# Transit vetting model

The vetting model gives a transit candidate a probability of being a planet rather than an
eclipsing binary, stellar activity or an instrument effect. It is phase 2 of
[transit-ml-roadmap.md](transit-ml-roadmap.md), and it learns from the
[vetting dataset](transit-vetting-dataset.md).

Transit Lab scores its candidates with it (phase 4): the model in `data/transit/model/` gives
each candidate a *planet score* on its card. See step 10 in [transit-lab.md](transit-lab.md).
To use a new model, download `vetter.bin` and `report.md` from its `vetter-<run>` release into
`data/transit/model/`; the next weekly run scores every star again.

## The model

A random forest (`CandidateVetter` in CSharpNumerics.Engines 1.3) on the scalar vetting
features of each signal: depth, duration, signal-to-noise, how consistent the transits are, odd
and even depths, a secondary eclipse, the transit shape, the duration compared with what the star
predicts, the planet radius and the host star. Feature schema 2 (Engines 1.3.0-preview.2) adds
four aimed at instrument effects and stellar activity: how much the individual transit depths
differ (`DepthChi2`), transits cut by gaps (`PartialTransitFraction`), transits near gaps and
the ends of the data (`GapEdgeFraction`), and the noise near the transits against the noise
everywhere (`LocalNoiseRatio`). `vetter-1` used the 15 features of schema 1.

The forest's probabilities are then **calibrated** with Platt scaling, fitted on its 5-fold
out-of-fold probabilities of the real TOIs, so that of the candidates given 0.8, about 80 % are
planets. Without it the probabilities are squeezed towards the middle: `vetter-1` gave 0.75 to
TOIs of which 91 % were planets. The cross-validation calibrates each fold's model on its own
training stars, so the reported metrics are of calibrated models.

| Setting | Value |
|---|---|
| Trees | 300 |
| Max depth | 12 |
| Examples per leaf | at least 5, for smoother probabilities |
| Features per split | √15 ≈ 4, chosen at random |
| Class balance | planets and non-planets weigh the same in total |
| Seed | 1: training is repeatable |

The phase-folded views in the dataset are not used yet; a network on the views is phase 3.

## Training data: real TOIs only

By default the model trains on the **real TOIs** only (CP and KP as planets, FP and FA as not),
not on the synthetic examples. A cross-validation of the first dataset (`dataset-3`, 2,299 real
TOIs) showed that the synthetic examples make the model worse at what matters, scoring real TOIs:

| Training data | ROC-AUC | Average precision | Precision at 90 % recall |
|---|---:|---:|---:|
| **Real TOIs only** | **0.915** | **0.932** | **0.851** |
| TOIs + synthetic, weight 0.1 | 0.891 | 0.916 | 0.804 |
| TOIs + binaries, inverted, scrambled, weight 0.3 | 0.899 | 0.921 | 0.818 |
| TOIs + synthetic, weight 0.3 | 0.861 | 0.889 | 0.772 |
| TOIs + synthetic, weight 1 | 0.813 | 0.846 | 0.742 |

These numbers come from scikit-learn's random forest with the same settings and the same
grouped folds, run before the C# training command existed. The training workflow reports the
numbers of the model itself.

The model separates the synthetic examples easily (injected planets average 0.87, inverted light
curves 0.03), but that teaches it little about real false positives. Injected planets hurt most:
they put planets on stars whose real TOIs are false positives, which tells the model that the
host star does not matter, while among real TOIs it does.

The synthetic examples still have a use: injection–recovery tests of the search and the model.
`--sources` and `--synthetic-weight` bring them back for experiments, and they may help the
network on the views in phase 3.

## Evaluation

The training command cross-validates before it trains the final model:

- **5 folds, each star in one fold.** A star's TOIs (and synthetic examples) are never on both
  sides, so the model is not scored on a star it has seen.
- **Measured on real TOIs only**, each by a model that did not see its star.
- **Metrics:** ROC-AUC, average precision, precision at 90 % recall with its threshold, accuracy
  at 0.5, Brier score, log loss, and a calibration table (of the TOIs given about 0.8, how many
  are planets).

### Transit Lab's own candidates

The TOIs hold few of the false signals Transit Lab finds: instrument effects and stellar
activity on the well-studied stars it searches. The weekly run therefore also measures the
model on its own candidates (`LabEvaluation`, at the end of the run's summary): candidates that
match a known planet count as planets, secondary eclipses and unmatched signals as not. A new
planet among the unmatched signals would count against the model, so the numbers are for
comparing models. `vetter-1` (2026-11-27): ROC-AUC 0.833; 7 of 21 unmatched signals scored 0.5
or more.

A high score partly reflects the TOI catalogue: the host star's temperature, surface gravity and
radius are among the most used features, and some of that is how TOIs were selected and followed
up rather than astrophysics. Scores should be presented as "looks like a planet", not as a
discovery.

## Running it

*Actions → Transit vetting model → Run workflow.*

| Input | Default | |
|---|---|---|
| `dataset` | newest `dataset-*` release | Dataset release to train on |
| `sources` | `toi` | `all`, or a list of `toi`, `planets`, `binaries`, `inverted`, `scrambled` |
| `synthetic_weight` | 0.3 | Weight of synthetic examples, when `sources` includes them |
| `publish` | on | Publish a release `vetter-<run number>`; off keeps only the workflow artifact |

The release holds:

| File | |
|---|---|
| `vetter.bin` | The model |
| `report.md` | Metrics, calibration, training examples and settings (also the release notes) |
| `oof.csv` | Out-of-fold probability of every training example: `id,tic,label,source,fold,probability` |

Locally:

```
dotnet run --project src/ExoplanetHunter.Transit.Cli -c Release -- train --dataset vetting-dataset.bin --output model
```

Options: `--name`, `--dataset-name`, `--sources`, `--synthetic-weight`, `--folds`, `--trees`,
`--max-depth`, `--min-leaf`, `--max-features`, `--seed`.

## The model file

`vetter.bin` holds the settings and the training examples (features as 32-bit floats), not the
trees: loading it trains the forest again, which takes a few seconds and gives the same model
because training is seeded and deterministic. CSharpNumerics cannot save trees yet.

A fingerprint of the model's output is saved too. Loading fails if the retrained forest differs,
for example after a CSharpNumerics change, so a model never changes silently. The training
command loads the saved file once to prove this works. A model trained on another feature schema
is refused as well.

## Next

- **Phase 3:** a network on the phase-folded views, stacked with these features.
