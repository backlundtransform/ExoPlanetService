Vetting model **vetter-1**: a random forest on 15 vetting features (schema 1), trained on 2299 examples from 2159 stars of dataset `dataset-3`.

Cross-validation: 5 folds, each star in one fold. Measured on the **2299 real TOIs** (1355 planets, 944 not), each scored by a model that did not see its star.

| Metric | Value |
|---|---:|
| ROC-AUC | 0.915 |
| Average precision | 0.929 |
| Precision at 90 % recall | 0.854 (threshold 0.46) |
| Accuracy at 0.5 | 0.851 |
| Brier score | 0.117 |
| Log loss | 0.384 |

Calibration on the real TOIs: of the TOIs given a probability in a bin, the share that are planets.

| Mean probability | Share planets | TOIs |
|---:|---:|---:|
| 0.06 | 0.00 | 168 |
| 0.15 | 0.08 | 253 |
| 0.25 | 0.15 | 195 |
| 0.35 | 0.31 | 158 |
| 0.45 | 0.42 | 156 |
| 0.55 | 0.64 | 166 |
| 0.65 | 0.80 | 228 |
| 0.75 | 0.87 | 234 |
| 0.85 | 0.93 | 416 |
| 0.93 | 0.97 | 325 |

Training examples and their mean out-of-fold probability:

| Source | Label | Examples | Weight | Mean probability |
|---|---|---:|---:|---:|
| Toi | planet | 1355 | 1 | 0.746 |
| Toi | not a planet | 944 | 1 | 0.303 |

Settings: 300 trees, max depth 12, at least 5 examples per leaf, √features features per split, seed 1, classes balanced: yes. Sources: Toi; synthetic weight 0.3.

Model file: 155 KB (the training examples; the forest is trained again when the file is loaded).
