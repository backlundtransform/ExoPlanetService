
export interface RelatedContent {
    image: string
    description: string
    created: Date
    url:string
  }

export const GetRelatedAsync = async (name:string): Promise<Array<RelatedContent>> => {
    const array= await fetch(`../api/GetRelatedContent?tag=${name}`)
      .then(response => {
        return response.json()
      })
      .then(myJson => {
        return myJson
      })
  
    return array as Array<RelatedContent>
  }