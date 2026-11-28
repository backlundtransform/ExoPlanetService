Vetting model **vetter-2**: a random forest on 19 vetting features (schema 2), trained on 2299 examples from 2159 stars of dataset `dataset-4`.

Cross-validation: 5 folds, each star in one fold. Measured on the **2299 real TOIs** (1355 planets, 944 not), each scored by a model that did not see its star.

| Metric | Value |
|---|---:|
| ROC-AUC | 0.915 |
| Average precision | 0.930 |
| Precision at 90 % recall | 0.858 (threshold 0.47) |
| Accuracy at 0.5 | 0.855 |
| Brier score | 0.111 |
| Log loss | 0.358 |

Calibration on the real TOIs: of the TOIs given a probability in a bin, the share that are planets.

| Mean probability | Share planets | TOIs |
|---:|---:|---:|
| 0.04 | 0.04 | 366 |
| 0.14 | 0.13 | 216 |
| 0.25 | 0.19 | 118 |
| 0.35 | 0.43 | 104 |
| 0.45 | 0.38 | 110 |
| 0.55 | 0.63 | 95 |
| 0.65 | 0.74 | 130 |
| 0.75 | 0.78 | 171 |
| 0.86 | 0.84 | 238 |
| 0.96 | 0.95 | 751 |

Training examples and their mean out-of-fold probability:

| Source | Label | Examples | Weight | Mean probability |
|---|---|---:|---:|---:|
| Toi | planet | 1355 | 1 | 0.811 |
| Toi | not a planet | 944 | 1 | 0.269 |

Settings: 300 trees, max depth 12, at least 5 examples per leaf, √features features per split, seed 1, classes balanced: yes. Sources: Toi; synthetic weight 0.3.

Calibration: Platt scaling fitted on 5-fold out-of-fold probabilities of the real TOIs, slope 1.654, intercept 0.093. The cross-validation metrics above are of calibrated models, each calibrated on its own training stars.

Model file: 191 KB (the training examples; the forest is trained again when the file is loaded).
