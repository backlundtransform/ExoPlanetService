export interface TransitTimeserie {
    label: string
    index: number,
    value: number,
  }

export const GetTransitAsync = async (ra:number,  dec:number, radius:number): Promise<Array<TransitTimeserie>> => {
    const transit = await fetch(`../api/Chart/Mast?ra=${ra}&dec=${dec}&radius=${radius}`)
      .then(response => {
        return response.json()
      })
      .then(myJson => {
        return myJson
      })
  
    return transit as Array<TransitTimeserie>
  }