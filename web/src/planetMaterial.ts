import { useTexture } from '@react-three/drei'
import { useMemo } from 'react'
import * as THREE from 'three'
import { planetTexture } from './textures'
import type { Planet } from './types'

export function usePlanetMaterial(planet: Planet) {
  const textureMap = useTexture(planetTexture(planet))
  const massClass = planet.massClass?.toLowerCase() ?? ''
  const esi = planet.esi ?? 0
  const gaseous = massClass === 'neptunian' || massClass === 'jovian'

  return useMemo(() => {
    const earthLike = esi > 0.8
    return new THREE.MeshStandardMaterial({
      color: colorByMassClass(massClass, earthLike),
      map: textureMap,
      metalness: gaseous ? 0.1 : 0.3,
      roughness: gaseous ? 0.9 : 0.7,
      emissive: earthLike ? 'green' : 'black',
      emissiveIntensity: earthLike ? 0.2 : 0,
    })
  }, [massClass, esi, gaseous, textureMap])
}

function colorByMassClass(massClass: string, earthLike: boolean): string {
  switch (massClass) {
    case 'terran': return earthLike ? '#66cc66' : '#3366cc'
    case 'neptunian': return '#99ccff'
    case 'jovian': return '#ff9933'
    default: return '#999999'
  }
}
