import {url} from "../service/getChart"
export interface RelatedContent {
    image: string
    description: string
    created: Date
    url:string
  }

export const GetRelatedAsync = async (name:string): Promise<Array<RelatedContent>> => {
    const array= await fetch(`${url}/api/GetRelatedContent?tag=${name}`,)
      .then(response => {
        return response.json()
      })
      .then(myJson => {
        return myJson
      })
  
    return array as Array<RelatedContent>
  }