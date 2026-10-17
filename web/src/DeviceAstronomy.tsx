import { useEffect, useState } from 'react'
import useDeviceAstronomy from './hooks/useDeviceAstronomy'
import { significantDigits } from './utils'

interface DeviceAstronomyProps {
  /** Called with where the phone points, RA in hours and Dec in degrees. */
  onPointing: (raHours: number, decDegrees: number) => void
}

/**
 * "Point at the sky" mode for the Android app: a toggle button that, when active, turns the
 * star map towards where the phone is pointing (GPS position + device orientation).
 */
export function DeviceAstronomy({ onPointing }: DeviceAstronomyProps) {
  const [active, setActive] = useState(false)

  return (
    <>
      {active && <PointingTracker onPointing={onPointing} />}
      <button
        className={`gps-button ${active ? 'gps-button--active' : ''}`}
        onClick={() => setActive(!active)}
        aria-pressed={active}
        aria-label="Point at the sky"
      >
        📡
      </button>
    </>
  )
}

// Mounted only while the mode is active, so sensors are released when it is switched off.
function PointingTracker({ onPointing }: DeviceAstronomyProps) {
  const sky = useDeviceAstronomy()
  const raHours = sky.ra / 15 // the hook reports RA in degrees

  useEffect(() => {
    onPointing(raHours, sky.dec)
  }, [raHours, sky.dec, onPointing])

  return (
    <div className="info-card info-card--device">
      RA: {significantDigits(raHours)}h<br />
      Dec: {significantDigits(sky.dec)}°<br />
      Azimuth: {significantDigits(sky.alpha)}°<br />
      Pitch: {significantDigits(sky.beta)}°<br />
      Roll: {significantDigits(sky.gamma)}°
    </div>
  )
}
