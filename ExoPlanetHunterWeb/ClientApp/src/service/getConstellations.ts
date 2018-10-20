export const GetConstellationsLines=  async () => {

    const lines=  await fetch(`http://exoplanets.azurewebsites.net/api/Maps/ConstellationsLines`)
    .then((response) => {
      return response.json();
    })
    .then((myJson) =>  {
  
  
      return myJson
  
    });
  
    return  lines;
   }

   export const GetStarsMarkers=  async () => {

    const lines=  await fetch(`http://exoplanets.azurewebsites.net/api/Maps/StarMarkers`)
    .then((response) => {
      return response.json();
    })
    .then((myJson) =>  {
  
  
      return myJson
  
    });
  
    return  lines;
   }
  
  