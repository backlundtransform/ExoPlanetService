import React, { useState, useEffect } from 'react'
import { Canvas } from '@react-three/fiber'
import { OrbitControls, Stars } from '@react-three/drei'
import { Planet } from './Planet'
import { Star } from './Star'
import { Bloom, EffectComposer } from '@react-three/postprocessing'
import { HabitableZone } from './HabitableZone'
import { OrbitPathInteractive } from './OrbitRing'
import type { SystemData, PlanetData } from './types'
import { makeScaleDistanceFn, makeScaleRadiusFn, scaleStarRadiusToUnits, significantDigits } from './utils'
import { useTranslation } from 'react-i18next'

interface ExoplanetSystemProps {
  system: SystemData
  onBack: () => void
}

export const ExoplanetSystem: React.FC<ExoplanetSystemProps> = ({ system, onBack }) => {
  const [hoveredPlanet, setHoveredPlanet] = useState<PlanetData | null>(null)
  const [hoveredStar, setHoveredStar] = useState<boolean>(false)
  const { t } = useTranslation()
  const [isMobile, setIsMobile] = useState(false)

  useEffect(() => {
    const checkMobile = () => setIsMobile(window.innerWidth < 768)
    checkMobile()
    window.addEventListener('resize', checkMobile)
    return () => window.removeEventListener('resize', checkMobile)
  }, [])

  const scaleDistanceFn = makeScaleDistanceFn(system, 200)
  const scaleRadiusFn = makeScaleRadiusFn(system, 5, 0.8)
  const maxDistanceAU = Math.max(...system.planets.map(p => p.meanDistance ?? 0))
  const maxDistance = scaleDistanceFn(maxDistanceAU)
  const cameraZ = maxDistance * 1.5

  
  
  const handleBackgroundClick = () => {
    if (isMobile) setHoveredPlanet(null)
  }

  return (
    <div className="canvas-container" style={{ width: '100%', height: '100%', position: 'relative' }} onClick={handleBackgroundClick}>
      <button
        onClick={onBack}
        style={{
          position: 'absolute',
          top: 10,
          left: 10,
          zIndex: 10,
          padding: '8px 12px',
          borderRadius: 4,
          border: 'none',
          backgroundColor: '#007bff',
          color: 'white',
          cursor: 'pointer',
        }}
      >
        ← {t('app.title')}
      </button>

      {hoveredPlanet && (
        <div
          style={{
            position: 'absolute',
            top: isMobile ? '50%' : 10,
            left: isMobile ? '50%' : undefined,
            right: isMobile ? undefined : 10,
            transform: isMobile ? 'translate(-50%, -50%)' : undefined,
            zIndex: 10,
            backgroundColor: 'rgba(0,0,0,0.75)',
            color: 'white',
            padding: '12px 16px',
            borderRadius: '8px',
            width: '250px',
            lineHeight: '1.4',
          }}
        >
          <b>{hoveredPlanet.name}</b>
          <br />
          {t('app.planet_mass')}: {significantDigits(hoveredPlanet.mass ?? 0)} M⊕
          <br />
          {t('app.planet_radius')}: {significantDigits(hoveredPlanet.radius ?? 0)} R⊕
          <br />
          {t('app.planet_distance')}: {significantDigits(hoveredPlanet.meanDistance ?? 0)} AU
          <br />
          {t('app.planet_temp')}: {hoveredPlanet.tsMean ? `${significantDigits(hoveredPlanet.tsMean)} K` : 'N/A'}
          <br />
          {t('app.planet_period')}: {significantDigits(hoveredPlanet.period ?? 0)} {t('app.planet_days')}
          <br />
          {t('app.planet_discovery_method')}: {hoveredPlanet.disc_Method ?? t('app.unknown')}
          <br />
          {t('app.planet_habitability')}: {hoveredPlanet.habitable ? t('app.yes') : t('app.no')}
          <br />
          {t('app.planet_discovery_year')}: {hoveredPlanet.disc_Year ?? t('app.unknown')}
          
        </div>
      )}
      {hoveredStar && (
        <div
          style={{
            position: 'absolute',
            top: 10,
            right: 10,
            zIndex: 10,
            backgroundColor: 'rgba(0,0,0,0.75)',
            color: 'white',
            padding: '12px 16px',
            borderRadius: '8px',
            width: '250px',
            lineHeight: '1.4',
          }}
        >
          <b>{system.name}</b>
          <br />
          {t('app.star_distance')}: {significantDigits(3.262 * (system.distance ?? 0))} ly
          <br />
          {t('app.star_mass')}: {significantDigits(system.mass ?? 0)} M☉
          <br />
          {t('app.star_temperature')}: {significantDigits(system.teff ?? 0)} K
          <br />
          {t('app.star_luminosity')}: {significantDigits(system.luminosity ?? 0)} L☉
          <br />
          {t('app.star_radius')}: {significantDigits(system.radius ?? 0)} R☉
          <br />
          {t('app.star_type')}: {system.type ?? t('app.unknown')}




        </div>
      )}  

      <Canvas camera={{ position: [0, maxDistance * 0.3, cameraZ], fov: 50 }}>
        <ambientLight intensity={0.3} />
        <pointLight position={[0, 0, 0]} intensity={2} />
        <Stars radius={300} depth={60} count={10000} factor={7} saturation={0} fade speed={1} />

        <Star 
        radius={scaleStarRadiusToUnits(system.radius ?? 0, maxDistance)} spectralType={system.type} onHover={setHoveredStar} />
        <HabitableZone innerRadius={system.habZoneMin} outerRadius={system.habZoneMax} scaleDistanceFn={scaleDistanceFn} />

        {system.planets.map((planet) => (
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
              scaleDistanceFn={scaleDistanceFn}
              scaleRadiusFn={scaleRadiusFn}
              system={system}
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
