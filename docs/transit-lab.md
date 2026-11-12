# Transit Lab

Transit Lab searches TESS light curves of known planet hosts for transits, then shows
what it found next to the planets in the catalogue. The goal is to show how planets are
found, not to discover new ones.

The analysis is precomputed and does not run in Azure:

1. A GitHub Actions job runs every week ([`.github/workflows/transit.yml`](../.github/workflows/transit.yml)).
2. The job writes one JSON file per star to [`data/transit/`](../data/transit/).
3. Once the results pull request is merged, the API picks the files up from GitHub within a
   day, and the web app draws them at `/astro3d/transit`.

## Weekly run

| | |
|---|---|
| When | Mondays 03:00 UTC, or manually from *Actions → Transit Lab → Run workflow* |
| Manual options | `only`: one star by name. `force`: analyse again even when nothing changed |
| Stars | [`data/transit/targets.json`](../data/transit/targets.json), 94 known systems |
| Catalogue | [`data/catalog.json.gz`](../data/catalog.json.gz), for star ids and known planets |
| Output | `data/transit/<starId>.json` per star, plus `data/transit/index.json` |
| Delivery | A pull request from the branch `transit-lab/results`, with a summary table |
| Time | About 20–40 minutes for a run that analyses every star (most of it downloads); a few minutes when little changed |
| Cost | Nothing: the repository is public, so Actions minutes are free; nothing runs in Azure |

### Steps for each star

1. **Find the star.** Look the star up by name in the catalogue, then find its TIC id in the
   NASA Exoplanet Archive (`pscomppars.tic_id`).
2. **Find the light curves.** Ask MAST for the star's TESS 2-minute light curves from the SPOC
   pipeline (`*_lc.fits`).
3. **Choose sectors.** Keep the longest run of consecutive sectors, at most 3 sectors (about
   80 days). When runs are equally long, keep the most recent one.

   Sectors years apart are not combined: the period grid is linear, so a long gap would need
   far more trial periods.
4. **Skip if nothing changed.** If the previous result used the same sectors, the same catalogue
   planets and the same analysis version, keep it and go to the next star.

   Most weeks no new data arrives, so the files and git stay unchanged.
5. **Download and analyse.** Download the FITS files and run the analysis described below. The
   downloads are cached between runs with `actions/cache` and pruned after 180 days.

If a star fails, its previous result is kept and the run continues; a MAST outage is one way
this happens. The job fails only if every star fails. When any result changed, the job opens a
pull request or updates the open one; when nothing changed, it does nothing.

> **Before the first run:** turn on *Settings → Actions → General → Allow GitHub Actions to
> create and approve pull requests*.
>
> A pull request opened by the job does not start the **API** workflow, because GitHub does not
> run workflows for PRs created with the job's own token. Close and reopen the pull request to
> start CI.

### Publishing

Merging the pull request is enough: no deploy is needed.

With `Transit:Refresh:Enabled`, the API checks `data/transit/index.json` on the `master`
branch once a day, using raw.githubusercontent.com. It downloads only the stars whose
`generatedAt` changed and saves them under `Transit:Refresh:StoragePath`, which survives
restarts and deploys. It then serves them from there. The code is in
[`TransitRefreshService`](../src/ExoplanetHunter.Api/Transit/TransitRefreshService.cs).

A day with no change costs one request of about 10 KB. Merged results are therefore live within
a day.

The refresh has these safeguards:

- **A broken or empty download is rejected**, and the current results stay.
- **Newer bundled results are kept.** If the deployment ships newer results than `master`, as
  after a deploy from another branch, the API keeps serving the bundled ones.
- **Stars removed from the index are removed** from storage too.

Without the setting, the API serves the copy shipped with the deployment (`Data/transit/`).

**Production app settings (Azure App Service → Configuration):**

| Setting | Value |
|---|---|
| `Transit__Refresh__Enabled` | `true` |
| `Transit__Refresh__StoragePath` | `D:\home\data\transit` |

Optional settings are `Transit__Refresh__Interval` (default `1.00:00:00`) and
`Transit__Refresh__SourceUrl`.

## The analysis

