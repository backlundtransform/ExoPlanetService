import {url} from "../service/getChart"

export const GetConstellationsLines=  async () => {

    const lines=  await fetch(`${url}/api/Maps/ConstellationsLines`)
    .then((response) => {
      return response.json();
    })
    .then((myJson) =>  {
  

      return myJson
  
    });
  
    return  lines;
   }

   export const GetStarsMarkers=  async () => {

    const lines=  await fetch(`${url}/api/Maps/StarMarkers`)
    .then((response) => {
      return response.json();
    })
    .then((myJson) =>  {
  
  
      return myJson
  
    });
  
    return  lines;
   }
  
  