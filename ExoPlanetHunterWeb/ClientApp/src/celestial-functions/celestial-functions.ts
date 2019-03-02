//https://www.aa.quae.nl/en/reken/hemelpositie.html#table2
//Testing for Jupiter
const julianDayZero = 2451545
const angle = 0.9856076686
const ecliptic= 23.4397* Math.PI/180

const julianDay =() =>{
  var date = new Date()
  var time = date.getTime()
return  Math.floor(time / 86400000 + 2440587.5 - (date.getTimezoneOffset()/1440))
}

export const getMeanAnomaly = (
  julianDay: number,
  meanAnomalyZero: number,
  distance: number
) =>
  ((meanAnomalyZero +
    ((julianDay - julianDayZero) * angle) / Math.pow(distance, 3 / 2)) *
    Math.PI) /
  180

export const getTrueAnomaly = (meanAnomaly: number, eccentricity: number) => {
  return (
    meanAnomaly +
    (2 * eccentricity - Math.pow(eccentricity, 3) / 4) * Math.sin(meanAnomaly) +
    (5 * Math.pow(eccentricity, 2) * Math.sin(2 * meanAnomaly)) / 4 +
    (13 * Math.pow(eccentricity, 3) * Math.sin(3 * meanAnomaly)) / 12
  )
}

export const getDistanceFromSun = (
  trueAnomaly: number,
  eccentricity: number,
  distance: number
) =>
  (distance * (1 - Math.pow(eccentricity, 2))) /
  (1 + eccentricity * Math.cos(trueAnomaly))

export const getHeliocentricCoordinates = (
  sunDistance: number,
  trueAnomaly: number,
  eclipticlongitude: number,
  argumentOmega: number,
  inclination: number
): Array<number> => {
  const x =
    sunDistance *
    (Math.cos(eclipticlongitude) * Math.cos(argumentOmega +trueAnomaly) -
      Math.sin(eclipticlongitude) *
        Math.cos(inclination) *
        Math.sin(argumentOmega + trueAnomaly))
  const y =
    sunDistance *
    (Math.sin(eclipticlongitude) * Math.cos(argumentOmega + trueAnomaly) +
      Math.cos(eclipticlongitude) *
        Math.cos(inclination) *
        Math.sin(argumentOmega + trueAnomaly))
  const z =
    sunDistance *
    (Math.sin(inclination) * Math.sin(argumentOmega + trueAnomaly))
  return [x, y, z]
}

export const getGeocentricCoordinates = (
  heliocoordplanet: Array<number>,
  heliocoordearth: Array<number>
): Array<number> => {
  const [px, py, pz] = heliocoordplanet
  const [ex, ey, ez] = heliocoordearth
  return [px - ex, py - ey, pz - ez]
}

export const getGeocentricLongLat = (
   geocoord: Array<number>,

  ): Array<number> => {
    const [x, y, z] = geocoord
    const delta =Math.sqrt(Math.pow(x, 2)+Math.pow(y, 2)+Math.pow(z, 2))
    const longitude=Math.atan2(y,x)
    const latitude=Math.asin(z/delta)
    return [longitude,latitude]
  }

  export const getRaDeg = (
    geolongLat: Array<number>,
 
   ): Array<number> => {
     const [longitude,latitude] = geolongLat
     const rightAscension =Math.atan2(Math.sin(longitude)*Math.cos(ecliptic)-Math.tan(latitude)*Math.sin(ecliptic),Math.cos(longitude))
     const declination=Math.asin(Math.sin(latitude)*Math.cos(ecliptic)+Math.cos(latitude)*Math.sin(longitude)*Math.sin(ecliptic))
  
     return [rightAscension,declination]
   }

  export const testfunction =(
   
  ) => {
    const day=julianDay()
    console.log(2458545-day)
     const jupitermeanAnomaly = getMeanAnomaly(2453006,20.020,
      5.20260
      )
    console.log(jupitermeanAnomaly*180/Math.PI)
    const jupitertrueAnomaly =getTrueAnomaly(jupitermeanAnomaly,0.04849)
    console.log(jupitertrueAnomaly*180/Math.PI)
    const jupiterdistancefromsun = getDistanceFromSun(jupitertrueAnomaly,0.04849,5.20260)
    console.log(jupiterdistancefromsun)
    const jupiterHeliocoord = getHeliocentricCoordinates(
      jupiterdistancefromsun,
      jupitertrueAnomaly,
      100.464*Math.PI/180,
      273.867*Math.PI/180,
      1.303*Math.PI/180
    )
    console.log(jupiterHeliocoord)
 const earthmeanAnomaly = getMeanAnomaly(2453006,357.529,
     1
      )
    console.log(earthmeanAnomaly*180/Math.PI)
    const earthtrueAnomaly =getTrueAnomaly(earthmeanAnomaly,0.01671	)
    console.log(earthtrueAnomaly*180/Math.PI)
    const earthdistancefromsun = getDistanceFromSun(earthtrueAnomaly,0.01671,1)
    console.log(earthdistancefromsun)
    const earthHeliocoord = getHeliocentricCoordinates(
      earthdistancefromsun,
      earthtrueAnomaly,
      174.873*Math.PI/180,
      288.064*Math.PI/180,
    0
    )
    console.log(earthHeliocoord)
    const jupiterGeocoord =getGeocentricCoordinates(jupiterHeliocoord,earthHeliocoord)
    console.log(jupiterGeocoord)
    const [long,lat]= getGeocentricLongLat(jupiterGeocoord)
    console.log(long*180/Math.PI,lat*180/Math.PI)
    const [ra,dec]= getRaDeg(getGeocentricLongLat(jupiterGeocoord)) 
    console.log(ra*180/Math.PI,dec*180/Math.PI)

   }
  
  
