import { Star } from './getPlanets'
export interface HertzsprungRussell {
  title: string
  type: string
  lum: number
  color: string
  size: number
  temp:number
  constellation: string
}
const getData = async (uri: string): Promise<any> => {
  const data = await fetch(uri)
    .then(response => {
      return response.json()
    })
    .then(myJson => {
      return myJson
    })

  return data
}

export const getHertzsprungRussell = async (
): Promise<Array<HertzsprungRussell>> => {
  const hertzsprungRussell  = await getData(`../api/Chart/HertzsprungRussell`)
  return hertzsprungRussell  as Promise<Array<HertzsprungRussell>>
}
