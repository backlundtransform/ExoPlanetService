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

export const GetPlanetListAsync = (
  filter: filter,
  filterstate: any,
  top: number
) => {
  let skip = top - 30

  let filterstring = '%24filter=Message eq null'
  let orderby = 'DiscYear%20desc'
  if (filter != null) {
    if (filter.Key === 'Hab' || filter.Key === 'Moons') {
      filterstring = `${filterstring} and ${filter.Key}%20eq%20true`
    }
    if (filter.Key === 'Temp' || filter.Key === 'Esi' || filter.Key === 'Sph') {
      filterstring = `${filterstring} and ${filter.Key} gt ${
        filter.MinValue
      } and ${filter.Key} lt  ${filter.MaxValue}`
    }

    if (filter.Name != null) {
      skip = 0
      top = 10
      filterstring = `${filterstring} and indexof(Name, '${encodeURIComponent(
        filter.Name
      )}') gt -1`
    }
  }

  if (filterstate.filter !== undefined) {
    const currentfilter = filterstate.filter.filter
    const compindex = resource.compsearch.indexOf(currentfilter.comp)
    const massindex = resource.masssearch.indexOf(currentfilter.mass)
    const atmosindex = resource.atmossearch.indexOf(currentfilter.atmos)
    const discindex = resource.discsearch.indexOf(currentfilter.disc)
    const tempindex = resource.tempsearch.indexOf(currentfilter.temp)
    const orderindex = resource.sortsearch.indexOf(currentfilter.sort)
    const lightyearsindex = resource.lightyearsearch.indexOf(
      currentfilter.lightyears
    )
    if (compindex > -1) {
      filterstring = `${filterstring} and Comp eq ${compindex}`
    }

    if (massindex > -1) {
      filterstring = `${filterstring} and MassType eq ${massindex}`
    }

    if (atmosindex > -1) {
      filterstring = `${filterstring} and Atmosphere eq ${atmosindex}`
    }
    if (tempindex > -1) {
      filterstring = `${filterstring} and TempZone eq ${tempindex}`
    }

    switch (lightyearsindex) {
      case 0:
        filterstring = `${filterstring} and Distance lt ${20}`
        break
      case 1:
        filterstring = `${filterstring} and Distance lt ${200}`
        break
      case 2:
        filterstring = `${filterstring} and Distance lt ${2000}`
        break
      case 3:
        filterstring = `${filterstring} and Distance lt ${20000}`
        break
    }

    if (orderindex > -1) {
      const order = currentfilter.order ? 'desc' : 'asc'
      if (orderindex === 0) {
        orderby = 'Distance'
      }

      if (orderindex === 1) {
        orderby = 'Esi'
      }

      if (orderindex === 2) {
        orderby = 'Mass'
      }

      if (orderindex === 3) {
        orderby = 'DiscYear'
      }
      orderby = `${orderby}%20${order}`
    }
  }
  const planetList = fetch(
    `../api/ExoSolarSystems/ExoPlanets?${filterstring}&%24top=${top}&%24skip=${skip}&%24orderby=${orderby}`
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
