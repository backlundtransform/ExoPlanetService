import { useCallback, useLayoutEffect, useMemo, useRef, useState } from 'react'
import { Canvas, type ThreeEvent } from '@react-three/fiber'
import { OrbitControls, Stars as Starfield } from '@react-three/drei'
import type { OrbitControls as OrbitControlsImpl } from 'three-stdlib'
import * as THREE from 'three'
import { useTranslation } from 'react-i18next'
import { Capacitor } from '@capacitor/core'
import type { Star } from './types'
import { LIGHT_YEARS_PER_PARSEC, raDecToXYZ, significantDigits } from './utils'
import { DeviceAstronomy } from './DeviceAstronomy'


interface StarMapProps {
  stars: Star[]
  habitableStars: ReadonlySet<string>
  onSelectStar: (id: number) => void
}

export default function StarMap({ stars, habitableStars, onSelectStar }: StarMapProps) {
  const { t } = useTranslation()
  const [hovered, setHovered] = useState<Star | null>(null)
  const controls = useRef<OrbitControlsImpl>(null)
  const isAndroid = Capacitor.getPlatform() === 'android'

  // Stars without coordinates (e.g. the Sun) cannot be placed on the sky.
  const placed = useMemo(() => stars.filter((s) => s.ra != null && s.dec != null), [stars])

  const pointCamera = useCallback((raHours: number, decDegrees: number) => {
    if (!controls.current) return
    controls.current.object.position.set(...raDecToXYZ(raHours, decDegrees))
    controls.current.target.set(0, 0, 0)
    controls.current.update()
  }, [])

  return (
    <div className="starmap">
      {hovered && (
        <div className="info-card info-card--star">
          <b>{hovered.name}</b>
          <br />
          RA: {significantDigits(hovered.ra ?? 0)}h · Dec: {significantDigits(hovered.dec ?? 0)}°
          {hovered.constellation && <> · {hovered.constellation}</>}
          {hovered.distance != null && (
            <>
              <br />
              {t('app.star_distance')}: {significantDigits(hovered.distance * LIGHT_YEARS_PER_PARSEC)} ly
            </>
          )}
          {hovered.planets != null && (
            <>
              <br />
              {t('app.planets_count', { count: hovered.planets })}
            </>
          )}
          {habitableStars.has(hovered.name) && <div className="habitable-note">🌍 {t('app.habitable_tooltip')}</div>}
        </div>
      )}

      <Canvas camera={{ position: [0, 0, 100], fov: 60 }} dpr={[1, 2]}>
        <ambientLight intensity={0.5} />
        <Starfield radius={100} depth={50} count={1000} factor={4} saturation={0} fade />
        <StarField
          stars={placed}
          habitableStars={habitableStars}
          hoveredId={hovered?.id ?? null}
          onHover={setHovered}
          onSelect={onSelectStar}
        />
        <OrbitControls ref={controls} />
      </Canvas>

      {isAndroid && <DeviceAstronomy onPointing={pointCamera} />}
    </div>
  )
}

const WHITE = new THREE.Color('white')
const YELLOW = new THREE.Color('yellow')

interface StarFieldProps {
  stars: Star[]
  habitableStars: ReadonlySet<string>
  hoveredId: number | null
  onHover: (star: Star | null) => void
  onSelect: (id: number) => void
}

/** All stars as one instanced mesh (one draw call), plus green halos for habitable systems. */
function StarField({ stars, habitableStars, hoveredId, onHover, onSelect }: StarFieldProps) {
  const starMesh = useRef<THREE.InstancedMesh>(null)
  const haloMesh = useRef<THREE.InstancedMesh>(null)
  const habitable = useMemo(() => stars.filter((s) => habitableStars.has(s.name)), [stars, habitableStars])

  useLayoutEffect(() => {
    const matrix = new THREE.Matrix4()
    const place = (mesh: THREE.InstancedMesh | null, list: Star[]) => {
      if (!mesh) return
      list.forEach((s, i) => {
        matrix.makeTranslation(...raDecToXYZ(s.ra!, s.dec!))
        mesh.setMatrixAt(i, matrix)
      })
      mesh.instanceMatrix.needsUpdate = true
      mesh.computeBoundingSphere()
    }
    place(starMesh.current, stars)
    place(haloMesh.current, habitable)
  }, [stars, habitable])

  useLayoutEffect(() => {
    const mesh = starMesh.current
    if (!mesh) return
    stars.forEach((s, i) => mesh.setColorAt(i, s.id === hoveredId ? YELLOW : WHITE))
    if (mesh.instanceColor) mesh.instanceColor.needsUpdate = true
  }, [stars, hoveredId])

  const starAt = (e: ThreeEvent<PointerEvent | MouseEvent>) => (e.instanceId == null ? null : stars[e.instanceId])

  return (
    <>
      <instancedMesh
        ref={starMesh}
        args={[undefined, undefined, stars.length]}
        onPointerMove={(e) => {
          e.stopPropagation()
          onHover(starAt(e))
        }}
        onPointerOut={() => onHover(null)}
        onClick={(e) => {
          e.stopPropagation()
          const star = starAt(e)
          if (star) onSelect(star.id)
        }}
      >
        <sphereGeometry args={[0.5, 12, 12]} />
        <meshBasicMaterial />
      </instancedMesh>
      <instancedMesh ref={haloMesh} args={[undefined, undefined, habitable.length]} raycast={() => null}>
        <sphereGeometry args={[0.65, 12, 12]} />
        <meshBasicMaterial color="green" transparent opacity={0.5} depthWrite={false} />
      </instancedMesh>
    </>
  )
}
