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
