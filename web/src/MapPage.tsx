import { useDeferredValue, useMemo, useState } from 'react'
import { useNavigate } from 'react-router'
import { useTranslation } from 'react-i18next'
import StarMap from './StarMap'
import { useCatalog } from './hooks/useCatalog'

// Rendering every star as a button is slow on phones; search narrows the list.
const MAX_LISTED = 300
const NO_STARS = new Set<string>()

export default function MapPage() {
  const { t } = useTranslation()
  const navigate = useNavigate()
  const { catalog, error } = useCatalog()
  const [query, setQuery] = useState('')
  const [habitableOnly, setHabitableOnly] = useState(false)
  const [sidebarOpen, setSidebarOpen] = useState(false)
  const deferredQuery = useDeferredValue(query)

  const stars = useMemo(() => catalog?.stars ?? [], [catalog])
  const habitableStars = catalog?.habitableStars ?? NO_STARS

  const filtered = useMemo(() => {
    const q = deferredQuery.trim().toLowerCase()
    return stars
      .filter((s) => !q || s.name.toLowerCase().includes(q))
      .filter((s) => !habitableOnly || s.name === 'Sun' || catalog?.habitableStars.has(s.name))
      .sort((a, b) => a.name.localeCompare(b.name))
  }, [stars, deferredQuery, habitableOnly, catalog])

  const openStar = (id: number) => {
    setSidebarOpen(false)
    navigate(`/star/${id}`)
  }

  return (
    <div className="app-container">
      <button className="sidebar-toggle" onClick={() => setSidebarOpen(!sidebarOpen)} aria-label="Menu">
        ☰
      </button>

      <aside className={`sidebar ${sidebarOpen ? 'open' : ''}`}>
        <h1 className="sidebar__title">{t('app.title')}</h1>

        <input
          className="sidebar__search"
          type="search"
          placeholder={t('app.search_placeholder')}
          value={query}
          onChange={(e) => setQuery(e.target.value)}
        />

        <label className="sidebar__filter">
          <input type="checkbox" checked={habitableOnly} onChange={(e) => setHabitableOnly(e.target.checked)} />
          {t('app.show_habitable_only')} 🌍
        </label>

        {!catalog && !error && <p className="muted">{t('app.loading')}</p>}
        {error && <p className="error">{t('app.error', { message: error })}</p>}
        {catalog && (
          <p className="muted small">{t('app.showing_count', { count: filtered.length, total: stars.length })}</p>
        )}

        <ul className="star-list">
          {filtered.slice(0, MAX_LISTED).map((star) => {
            const habitable = star.name === 'Sun' || habitableStars.has(star.name)
            return (
              <li key={star.id}>
                <button className="star-list__item" onClick={() => openStar(star.id)}>
                  <span>
                    {habitable && '🌍 '}
                    {star.name}
                  </span>
                  {habitable && <span className="habitable-dot" title={t('app.habitable_tooltip')} />}
                </button>
              </li>
            )
          })}
        </ul>
      </aside>

      {sidebarOpen && <div className="overlay" onClick={() => setSidebarOpen(false)} />}

      <main className="starmap-container">
        <StarMap stars={stars} habitableStars={habitableStars} onSelectStar={openStar} />
      </main>
    </div>
  )
}
