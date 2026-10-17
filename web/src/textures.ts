import type { Planet } from './types'

// Textures in public/textures, named {zone}_{massClass}_{composition}.webp.
const ZONES = ['hot', 'warm', 'cold'] as const
const COMPOSITION: Record<string, string> = {
  mercurian: 'rocky-iron',
  subterran: 'rocky-water',
  terran: 'rocky-water',
  superterran: 'rocky-water',
  neptunian: 'gas',
  jovian: 'gas',
}

export const textureUrl = (name: string) => `${import.meta.env.BASE_URL}textures/${name}.webp`

export const SUN_TEXTURE = textureUrl('sun')

/**
 * Texture for a planet. Every zone/mass-class pair has a texture; unknown zones are
 * drawn as warm and unknown mass classes as Jovian (most planets without data are giants).
 */
export function planetTexture(planet: Pick<Planet, 'zoneClass' | 'massClass'>): string {
  const zone = ZONES.find((z) => z === planet.zoneClass?.toLowerCase()) ?? 'warm'
  const massClass = planet.massClass?.toLowerCase() ?? ''
  const mass = massClass in COMPOSITION ? massClass : 'jovian'
  return textureUrl(`${zone}_${mass}_${COMPOSITION[mass]}`)
}
