import React, { useMemo } from 'react'
import { Line, Tube } from '@react-three/drei'
import * as THREE from 'three'
import { LineMaterial } from 'three-stdlib'
import type { PlanetData } from './types'

interface OrbitPathProps {
  semiMajorAxis: number
  eccentricity: number
  inclination: number
  color?: string
  segments?: number
  planet?: PlanetData | null
  onHover?: (planet: PlanetData | null) => void
}

const material = new LineMaterial({
  linewidth: 0.5,
  transparent: true,
  opacity: 0.1,
  depthWrite: false,
})

export const OrbitPathInteractive: React.FC<OrbitPathProps> = ({
  semiMajorAxis,
  eccentricity,
  inclination,
  color = 'white',
  segments = 128,
  planet = null,
  onHover,
}) => {
  const points = useMemo(() => {
    const a = semiMajorAxis
    const b = a * Math.sqrt(1 - eccentricity ** 2)
    const inclinationRad = (inclination * Math.PI) / 180

    const pts = new Array(segments).fill(0).map((_, i) => {
      const angle = (i / segments) * 2 * Math.PI
      const x = a * Math.cos(angle)
      const z = b * Math.sin(angle)
      const y = 0

      const rotatedY = y * Math.cos(inclinationRad) - z * Math.sin(inclinationRad)
      const rotatedZ = y * Math.sin(inclinationRad) + z * Math.cos(inclinationRad)

      return new THREE.Vector3(x, rotatedY, rotatedZ)
    })

    pts.push(pts[0].clone())
    return pts
  }, [semiMajorAxis, eccentricity, inclination, segments])

  const curve = useMemo(() => new THREE.CatmullRomCurve3(points), [points])

  return (
    <>
      {/* Visuell orbit-linje */}
      <Line points={points} color={color} lineWidth={0.5} material={material} />

      {/* Osynlig tube för touch på mobil */}
      <Tube
        args={[curve, segments, semiMajorAxis * 0.02, 8, false]} // liten men tryckvänlig
        visible={false}
        onPointerOver={() => onHover?.(planet)}
        onPointerOut={() => onHover?.(null)}
        onClick={() => onHover?.(planet)}
      />
    </>
  )
}
