import { resource } from '../config/Resource'

export interface SearchPageState {
  mass: string
  comp: string
  atmos: string
  disc: string
  temp: string
  lightyears: string
  sort: string
  order: boolean
}

export interface filter {
  Key?: string
  MaxValue?: number
  MinValue?: number
  Name?: string
}

export interface planetcolor {
  jovian: string
  iron: string
  hotsuperearth: string
  hotjupiter: string
  hotstone: string
  coldsuperearth: string
  coldstone: string
  superearth: string
  stone: string
}
export interface Planet {
  name: string
  img: any
  period: number
  hzd: number
  hzc: number
  hza: number
  hzi: number
  type: string
  comp: number
  atmosphere: number
  meanDistance: number
  distance: number
  esi: number
  sph: number
  discYear: number
  discMethod: number
  radius?: number
  radiusEu?: number
  coordinate?: any
  starDistance: number
  eccentricity?: number
  star?: Star
  temp?: number
  tempMin?: number
  mass?: number
  density?: number
  gravity?: number
  surfacePressure?: number
  escapeVelocity?: number
  massType?: number
  tempMax?: number
  tempZone?: number
  hab?: boolean
  moons: true
}

export interface Star {
  name: string
  img: any
  type: string
  color?: number
  luminosity?: number
  magnitude?: number
  constellation?: number
  habZoneMin?: number
  habZoneMax?: number
  mass?: number
  age?: number
  temp?: number
  planets?: Array<Planet>
  noHabPlanets?: number
  noPlanets?: number
  radius?: number
  radiusSu?: number
}
export interface statistics {
  confirmedPlanets: number
  confirmedHabitablePlanets: number
  possibleHabitableMoons: number
  dateUpdated: Date
}
export const GetStatisticsAsync = async (): Promise<statistics> => {
  const stat = await fetch(`../api/Statistics`)
    .then(response => {
      return response.json()
    })
    .then(myJson => {
      return myJson
    })

  return stat as statistics
}

export const GetPlanetListAsync = (filter: filter, top: number) => {
  let page = top / 30

  const hab = filter && filter.Key === 'Hab'
  const moon = filter && filter.Key === 'Moons'

  const type = !hab && !moon && filter && filter.Key ? filter.Key : ''
  const key =
    type !== '' && !hab && !moon && filter && filter.Name ? filter.Name : ''

    const name = type === '' &&filter && filter.Name ? filter.Name : ''

  const planetList = fetch(
    `../api/ExoSolarSystems/GetPaginatedPlanets?page=${page -
      1}&hab=${hab}&moon=${moon}&type=${type}&key=${key}
    &name=${name}
    `
  )
    .then(response => {
      return response.json()
    })
    .then(
      (myJson): Array<Planet> => {
        return myJson as Array<Planet>
      }
    )

  return planetList
}

export const GetPlanetAsync = (name: string) => {
  const planetList = fetch(
    `../api/ExoSolarSystems/ExoPlanets?%24filter=Name eq '${encodeURIComponent(
      name
    )}'`
  )
    .then(response => {
      return response.json()
    })
    .then(
      (myJson): Planet => {
        return myJson[0] as Planet
      }
    )

  return planetList
}

export const GetHabitablePlanets = async () => {
  const planetList = await fetch(`../api/ExoSolarSystems/GetHabitablePlanets`)
    .then(response => {
      return response.json()
    })
    .then(myJson => {
      return myJson
    })

  return planetList
}
