import * as THREE from 'three'
import type { Planet } from './types'

export const LIGHT_YEARS_PER_PARSEC = 3.26156

/** Radius of the sphere the star map draws the sky on, in scene units. */
export const SKY_RADIUS = 40

/** Position on the celestial sphere. RA in hours, Dec in degrees. */
export function raDecToXYZ(raHours: number, decDegrees: number, radius = SKY_RADIUS): [number, number, number] {
  const ra = (raHours / 24) * 2 * Math.PI
  const dec = (decDegrees / 180) * Math.PI
  return [radius * Math.cos(dec) * Math.cos(ra), radius * Math.sin(dec), radius * Math.cos(dec) * Math.sin(ra)]
}

/**
 * Position on an elliptical orbit centred on the star, for a semi-major axis (scene units),
 * eccentricity, inclination (degrees) and orbital angle (radians).
 */
export function getPositionFromAngle(
  semiMajorAxis: number,
  eccentricity: number,
  inclinationDeg: number,
  orbitalAngle: number,
): THREE.Vector3 {
  const a = semiMajorAxis
  const b = a * Math.sqrt(1 - eccentricity ** 2)
  const inclination = (inclinationDeg * Math.PI) / 180

  const x = a * Math.cos(orbitalAngle)
  const z = b * Math.sin(orbitalAngle)
  return new THREE.Vector3(x, -z * Math.sin(inclination), z * Math.cos(inclination))
}

/** Rounds to a number of decimals that keeps about four significant digits. */
export function significantDigits(num: number): number {
  if (num === 0) return 0
  const magnitude = Math.floor(Math.log10(Math.abs(num)))
  const decimals = Math.min(6, Math.max(0, 3 - magnitude))
  const factor = 10 ** decimals
  return Number((Math.round((num + Number.EPSILON) * factor) / factor).toFixed(decimals))
}

/** "1.234 AU", or the fallback when the value is unknown. */
export function formatValue(value: number | null | undefined, unit: string, unknown: string): string {
  return value == null ? unknown : `${significantDigits(value)} ${unit}`.trim()
}

/** Log scale from AU to scene units, so close-in and distant planets both fit. */
export function makeScaleDistanceFn(planets: Planet[], targetMax = 200) {
  const maxDistanceAU = Math.max(0, ...planets.map((p) => p.meanDistance ?? 0))
  return (distanceAU: number) => {
    if (!maxDistanceAU || distanceAU <= 0) return 0
    return (Math.log10(distanceAU + 1) / Math.log10(maxDistanceAU + 1)) * targetMax
  }
}

export function makeScaleRadiusFn(planets: Planet[], targetMax = 5, minSize = 0.8) {
  const maxRadius = Math.max(0, ...planets.map((p) => p.radius ?? 0))
  return (radiusEarthRadii: number) => (maxRadius ? (radiusEarthRadii / maxRadius) * targetMax + minSize : minSize)
}

export function scaleStarRadiusToUnits(radiusSolarRadii: number, maxPlanetDistance: number) {
  const base = Math.log10(radiusSolarRadii + 1) * 10
  const maxScaled = Math.log10(maxPlanetDistance + 1) * 0.2
  return Math.min(Math.max(base, 3), maxScaled)
}
