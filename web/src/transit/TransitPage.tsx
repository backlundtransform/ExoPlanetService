import { useEffect, useMemo, useState } from 'react'
import { Link, useParams } from 'react-router'
import { useTranslation } from 'react-i18next'
import { ApiError, fetchTransit } from '../api'
import type { KnownPlanetStatus, TransitCandidate, TransitResult } from '../types'
import { Chart, type ChartMarker } from './Chart'

/** Candidate colours: the first three categorical slots, validated together on the app's dark surface. */
const CANDIDATE_COLORS = ['#3987e5', '#d95926', '#199e70']
const DATA_COLOR = '#8b949e'
const KNOWN_COLOR = '#c9d1d9'

const STATUS_ICON: Record<KnownPlanetStatus, string> = {
  found: '✔',
  'found-at-harmonic': '≈',
  'not-found': '✖',
  'out-of-range': '⏱',
  'no-period': '–',
}

type State = { status: 'loading' } | { status: 'ready'; result: TransitResult } | { status: 'missing' } | { status: 'error'; message: string }

/** /transit/:id — the precomputed TESS transit search for one star. */
export default function TransitPage() {
  const { t } = useTranslation()
  const { id } = useParams()
  const [state, setState] = useState<State>({ status: 'loading' })

  useEffect(() => {
    const controller = new AbortController()
    setState({ status: 'loading' })
    fetchTransit(Number(id), controller.signal)
      .then((result) => setState({ status: 'ready', result }))
      .catch((error: unknown) => {
        if (controller.signal.aborted) return
        if (error instanceof ApiError && error.status === 404) setState({ status: 'missing' })
        else setState({ status: 'error', message: error instanceof Error ? error.message : String(error) })
      })
    return () => controller.abort()
  }, [id])

  useEffect(() => {
    if (state.status === 'ready') document.title = `${state.result.starName} · Transit Lab · Exoplanet Hunter`
  }, [state])

  if (state.status !== 'ready') {
    return (
      <div className="page-message">
        {state.status === 'loading' && <p>{t('app.loading')}</p>}
        {state.status === 'missing' && <p>{t('transit.not_analysed')}</p>}
        {state.status === 'error' && <p className="error">{t('app.error', { message: state.message })}</p>}
        <Link to="/transit">← {t('transit.title')}</Link>
      </div>
    )
  }

  return <TransitReport result={state.result} />
}

function TransitReport({ result }: { result: TransitResult }) {
  const { t, i18n } = useTranslation()
  const number = (value: number, digits = 0) =>
    value.toLocaleString(i18n.language, { maximumFractionDigits: digits, minimumFractionDigits: digits })

  // Predicted transit times of each candidate, as short ticks under the light curve.
  const transitTicks = useMemo<ChartMarker[]>(() => {
    const times = result.lightCurve.x
    if (times.length === 0) return []
    const [start, end] = [times[0], times[times.length - 1]]
    return result.candidates.flatMap((c, i) => {
      const ticks: ChartMarker[] = []
      for (let time = c.epochBtjd; time <= end && c.periodDays > 0; time += c.periodDays)
        if (time >= start) ticks.push({ x: time, color: CANDIDATE_COLORS[i % 3], tick: true })
      return ticks
    })
  }, [result])

  // BLS power has no natural unit; show it relative to the strongest peak.
  const relativePower = useMemo(() => {
    const max = Math.max(...result.periodogram.y)
    return max > 0 ? result.periodogram.y.map((v) => v / max) : result.periodogram.y
  }, [result])

  const periodMarkers: ChartMarker[] = [
    ...result.knownPlanets
      .filter((k) => k.periodDays && k.status !== 'out-of-range' && k.status !== 'no-period')
      .map((k) => ({ x: k.periodDays!, color: KNOWN_COLOR, dashed: true })),
    ...result.candidates.map((c, i) => ({ x: c.periodDays, color: CANDIDATE_COLORS[i % 3], label: `${i + 1}` })),
  ]

  return (
    <div className="transit-page">
      <header className="transit-page__header">
        <Link to="/transit">← {t('transit.title')}</Link>
        <h1>{result.starName}</h1>
        <p className="muted">
          {t('transit.observed', {
            mission: result.mission,
            sectors: result.sectors.join(', '),
            points: number(result.points),
            tic: result.ticId,
          })}{' '}
          <Link to={`/star/${result.starId}`}>{t('transit.open_system')}</Link>
        </p>
      </header>

      <section className="transit-section">
        <h2>{t('transit.known_planets')}</h2>
        {result.knownPlanets.length === 0 ? (
          <p className="muted">{t('transit.no_known_planets')}</p>
        ) : (
          <ul className="known-planets">
            {result.knownPlanets.map((k) => (
              <li key={k.name} className={`known-planet known-planet--${k.status}`}>
                <span className="known-planet__icon" aria-hidden>{STATUS_ICON[k.status]}</span>
                <span className="known-planet__name">{k.name}</span>
                <span className="muted">
                  {k.periodDays != null && <>{number(k.periodDays, 3)} {t('app.planet_days')} · </>}
                  {t(`transit.status.${k.status}`)}
                  {k.candidateIndex != null && <> ({t('transit.candidate_n', { n: k.candidateIndex + 1 })})</>}
                </span>
              </li>
            ))}
          </ul>
        )}
      </section>

      <section className="transit-section">
        <h2>{t('transit.light_curve')}</h2>
        <p className="muted small">{t('transit.light_curve_help')}</p>
        <Chart
          ariaLabel={t('transit.light_curve')}
          series={[{ label: t('transit.light_curve'), x: result.lightCurve.x, y: result.lightCurve.y, color: DATA_COLOR, kind: 'dots' }]}
          markers={transitTicks}
          xLabel={t('transit.time_axis')}
          yLabel={t('transit.flux_axis')}
          formatX={(v) => number(v, 1)}
          formatY={(v) => number(v, 4)}
        />
      </section>

      <section className="transit-section">
        <h2>{t('transit.periodogram')}</h2>
        <p className="muted small">{t('transit.periodogram_help')}</p>
        <Chart
          ariaLabel={t('transit.periodogram')}
          series={[{ label: t('transit.periodogram'), x: result.periodogram.x, y: relativePower, color: DATA_COLOR, kind: 'line' }]}
          markers={periodMarkers}
          xLog
          xLabel={t('transit.period_axis')}
          yLabel={t('transit.power_axis')}
          formatX={(v) => number(v, v < 1 ? 2 : 1)}
          formatY={(v) => number(v, 2)}
        />
        <p className="legend small">
          <span className="legend__swatch legend__swatch--dashed" style={{ borderColor: KNOWN_COLOR }} /> {t('transit.legend_known')}
          {result.candidates.map((_, i) => (
            <span key={i} className="legend__item">
              <span className="legend__swatch" style={{ background: CANDIDATE_COLORS[i % 3] }} /> {t('transit.candidate_n', { n: i + 1 })}
            </span>
          ))}
        </p>
      </section>

      <section className="transit-section">
        <h2>{t('transit.candidates')}</h2>
        {result.candidates.length === 0 && <p className="muted">{t('transit.no_candidates')}</p>}
        <div className="candidates">
          {result.candidates.map((c, i) => (
            <CandidateCard key={i} index={i} candidate={c} number={number} />
          ))}
        </div>
      </section>

      <p className="muted small">{t('transit.method_note', { date: new Date(result.generatedAt).toLocaleDateString(i18n.language) })}</p>
    </div>
  )
}

