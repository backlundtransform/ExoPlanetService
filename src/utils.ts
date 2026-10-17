import * as THREE from 'three'
import type { SystemData } from './types'

/**
 * Beräknar planetens position på banan utifrån semi-major axis, excentricitet,
 * inklination och orbital vinkel.
 */
export function getOrbitalPosition(
  semiMajorAxis: number,
  eccentricity: number,
  inclinationDeg: number,
  orbitalAngle: number
): THREE.Vector3 {
  const a = semiMajorAxis
  const b = a * Math.sqrt(1 - eccentricity ** 2)
  const inclinationRad = (inclinationDeg * Math.PI) / 180

  const x = a * Math.cos(orbitalAngle)
  const z = b * Math.sin(orbitalAngle)

  const y = 0
  const rotatedY = y * Math.cos(inclinationRad) - z * Math.sin(inclinationRad)
  const rotatedZ = y * Math.sin(inclinationRad) + z * Math.cos(inclinationRad)

  return new THREE.Vector3(x, rotatedY, rotatedZ)
}


export const getPositionFromAngle = getOrbitalPosition

export function getAngleFromPosition(
  semiMajorAxis: number,
  mousePoint: THREE.Vector3
): number {

  const dx = mousePoint.x
  const dz = mousePoint.z

  const a = semiMajorAxis
  const e = 0 
  const b = a * Math.sqrt(1 - e ** 2)


  const angle = Math.atan2(dz / b, dx / a)
  return angle
}

export function significantDigits(num: number): number {
  if (num === 0) return 0;

  const abs = Math.abs(num);
  const log10 = Math.floor(Math.log10(abs));

  let decimaler;
  if (log10 >= 3) decimaler = 0;         
  else if (log10 >= 2) decimaler = 1;    
  else if (log10 >= 1) decimaler = 2;    
  else if (log10 >= 0) decimaler = 3;    
  else if (log10 >= -1) decimaler = 4;   
  else if (log10 >= -2) decimaler = 5;   
  else decimaler = 6;                    

  const faktor = Math.pow(10, decimaler);
  const avrundat = Math.round((num + Number.EPSILON) * faktor) / faktor;
  
  return Number(avrundat.toFixed(decimaler));
}

export function makeScaleDistanceFn(system: SystemData, targetMax = 200) {
  const maxDistanceAU = Math.max(...system.planets.map(p => p.meanDistance ?? 0))

  return (distanceAU: number) => {
    if (!maxDistanceAU || distanceAU <= 0) return 0

    
    const logPart = Math.log10(distanceAU + 1) / Math.log10(maxDistanceAU + 1)
    return logPart * targetMax
  }
}

export function makeScaleRadiusFn(system: SystemData, targetMax = 5, minSize = 0.8) {
  const maxRadius = Math.max(...system.planets.map(p => p.radius ?? 0))
  return (radiusEarthRadii: number) => {
    if (!maxRadius) return minSize
    return (radiusEarthRadii / maxRadius) * targetMax + minSize
  }
}

export function computeOrbitSpeed(periodDays: number, baseOrbitTime = 30) {
 
  const logPeriod = Math.log10(periodDays + 1)
  const speed = (2 * Math.PI) / (baseOrbitTime * logPeriod)
  return speed
}


export function scaleStarRadiusToUnits(radiusSolarRadii: number, maxPlanetDistanceAU: number){

  const base = Math.log10(radiusSolarRadii + 1) * 10
  const maxScaled = Math.log10(maxPlanetDistanceAU + 1) * 0.2
  return Math.min(Math.max(base, 3), maxScaled)

}

const time = (longitude: number): number => {
    const now = new Date()
    var utc_timestamp = Date.UTC(
        now.getUTCFullYear(),
        now.getUTCMonth(),
        now.getUTCDate(),
        now.getUTCHours(),
        now.getUTCMinutes(),
        now.getUTCSeconds(),
        now.getUTCMilliseconds(),
    )

    const start = Date.UTC(now.getUTCFullYear(), 0, 1)
    const diff = utc_timestamp - start
    const oneDay = 1000 * 60 * 60 * 24
    const days = Math.floor(diff / oneDay)

    let startime =
        0.0657098 * days -
        17.41409 +
        ((now.getUTCHours() * 60 + now.getUTCMinutes() + 4 * longitude) / 60) *
            1.002737909

    return (startime + 24) % 24
}
const hourangle = (latitude: number, altitude: number, azimuth: number) => {
    let numerator = Math.sin(toRadians(azimuth))
    let denominator =
        Math.cos(toRadians(azimuth)) * Math.sin(toRadians(latitude)) +
        Math.tan(toRadians(altitude)) * Math.cos(toRadians(latitude))

    return toDegrees(Math.atan2(numerator, denominator))
}

export const siderealtime = (longitude: number): string => {
    return timeformat(time(longitude))
}
export const dot_product = (
    a1: number,
    a2: number,
    a3: number,
    b1: number,
    b2: number,
    b3: number,
): number => {
    let numerator =
        Math.sqrt(a1 * a1 + a2 * a2 + a3 * a3) *
        Math.sqrt(b1 * b1 + b2 * b2 + b3 * b3)
    numerator = numerator === 0 ? 1 : numerator
    return toDegrees(Math.acos((a1 * b1 + a2 * b2 + a3 * b3) / numerator))
}
export const getdeclination = (
    latitude: number,
    altitude: number,
    azimuth: number,
): number => {
    let sindec =
        Math.sin(toRadians(latitude)) * Math.sin(toRadians(altitude)) -
        Math.cos(toRadians(latitude)) *
            Math.cos(toRadians(altitude)) *
            Math.cos(toRadians(azimuth))

    return toDegrees(Math.asin(sindec))
}

export const toRadians = (value: number) => {
    return (value * Math.PI) / 180
}
export const toDegrees = (value: number) => {
    return (value * 180) / Math.PI
}

export const azimuth_degree = (accelerometer: any, data: any) => {
    const b = calculatecrossproduct(accelerometer, { x: 0, y: 0, z: 1 })
    const a = calculatecrossproduct(accelerometer, data)
    const az = dot_product(a[0], a[1], a[2], b[0], b[1], b[2])
    return (az + 360) % 360
}

export const calculatecrossproduct = (a: any, b: any) => [
    a.y * b.z - a.z * b.y,
    a.z * b.x - a.x * b.z,
    a.x * b.y - a.y * b.x,
]

export const right_ascension = (
    longitude: number,
    latitude: number,
    altitude: number,
    azimuth: number,
) => {
    const angle = hourangle(latitude, altitude, azimuth)
    let right_ascension = time(longitude) * 15 + angle

    return (right_ascension / 15 + 24) % 24
}

const timeformat = (time: number) => {
    const decimal = Math.trunc(time)
    const fractional = time - decimal
    let numb = (fractional * 60).toString().substring(0, 2)
    if (fractional < 10 / 60) {
        numb = '0' + (fractional * 60).toString().substring(0, 1)
    }

    return decimal + ':' + numb
}