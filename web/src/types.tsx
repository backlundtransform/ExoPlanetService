export interface StarApi {
  id: number;
  name: string;
  nameHD?: string | null;
  nameHIP?: string | null;
  constellation?: string | null;
  type?: string | null;
  mass?: number | null;
  radius?: number | null;
  teff?: number | null;
  luminosity?: number | null;
  feH?: number | null;
  age?: number | null;
  apparMag?: number | null;
  distance?: number | null;
  ra?: number | null;
  dec?: number | null;
  magfromPlanet?: number | null;
  sizefromPlanet?: number | null;
  habZoneMin?: number | null;
  habZoneMax?: number | null;
  habCat?: boolean | null;
  planets?: number | null;
  message?: string | null;
}

export interface StarData {
  id: number;
  name: string;
  nameHD?: string | null;
  nameHIP?: string | null;
  constellation?: string | null;
  type?: string | null;
  mass: number;
  radius?: number | null;
  teff?: number | null;
  luminosity?: number | null;
  feH?: number | null;
  age?: number | null;
  apparMag?: number | null;
  distance?: number | null;
  ra?: number | null;
  dec?: number | null;
  magfromPlanet?: number | null;
  sizefromPlanet?: number | null;
  habZoneMin?: number | null;
  habZoneMax?: number | null;
  habCat?: boolean | null;
  planets?: number | null;
  message?: string | null;
}

export interface HabitablePlanetApi {
  id: number;
  name: string;
  img?: string | null;
  period?: number | null;
  habType?: number | null;
  meanDistance?: number | null;
  distance?: number | null;
  esi?: number | null;
  radius: number;
  eccentricity?: number | null;
  coordinate?: {
    longitude: number;
    latitude: number;
  };
  star?: {
    name: string;
    type: string;
    mass: number;
    radius: number;
    temp?: number;
  };
}


export interface PlanetData {
    id: number
    name: string
    nameKepler: string | null
    nameKOI: string | null
    zoneClass: string | null
    massClass: string  | null,
    compositionClass: string  | null,
    atmosphereClass: string | null
    habitableClass: string | null
    minMass: number | null
    mass: number | null
    maxMass: number | null
    radius?: number | null;
    density: number | null
    gravity: number | null
    escVel: number | null
    sFluxMin: number | null
    sFluxMean: number | null
    sFluxMax: number | null
    teqMin: number | null
    teqMean: number | null
    teqMax: number | null
    tsMin: number | null
    tsMean: number | null
    tsMax: number | null
    surfPress: number | null
    mag: number | null
    apparSize: number | null
    period: number  
    semMajorAxis: number | null
    eccentricity: number | null
    meanDistance: number  
    inclination: number | null
    omega: number | null
    hzd: number | null
    hzc: number | null
    hza: number | null
    hzi: number | null
    sph: number | null
    intEsi: number | null
    surfEsi: number | null
    esi: number | null
    habitable: boolean
    habMoon: boolean | null
    confirmed: boolean | null
    disc_Method: string | null
    disc_Year: number | null
    message: string | null
  }
  
  export  interface SystemData {
    id: number,
    name: string,
    nameHD:string | null,
    nameHIP: string | null,
    constellation:string | null,
    type: string,
    mass: number,
    radius?: number | null,
    teff:number,
    luminosity:number,
    feH: number  | null,
    age: number  | null,
    apparMag: number,
    distance: number,
    ra:number | null,
    dec: number | null,
    magfromPlanet: number | null,
    sizefromPlanet: number | null,
    habZoneMin: number,
    habZoneMax: number,
    habCat: number | null,
    planets: PlanetData[],
    message:string | null
  }

  // --- API RESPONSE ---
export interface StarApi {
  id: number
  name: string
  nameHD?: string | null
  nameHIP?: string | null
  constellation?: string | null
  type?: string | null
  mass?: number | null
  radius?: number | null
  teff?: number | null
  luminosity?: number | null
  feH?: number | null
  age?: number | null
  apparMag?: number | null
  distance?: number | null
  ra?: number | null
  dec?: number | null
  habZoneMin?: number | null
  habZoneMax?: number | null
  message?: string | null
}

export interface PlanetApi {
  id: number
  name: string
  zoneClass?: string | null
  massClass?: string | null
  mass?: number | null
  radius?: number | null
  density?: number | null
  gravity?: number | null
  escVel?: number | null
  sFluxMean?: number | null
  teqMean?: number | null
  tsMean?: number | null
  period?: number | null
  semMajorAxis?: number | null
  eccentricity?: number | null
  meanDistance?: number | null
  inclination?: number | null
  omega?: number | null
  esi?: number | null
  habitable?: boolean | null
  disc_Method?: string | null
  disc_Year?: number | null
  message?: string | null
}