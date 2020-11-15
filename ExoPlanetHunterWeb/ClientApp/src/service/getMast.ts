export interface TransitTimeserie {
    label: string
    index: number,
    value: number,
  }

export const GetTransitAsync = async (): Promise<Array<TransitTimeserie>> => {
    const transit = await fetch(`../api/Chart/Mast`)
      .then(response => {
        return response.json()
      })
      .then(myJson => {
        return myJson
      })
  
    return transit as Array<TransitTimeserie>
  }