import { Capacitor } from '@capacitor/core'
import type { HabitablePlanet, Star, StarPlanets, StarSystem } from './types'

// In the browser the app is served by the API itself; the Android app calls production.
// On Android, fetch() goes through CapacitorHttp (see capacitor.config.ts), so CORS does not apply.
const API_BASE = Capacitor.isNativePlatform() ? 'https://exoplanethunter.com/api' : '/api'

export class ApiError extends Error {
  readonly status: number

  constructor(status: number, message: string) {
    super(message)
    this.status = status
  }
}

async function get<T>(path: string, signal?: AbortSignal): Promise<T> {
  const response = await fetch(`${API_BASE}${path}`, { signal, headers: { Accept: 'application/json' } })
  if (!response.ok) {
    throw new ApiError(response.status, `${response.status} ${response.statusText}`)
  }
  return (await response.json()) as T
}

export const fetchStars = (signal?: AbortSignal) => get<Star[]>('/Stars', signal)

export const fetchHabitablePlanets = (signal?: AbortSignal) =>
  get<HabitablePlanet[]>('/ExoSolarSystems/GetHabitablePlanets', signal)

export async function fetchSystem(starId: number, signal?: AbortSignal): Promise<StarSystem> {
  const [star, planets] = await Promise.all([
    get<Star>(`/Stars/${starId}`, signal),
    get<StarPlanets>(`/Stars/${starId}/planets`, signal),
  ])
  return { star, planets: planets.planets }
}