The search uses `TransitDetectionPipeline` from the NuGet package
[CSharpNumerics.Engines](https://www.nuget.org/packages/CSharpNumerics.Engines) (1.0.4), wrapped
by [`TransitAnalyzer`](../src/ExoplanetHunter.Transit/TransitAnalyzer.cs).

1. **Clean.** Read `PDCSAP_FLUX` from each sector. Keep only points with `QUALITY = 0` and finite
   values, then divide each sector by its median. Sectors are joined into one light curve, with
   time in BTJD (BJD − 2457000).
2. **Bin.** Average the points into 10-minute bins. A three-sector light curve then has about
   10,000 points instead of 60,000.
3. **Detrend.** Divide the curve by a running median about 17 hours wide (101 bins). The window
   removes stellar variability and slow instrument drifts, but is much wider than a transit, so
   it keeps the transits.
4. **Search with BLS (Box Least Squares).** Fold the light curve on 20,000 trial periods,
   spaced linearly from 0.3 days to half the observed span (at most 100 days). For each trial
   period, BLS finds the box-shaped dip that best fits the folded curve. The strongest period is
   then refined on a finer grid of 400 periods around the peak.

   Requiring at least two transits in the data is what sets the longest period.
5. **Fit.** At the best period, fit the transit epoch, depth, duration and radius ratio Rp/R★,
   where depth ≈ (Rp/R★)².
6. **Validate.** A signal is accepted as a candidate only if it passes the checks marked
   *reject* below. The other checks lower the candidate's score and add a warning, which is
   shown on the candidate card.

   | Check | Rule | On failure |
   |---|---|---|
   | Signal-to-noise | SNR = depth / σ × √N(in-transit points) must be at least 7 | Reject |
   | Depth | At least 100 ppm | Reject |
   | Period | Inside the search range | Reject |
   | Odd and even transits | Depths within a factor of 2. Unequal depths suggest an eclipsing binary at twice the period | Warning |
   | Duration | Shorter than 20 % of the period | Warning |
   | Secondary eclipse | Shallower than 30 % of the transit; a deeper one suggests an eclipsing binary | Warning |

7. **Repeat.** Mask the found transits (±1 duration around each) and search again, up to 3
   candidates. A rejected signal is masked as well, so a strong false signal cannot hide a
   weaker real one.
8. **Compare with the catalogue.** Compare the candidates with the planets the catalogue lists
   for the star:

   | Status | Meaning |
   |---|---|
   | `found` | A candidate's period is within 1 % of the planet's period |
   | `found-at-harmonic` | A candidate is at twice or half the period, where BLS often locks on |
   | `not-found` | No candidate matches. The planet is often too small or shallow for the noise, or it does not transit |
   | `out-of-range` | The period is longer than half the observed span, so two transits cannot be seen |
   | `no-period` | The catalogue has no period for the planet |

   Each candidate is matched to at most one planet.
9. **Mark secondary eclipses.** A candidate with the same period as a found planet but half an
   orbit later is marked as that planet's secondary eclipse: the planet passing behind its star.
   WASP-18 b's secondary eclipse shows up this way.

   A candidate that matches nothing is shown as an *unmatched signal*. It is usually noise,
   stellar activity or an instrument effect, and is not a claimed discovery.

For display, the stored result is reduced to these series:

| Series | Reduction |
|---|---|
| Light curve | Binned to 30 minutes |
| Periodogram | Reduced to 1,500 points, keeping peaks |
| Folded curve | One per candidate, at that candidate's period |

When a change to the analysis or the engine would change the results, bump
`TransitAnalyzer.Version`. The next run then analyses every star again, because a result with
an older version is never reused.

## Target list

[`targets.json`](../data/transit/targets.json) lists the stars by their catalogue name. It is
picked by hand to show the method well. Every star meets these requirements:

| Requirement | Reason |
|---|---|
| A transiting planet in the NASA Exoplanet Archive, with a TIC id | MAST has no data for the star without one |
| TESS magnitude brighter than about 12.5 | Fainter stars are too noisy for small planets |
| At least one planet with a period short enough for two transits in about 80 days | Otherwise the search cannot find it |

The list covers two groups:

- **Hot Jupiters** such as WASP-18, WASP-121, KELT-9 and HD 189733. Their transits are deep and
  found with very high SNR, and some also show a secondary eclipse.
- **Multi-planet and small-planet systems** such as L 98-59, TOI-270, TOI-178, HD 23472,
  LTT 1445 A and AU Mic. They show the limits of the method: small planets often fall below the
  noise, and long periods fall out of range.

Some stars left out on purpose:

- **TRAPPIST-1:** too faint for TESS.
- **K2-18 and TOI-2257:** too faint, with long periods.
- **WASP-103 and GJ 1214:** MAST has no 2-minute SPOC light curves for them.

**To add a star:**

1. Add its catalogue name to `targets.json`.
2. Start the workflow with `only` set to that name, or wait for the next weekly run.

A star with no 2-minute SPOC data fails with a message in the run summary and gets no result.

## Running locally

```sh
dotnet run --project src/ExoplanetHunter.Transit.Cli -c Release -- --only "WASP-18"
```

| Option | Default | Effect |
|---|---|---|
| `--targets` | `data/transit/targets.json` | Target list |
| `--catalog` | `data/catalog.json.gz` | Catalogue |
| `--output` | `data/transit` | Directory for the results |
| `--cache` | `.cache/mast` | MAST download cache (gitignored) |
| `--max-sectors` | `3` | Most sectors per star |
| `--force` | `false` | Analyse even when the result could be reused |
| `--summary` | | Write a Markdown report of the run |

## Limitations

- **Only BLS.** The search looks only for box-shaped dips. A machine-learning classifier of
  candidates may come later.
- **Short periods.** Small planets with periods under about one day are often missed: LHS 3844 b,
  TOI-1807 b, TOI-1685 b and Kepler-10 b. The trial periods are spaced linearly, 0.002 days
  apart over three sectors. Over about 170 orbits, that error moves the predicted transits by
  more than a transit duration. Deep transits survive this, which is why hot Jupiters are
  found. The search then often locks on to 3× the period instead (LHS 3844). A grid uniform in
  frequency in CSharpNumerics.Engines would fix it.
- **Young or active stars.** Starspot rotation of a few days, as on HIP 67522, DS Tuc A and
  AU Mic, survives the detrending and appears as unmatched signals.
- **At most 3 sectors and 3 candidates per star.** Planets beyond the third signal are not
  searched for.
- **Catalogue snapshot.** The job uses the catalogue snapshot in the repository, not the weekly
  refreshed copy in production. A planet added only in production is therefore missing from
  the comparison until `data/catalog.json.gz` is updated.
