import { useEffect, useState } from 'react'
import { Link } from 'react-router'
import { useTranslation } from 'react-i18next'
import { fetchTransitIndex } from '../api'
import type { TransitIndexEntry } from '../types'

/** /transit — stars with a precomputed TESS transit search. */
export default function TransitListPage() {
  const { t } = useTranslation()
  const [entries, setEntries] = useState<TransitIndexEntry[] | null>(null)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    document.title = 'Transit Lab · Exoplanet Hunter'
    const controller = new AbortController()
    fetchTransitIndex(controller.signal)
      .then(setEntries)
      .catch((e: unknown) => !controller.signal.aborted && setError(e instanceof Error ? e.message : String(e)))
    return () => controller.abort()
  }, [])

  return (
    <div className="transit-page">
      <header className="transit-page__header">
        <Link to="/">← {t('app.back_to_map')}</Link>
        <h1>{t('transit.title')}</h1>
        <p className="muted">{t('transit.intro')}</p>
      </header>

      {error && <p className="error">{t('app.error', { message: error })}</p>}
      {!entries && !error && <p className="muted">{t('app.loading')}</p>}
      {entries && (
        <table className="transit-index">
          <thead>
            <tr>
              <th>{t('transit.star')}</th>
              <th>{t('transit.found_of_known')}</th>
              <th>{t('transit.candidates')}</th>
              <th>{t('transit.sectors')}</th>
            </tr>
          </thead>
          <tbody>
            {entries.map((e) => (
              <tr key={e.starId}>
                <td><Link to={`/transit/${e.starId}`}>{e.starName}</Link></td>
                <td>{e.knownPlanetsFound} / {e.knownPlanets}</td>
                <td>{e.candidates}</td>
                <td>{e.sectors.join(', ')}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  )
}
