
import { Capacitor } from '@capacitor/core';
import { Http } from '@capacitor-community/http';
import type { StarApi, StarData, HabitablePlanetApi } from './types';

const API_URL = "/api";

export async function fetchData(url: string) {

  try {
    if (Capacitor.getPlatform() === 'web') {
      const res = await fetch(url);
      
      const data = await res.json();
      return data;
    }

    const res = await Http.request({
      method: 'GET',
      url: url.startsWith('http') ? url : `https://exoplanethunter.com${url}`,
      params: {},       
      headers: {},
    });

    return typeof res.data === 'string' ? JSON.parse(res.data) : res.data;
  } catch (err) {
    console.error('Fetch error:', err);
    throw err;
  }
}

// Mapper för stjärnor
export function mapStar(api: any): StarData {
  return {
    id: api.id,
    name: api.name,
    nameHD: api.nameHD ?? null,
    nameHIP: api.nameHIP ?? null,
    constellation: api.constellation ?? null,
    type: api.type ?? null,
    mass: api.mass ?? 1,
    radius: api.radius ?? 1,
    teff: api.teff ?? null,
    luminosity: api.luminosity ?? null,
    feH: api.feH ?? null,
    age: api.age ?? null,
    apparMag: api.apparMag ?? null,
    distance: api.distance ?? null,
    ra: api.ra ?? null,
    dec: api.dec ?? null,
    magfromPlanet: api.magfromPlanet ?? null,
    sizefromPlanet: api.sizefromPlanet ?? null,
    habZoneMin: api.habZoneMin ?? null,
    habZoneMax: api.habZoneMax ?? null,
    habCat: api.habCat ?? null,
    planets: api.planets ?? null,
    message: api.message ?? null
  };
}

// Mapper för planeter
export function mapPlanet(api: any) {
  return {
    id: api.id,
    name: api.name,
    nameKepler: api.nameKepler ?? null,
    nameKOI: api.nameKOI ?? null,
    zoneClass: api.zoneClass ?? null,
    massClass: api.massClass ?? null,
    compositionClass: api.compositionClass ?? null,
    atmosphereClass: api.atmosphereClass ?? null,
    habitableClass: api.habitableClass ?? null,
    minMass: api.minMass ?? null,
    mass: api.mass ?? null,
    maxMass: api.maxMass ?? null,
    radius: api.radius ?? null,
    density: api.density ?? null,
    gravity: api.gravity ?? null,
    escVel: api.escVel ?? null,
    sFluxMin: api.sFluxMin ?? null,
    sFluxMean: api.sFluxMean ?? null,
    sFluxMax: api.sFluxMax ?? null,
    teqMin: api.teqMin ?? null,
    teqMean: api.teqMean ?? null,
    teqMax: api.teqMax ?? null,
    tsMin: api.tsMin ?? null,
    tsMean: api.tsMean ?? null,
    tsMax: api.tsMax ?? null,
    surfPress: api.surfPress ?? null,
    mag: api.mag ?? null,
    apparSize: api.apparSize ?? null,
    period: api.period ?? 1,
    semMajorAxis: api.semMajorAxis ?? null,
    meanDistance: api.meanDistance ?? api.semMajorAxis ?? 0.1,
    eccentricity: api.eccentricity ?? 0,
    inclination: api.inclination ?? 0,
    omega: api.omega ?? null,
    hzd: api.hzd ?? null,
    hzc: api.hzc ?? null,
    hza: api.hza ?? null,
    hzi: api.hzi ?? null,
    sph: api.sph ?? null,
    intEsi: api.intEsi ?? null,
    surfEsi: api.surfEsi ?? null,
    esi: api.esi ?? null,
    habitable: api.habitable ?? false,
    habMoon: api.habMoon ?? null,
    confirmed: api.confirmed ?? null,
    disc_Method: api.disc_Method ?? null,
    disc_Year: api.disc_Year ?? null,
    message: api.message ?? null
  };
}


export function mapSystem(star: StarApi, planets: any) {


  return {
    ...mapStar(star),
    planets: planets.planets.map(mapPlanet)
  };
}

// API-anrop
export async function fetchStars(): Promise<StarData[]> {
  const data: StarApi[] = await fetchData(`${API_URL}/Stars`);
  return data.map(mapStar);
}

export async function fetchHabitablePlanets(): Promise<HabitablePlanetApi[]> {
  return fetchData(`${API_URL}/ExoSolarSystems/GetHabitablePlanets`);
}
export async function fetchSystem(starId: number) {
  
  const star: StarApi = await fetchData(`${API_URL}/Stars/${starId}`);

  const planets: any[] = await fetchData(`${API_URL}/Stars/${starId}/planets`);

  return mapSystem(star, planets);
}