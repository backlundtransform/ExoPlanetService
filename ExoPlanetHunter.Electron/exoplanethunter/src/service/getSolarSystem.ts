import { Star } from './getPlanets'
import {url} from "../service/getChart"
const getData = async (uri: string): Promise<any> => {
  const SolarSystems = await fetch(uri)
    .then(response => {
      return response.json()
    })
    .then(myJson => {
      return myJson
    })

  return SolarSystems
}

export const getStarSize = (star: Star): number => {
  if (star.luminosity < 3) {
    return 160
  }

  if (star.luminosity < 6) {
    return 120
  }

  return 80
}

export const getSolarSystem = async (star: Star): Promise<Star> => {
  const SolarSystems = await getData(
    `${url}/api/ExoSolarSystems/GetExoSolarSystemByName?name=${encodeURIComponent(
      star.name
    )}`
  )
  return SolarSystems as Promise<Star>
}

export const ConstellationSolarSystems = async (
  constellation: number, page:number
): Promise<Array<Star>> => {
  const SolarSystems = await getData(
    `${url}/api/ExoSolarSystems/GetSolarSystemPerConstellation?constellation=${constellation}&page=${page}`
  )

  return SolarSystems as Promise<Array<Star>>
}
