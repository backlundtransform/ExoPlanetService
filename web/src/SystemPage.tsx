import { Suspense, useEffect, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router'
import { useTranslation } from 'react-i18next'
import { ApiError, fetchSystem } from './api'
import { ExoplanetSystem } from './ExoplanetSystem'
import type { StarSystem } from './types'

type State =
  | { status: 'loading' }
  | { status: 'ready'; system: StarSystem }
  | { status: 'not-found' }
  | { status: 'error'; message: string }

/** /star/:id — a shareable link to one star system. */
export default function SystemPage() {
  const { t } = useTranslation()
  const navigate = useNavigate()
  const { id } = useParams()
  const starId = Number(id)
  const [state, setState] = useState<State>({ status: 'loading' })
  const [attempt, setAttempt] = useState(0)

  useEffect(() => {
    if (!Number.isInteger(starId)) {
      setState({ status: 'not-found' })
      return
    }

    const controller = new AbortController()
    setState({ status: 'loading' })
    fetchSystem(starId, controller.signal)
      .then((system) => setState({ status: 'ready', system }))
      .catch((error: unknown) => {
        if (controller.signal.aborted) return
        if (error instanceof ApiError && error.status === 404) setState({ status: 'not-found' })
        else setState({ status: 'error', message: error instanceof Error ? error.message : String(error) })
      })
    return () => controller.abort()
  }, [starId, attempt])

  useEffect(() => {
    document.title = state.status === 'ready' ? `${state.system.star.name} · Exoplanet Hunter` : 'Exoplanet Hunter'
  }, [state])

  if (state.status === 'ready') {
    return (
      <Suspense fallback={<p className="page-message">{t('app.loading')}</p>}>
        <ExoplanetSystem system={state.system} onBack={() => navigate('/')} />
      </Suspense>
    )
  }

  return (
    <div className="page-message">
      {state.status === 'loading' && <p>{t('app.loading')}</p>}
      {state.status === 'not-found' && <p>{t('app.not_found')}</p>}
      {state.status === 'error' && (
        <>
          <p className="error">{t('app.error', { message: state.message })}</p>
          <button onClick={() => setAttempt((n) => n + 1)}>{t('app.retry')}</button>
        </>
      )}
      <p>
        <Link to="/">← {t('app.back_to_map')}</Link>
      </p>
    </div>
  )
}
