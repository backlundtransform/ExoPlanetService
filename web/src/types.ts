// Shapes of the ExoplanetHunter API responses (see docs/api-compatibility.md).
// Units: star mass/radius/luminosity in solar units, distance in parsecs, RA in hours,
// Dec in degrees; planet mass/radius in Earth units, distances in AU, period in days.

export interface Star {
  id: number
  name: string
  nameHD: string | null
  nameHIP: string | null
  constellation: string | null
  type: string | null
  mass: number | null
  radius: number | null
  teff: number | null
  luminosity: number | null
  feH: number | null
  age: number | null
  apparMag: number | null
  distance: number | null
  ra: number | null
  dec: number | null
  habZoneMin: number | null
  habZoneMax: number | null
  /** Number of planets. */
  planets: number | null
}

export interface Planet {
  id: number
  name: string
  zoneClass: string | null
  massClass: string | null
  compositionClass: string | null
  mass: number | null
  radius: number | null
  density: number | null
  gravity: number | null
  escVel: number | null
  sFluxMean: number | null
  teqMean: number | null
  tsMean: number | null
  period: number | null
  semMajorAxis: number | null
  eccentricity: number | null
  meanDistance: number | null
  inclination: number | null
  esi: number | null
  habitable: boolean | null
  disc_Method: string | null
  disc_Year: number | null
}

export interface StarPlanets {
  id: number
  name: string
  planets: Planet[]
}

/** Item of /api/ExoSolarSystems/GetHabitablePlanets. */
export interface HabitablePlanet {
  name: string
  star: { name: string } | null
}

export interface StarSystem {
  star: Star
  planets: Planet[]
}

// --- Transit Lab (/api/v2/Transits) ---
// Times are BTJD (BJD − 2457000), flux is normalised to 1, depths in ppm, durations in hours.

export interface TransitIndexEntry {
  starId: number
  starName: string
  ticId: number
  sectors: number[]
  candidates: number
  knownPlanetsFound: number
  knownPlanets: number
  generatedAt: string
}

/** Two equally long arrays, e.g. time and flux, or period and power. */
export interface Series {
  x: number[]
  y: number[]
}

export interface TransitCandidate {
  periodDays: number
  epochBtjd: number
  depthPpm: number
  durationHours: number
  radiusRatio: number
  snr: number
  oddEvenDepthRatio: number
  score: number
  disposition: string
  warnings: string[]
  /** Phase −0.5…0.5 (transit at 0) against flux. */
  folded: Series
  matchedPlanet: string | null
  secondaryEclipseOf: string | null
}

export type KnownPlanetStatus = 'found' | 'found-at-harmonic' | 'not-found' | 'out-of-range' | 'no-period'

export interface KnownPlanetMatch {
  name: string
  periodDays: number | null
  status: KnownPlanetStatus
  candidateIndex: number | null
}

export interface TransitResult {
  starId: number
  starName: string
  ticId: number
  mission: string
  sectors: number[]
  points: number
  generatedAt: string
  lightCurve: Series
  periodogram: Series
  candidates: TransitCandidate[]
  knownPlanets: KnownPlanetMatch[]
}
