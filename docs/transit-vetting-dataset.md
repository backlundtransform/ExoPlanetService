# Transit vetting dataset

Training data for the model that will score Transit Lab candidates: how much a transit-like
signal looks like a planet rather than an eclipsing binary, stellar activity or an instrument
effect. See [transit-ml-roadmap.md](transit-ml-roadmap.md) for the whole plan.

The dataset is built by hand-started GitHub Actions runs and published as a release asset.
Nothing runs in Azure, and the data is never committed to git.

## Examples

Each example is one signal on one star: five phase-folded views and 15 scalar features, with a
label.

| Source | Label | What it is |
|---|---|---|
| `Toi` | 1 for CP and KP, 0 for FP and FA | A TESS Object of Interest at its catalogue period, epoch and duration, from the NASA Exoplanet Archive `toi` table |
| `InjectedPlanet` | 1 | A limb-darkened planet transit injected into the same star's light curve, with a signal-to-noise between 8 and 40 |
| `EclipsingBinary` | 0 | V-shaped eclipses with a secondary eclipse, and in half of them odd and even eclipses of different depth |
| `Inverted` | 0 | The strongest BLS signal in the star's light curve turned upside down |
| `Scrambled` | 0 | The strongest BLS signal after 2-day chunks of the light curve are shuffled |

TOIs without a TFOPWG decision (PC, APC) are left out; `--include-undecided true` adds them with
label −1, for scoring rather than training.

The synthetic examples use each star's own noise and variability, so a model cannot tell real
from synthetic examples by the noise. Inverted and scrambled light curves are the method the
Kepler Robovetter used to measure false alarms (Thompson et al. 2018).

The views and features come from CSharpNumerics.Engines 1.2 (`TransitViewBuilder`,
`VettingFeatureExtractor`):

| View | Bins | Covers |
|---|---|---|
| Global | 201 | The whole orbit, transit in the middle |
| Local | 61 | ±2 transit durations around the transit |
| Local odd, local even | 61 each | As local, odd- or even-numbered transits only |
| Secondary | 61 | As local, half an orbit later |

All views are scaled so that out of transit is 0 and the bottom of the transit is −1. The features
are listed in the Engines README (`VettingFeatures.Names`, schema version 1).

## Steps for each star

1. **Light curves.** Find the star's 2-minute SPOC light curves in MAST and choose up to 3
   consecutive sectors, as close as possible to the epoch of its TOIs. That is where the
   catalogue ephemeris is most accurate.
2. **Clean and join.** Keep good points, divide each sector by its median and join them.
3. **Detrend.** A biweight filter over 0.75 days (or 3 transit durations, if longer), with every
   TOI of the star masked so the filter does not eat into any transit.
4. **TOI examples.** For each labelled TOI, build the views and features at its catalogue
   ephemeris. A TOI is skipped if its predicted transit times may have drifted by more than half a
   duration over the data (1σ, from the period and epoch errors), or if no transit falls in the data.
5. **Synthetic examples.** Inject one planet and one eclipsing binary, detrend again and build
   their examples. Invert and scramble the light curve, run BLS on each, and build an example from
   the strongest signal.

Synthetic signals get a period between 0.5 days and a third of the observed span (at most 20
days), so at least three transits fall in the data. Everything random is seeded from the TIC id,
so a rerun gives the same dataset.

## Running it

*Actions → Transit vetting dataset → Run workflow.*

| Input | Default | |
|---|---|---|
| `shards` | 16 | Parallel jobs. Each handles every 16th star |
| `limit` | 0 | Stars per job. Use e.g. 3 for a trial run |
| `synthetic` | `all` | `none`, or a list such as `planets,binaries` |
| `publish` | on | Publish a release `dataset-<run number>`; off keeps only the workflow artifact |

About 2,700 stars have labelled TOIs; most have 2-minute data. A full run should take a few
hours across the jobs. Each job stops starting new stars after 300 minutes and saves what it
has, so a slow MAST day gives a smaller dataset rather than a failed run.

The release holds `vetting-dataset.bin` and `report.md`: examples per source and label, and why
examples were skipped.

Locally:

```
dotnet run --project src/ExoplanetHunter.Transit.Cli -c Release -- toi --output toi.json
dotnet run --project src/ExoplanetHunter.Transit.Cli -c Release -- dataset --toi toi.json --limit 3 --output out/test.bin
dotnet run --project src/ExoplanetHunter.Transit.Cli -c Release -- dataset-merge --input out --output dataset.bin --report report.md
```

## File format

`vetting-dataset.bin` is little-endian binary, written and read by
[`VettingDataset`](../src/ExoplanetHunter.Transit/Vetting/VettingDataset.cs):

- Header: magic `EXVD`, format version 1, feature schema version, global bins, local bins, local
  half-width in durations, and the feature names.
- Then for each example: id, TIC id, label, source, period, and the five views and the features
  as 32-bit floats. That is about 1.9 KB per example.

A model must be trained and used with the same feature schema version and view layout. The
header records both, and `dataset-merge` refuses files whose layouts differ.

## Limitations

- **2-minute data only.** TOIs found only in full-frame images (QLP, TESS-SPOC FFI) are skipped.
- **Catalogue ephemerides.** TOI examples use the archive's period, epoch and duration, not a
  fresh fit. The drift check removes the worst cases.
- **Other planets on the same star** are masked for detrending, but they can still fall in
  another TOI's views.
- **No centroid information.** A signal from a nearby star looks the same as one from the target.
