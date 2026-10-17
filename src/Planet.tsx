import React, { useRef } from 'react'
import { useFrame } from '@react-three/fiber'
import { Mesh } from 'three'
import type { PlanetData, SystemData } from './types'
import { usePlanetMaterial } from './planetMaterial'
import { getPositionFromAngle } from './utils'

interface PlanetProps {
  planet: PlanetData
  scaleDistanceFn: (distanceAU: number) => number
  scaleRadiusFn: (radiusEarthRadii: number) => number
  system: SystemData
  onHover: (planet: PlanetData | null) => void
  orbitSpeedMultiplier?: number // global speed multiplier
}

export const Planet: React.FC<PlanetProps> = ({
  planet,
  scaleDistanceFn,
  scaleRadiusFn,
  system,
  onHover,
  orbitSpeedMultiplier = 0.3, // Långsammare rotation som default
}) => {
  const ref = useRef<Mesh>(null)

  const scaledRadius = scaleRadiusFn(planet.radius ?? 0)
  const material = usePlanetMaterial(planet)

  const a = scaleDistanceFn(planet.meanDistance ?? 0)
  const e = planet.eccentricity ?? 0
  const i = planet.inclination ?? 0

  const shortestPeriod = Math.min(...system.planets.map(p => p.period ?? 1))
  const periodDays = planet.period ?? 1
  const fastestOrbitSeconds = 20 // bas för snabbaste planeten

  useFrame(({ clock }) => {
    if (!ref.current) return

    const elapsed = clock.getElapsedTime()

    // Beräkna kontinuerlig vinkel
    const angle =
      (elapsed / fastestOrbitSeconds) *
      (shortestPeriod / periodDays) *
      2 *
      Math.PI *
      orbitSpeedMultiplier

    const pos = getPositionFromAngle(a, e, i, angle)
    ref.current.position.set(pos.x, pos.y, pos.z)
  })

  return (
    <mesh
      ref={ref}
      onPointerOver={() => onHover(planet)}
      onPointerOut={() => onHover(null)}
    >
      <sphereGeometry args={[scaledRadius, 32, 32]} />
      <primitive object={material} attach="material" />
    </mesh>
  )
}