function CandidateCard({ index, candidate: c, number }: { index: number; candidate: TransitCandidate; number: (v: number, d?: number) => string }) {
  const { t } = useTranslation()
  const color = CANDIDATE_COLORS[index % 3]
  // Zoom on the transit: three durations either side, within the full phase range.
  const window = Math.min(0.5, Math.max(0.05, (3 * c.durationHours) / 24 / c.periodDays))

  const identity = c.matchedPlanet
    ? t('transit.matches', { planet: c.matchedPlanet })
    : c.secondaryEclipseOf
      ? t('transit.secondary_eclipse', { planet: c.secondaryEclipseOf })
      : t('transit.unknown_signal')

  return (
    <article className="candidate-card" style={{ borderTopColor: color }}>
      <h3>
        <span className="legend__swatch" style={{ background: color }} /> {t('transit.candidate_n', { n: index + 1 })}
        <span className="candidate-card__identity">{identity}</span>
      </h3>
      <Chart
        ariaLabel={t('transit.folded', { n: index + 1 })}
        series={[{ label: t('transit.folded', { n: index + 1 }), x: c.folded.x, y: c.folded.y, color, kind: 'dots' }]}
        xDomain={[-window, window]}
        height={200}
        xLabel={t('transit.phase_axis')}
        yLabel={t('transit.flux_axis')}
        formatX={(v) => number(v, 2)}
        formatY={(v) => number(v, 4)}
      />
      <table className="candidate-card__table">
        <tbody>
          <tr><th>{t('app.planet_period')}</th><td>{number(c.periodDays, 5)} {t('app.planet_days')}</td></tr>
          <tr><th>{t('transit.depth')}</th><td>{number(c.depthPpm)} ppm</td></tr>
          <tr><th>{t('transit.duration')}</th><td>{number(c.durationHours, 2)} h</td></tr>
          <tr><th>{t('transit.radius_ratio')}</th><td>{number(c.radiusRatio, 3)}</td></tr>
          <tr><th>SNR</th><td>{number(c.snr, 1)}</td></tr>
          <tr><th>{t('transit.odd_even')}</th><td>{number(c.oddEvenDepthRatio, 2)}</td></tr>
          <tr><th>{t('transit.first_transit')}</th><td>BTJD {number(c.epochBtjd, 3)}</td></tr>
        </tbody>
      </table>
      {c.warnings.length > 0 && (
        <ul className="candidate-card__warnings">
          {c.warnings.map((w) => <li key={w}>⚠ {w}</li>)}
        </ul>
      )}
    </article>
  )
}
