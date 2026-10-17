import { useEffect, useState } from 'react'
import { fetchHabitablePlanets, fetchStars } from '../api'
import type { Star } from '../types'

export interface Catalog {
  stars: Star[]
  /** Names of stars with at least one potentially habitable planet. */
  habitableStars: ReadonlySet<string>
}

// Loaded once per page and shared between views, so returning from a system to the
// map does not refetch ~5,000 stars.
let catalogPromise: Promise<Catalog> | null = null

function loadCatalog(): Promise<Catalog> {
  catalogPromise ??= Promise.all([fetchStars(), fetchHabitablePlanets()])
    .then(([stars, habitable]) => ({
      stars,
      habitableStars: new Set(habitable.map((p) => p.star?.name).filter((n): n is string => !!n)),
    }))
    .catch((error: unknown) => {
      catalogPromise = null // allow a retry
      throw error
    })
  return catalogPromise
}

export function useCatalog() {
  const [catalog, setCatalog] = useState<Catalog | null>(null)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    let active = true
    loadCatalog()
      .then((c) => active && setCatalog(c))
      .catch((e: unknown) => active && setError(e instanceof Error ? e.message : String(e)))
    return () => {
      active = false
    }
  }, [])

  return { catalog, error }
}
