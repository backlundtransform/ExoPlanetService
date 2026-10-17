import React, { useMemo, useState } from 'react'
import { Canvas } from '@react-three/fiber'
import { OrbitControls, Stars } from '@react-three/drei'
import { Bloom, EffectComposer } from '@react-three/postprocessing'
import { useTranslation } from 'react-i18next'
import { Planet } from './Planet'
import { Star } from './Star'
import { HabitableZone } from './HabitableZone'
import { OrbitPathInteractive } from './OrbitRing'
import type { Planet as PlanetData, StarSystem } from './types'
import {
  LIGHT_YEARS_PER_PARSEC,
  formatValue,
  makeScaleDistanceFn,
  makeScaleRadiusFn,
  scaleStarRadiusToUnits,
} from './utils'

interface ExoplanetSystemProps {
  system: StarSystem
  onBack: () => void
}

export const ExoplanetSystem: React.FC<ExoplanetSystemProps> = ({ system, onBack }) => {
  const { t } = useTranslation()
  const [hoveredPlanet, setHoveredPlanet] = useState<PlanetData | null>(null)
  const [hoveredStar, setHoveredStar] = useState(false)
  const { star, planets } = system
  const unknown = t('app.unknown')

  const scaleDistanceFn = useMemo(() => makeScaleDistanceFn(planets, 200), [planets])
  const scaleRadiusFn = useMemo(() => makeScaleRadiusFn(planets, 5, 0.8), [planets])
  const maxDistance = scaleDistanceFn(Math.max(0, ...planets.map((p) => p.meanDistance ?? 0)))
  const cameraZ = Math.max(maxDistance * 1.5, 30)

  return (
    <div className="system-view">
      <button className="back-button" onClick={onBack}>
        ← {t('app.back_to_map')}
      </button>

      {hoveredPlanet && (
        <div className="info-card info-card--planet">
          <b>{hoveredPlanet.name}</b>
          <br />
          {t('app.planet_mass')}: {formatValue(hoveredPlanet.mass, 'M⊕', unknown)}
          <br />
          {t('app.planet_radius')}: {formatValue(hoveredPlanet.radius, 'R⊕', unknown)}
          <br />
          {t('app.planet_distance')}: {formatValue(hoveredPlanet.meanDistance, 'AU', unknown)}
          <br />
          {t('app.planet_temp')}: {formatValue(hoveredPlanet.tsMean, 'K', unknown)}
          <br />
          {t('app.planet_period')}: {formatValue(hoveredPlanet.period, t('app.planet_days'), unknown)}
          <br />
          {t('app.planet_discovery_method')}: {hoveredPlanet.disc_Method ?? unknown}
          <br />
          {t('app.planet_habitability')}: {hoveredPlanet.habitable ? t('app.yes') : t('app.no')}
          <br />
          {t('app.planet_discovery_year')}: {hoveredPlanet.disc_Year ?? unknown}
        </div>
      )}

      {hoveredStar && (
        <div className="info-card info-card--star">
          <b>{star.name}</b>
          <br />
          {t('app.star_distance')}: {formatValue(star.distance == null ? null : star.distance * LIGHT_YEARS_PER_PARSEC, 'ly', unknown)}
          <br />
          {t('app.star_mass')}: {formatValue(star.mass, 'M☉', unknown)}
          <br />
          {t('app.star_temperature')}: {formatValue(star.teff, 'K', unknown)}
          <br />
          {t('app.star_luminosity')}: {formatValue(star.luminosity, 'L☉', unknown)}
          <br />
          {t('app.star_radius')}: {formatValue(star.radius, 'R☉', unknown)}
          <br />
          {t('app.star_type')}: {star.type || unknown}
        </div>
      )}

      <Canvas
        camera={{ position: [0, maxDistance * 0.3, cameraZ], fov: 50 }}
        dpr={[1, 2]}
        // Tapping empty space closes the info card on touch screens.
        onPointerMissed={() => setHoveredPlanet(null)}
      >
        <ambientLight intensity={0.3} />
        <pointLight position={[0, 0, 0]} intensity={2} />
        <Stars radius={300} depth={60} count={10000} factor={7} saturation={0} fade speed={1} />

        <Star radius={scaleStarRadiusToUnits(star.radius ?? 1, maxDistance)} spectralType={star.type ?? ''} onHover={setHoveredStar} />
        {star.habZoneMin != null && star.habZoneMax != null && (
          <HabitableZone innerRadius={star.habZoneMin} outerRadius={star.habZoneMax} scaleDistanceFn={scaleDistanceFn} />
        )}

        {planets.map((planet) => (
          <React.Fragment key={planet.id}>
            <OrbitPathInteractive
              semiMajorAxis={scaleDistanceFn(planet.meanDistance ?? 0)}
              eccentricity={planet.eccentricity ?? 0}
              inclination={planet.inclination ?? 0}
              planet={planet}
              onHover={setHoveredPlanet}
            />
            <Planet
              planet={planet}
              planets={planets}
              scaleDistanceFn={scaleDistanceFn}
              scaleRadiusFn={scaleRadiusFn}
              onHover={setHoveredPlanet}
            />
          </React.Fragment>
        ))}

        <EffectComposer>
          <Bloom intensity={1.5} kernelSize={3} luminanceThreshold={0.1} luminanceSmoothing={0.9} />
        </EffectComposer>

        <OrbitControls />
      </Canvas>
    </div>
  )
}
