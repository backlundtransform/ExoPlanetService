import { useEffect, useState } from 'react'
import { fetchTransitIndex } from '../api'
import type { TransitIndexEntry } from '../types'

// Loaded once per page: small, and shared by the system view and the Transit Lab list.
let indexPromise: Promise<TransitIndexEntry[]> | null = null

/** Stars with a Transit Lab analysis; null while loading or when the request failed. */
export function useTransitIndex() {
  const [index, setIndex] = useState<TransitIndexEntry[] | null>(null)

  useEffect(() => {
    let active = true
    indexPromise ??= fetchTransitIndex().catch((error: unknown) => {
      indexPromise = null
      throw error
    })
    indexPromise.then((entries) => active && setIndex(entries)).catch(() => {})
    return () => {
      active = false
    }
  }, [])

  return index
}
