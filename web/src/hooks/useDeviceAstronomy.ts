import { useEffect, useState } from 'react'
import { Geolocation } from '@capacitor/geolocation'
import { Motion } from '@capacitor/motion'
import type { PluginListenerHandle } from '@capacitor/core'

const toRad = (d: number) => (d * Math.PI) / 180
const toDeg = (r: number) => (r * 180) / Math.PI

/** Local sidereal time in hours for a longitude in degrees (east positive). */
const localSiderealTime = (longitude: number, now = new Date()) => {
  const daysSinceJ2000 = now.getTime() / 86400000 + 2440587.5 - 2451545.0
  const gst = 18.697374558 + 24.06570982441908 * daysSinceJ2000
  return (((gst + longitude / 15) % 24) + 24) % 24
}

/** Horizontal to equatorial coordinates. Returns RA and Dec in degrees. */
const altAzToRaDec = (altitude: number, azimuth: number, latitude: number, longitude: number) => {
  const az = toRad((450 - azimuth) % 360)
  const alt = toRad(altitude)
  const lat = toRad(latitude)

  const dec = Math.asin(Math.sin(lat) * Math.sin(alt) + Math.cos(lat) * Math.cos(alt) * Math.cos(az))
  const hourAngle = Math.atan2(-Math.sin(az) * Math.cos(alt), Math.sin(alt) - Math.sin(lat) * Math.sin(dec))
  const ra = (localSiderealTime(longitude) * 15 - toDeg(hourAngle) + 360) % 360

  return { ra, dec: toDeg(dec) }
}

/**
 * Where the phone is pointing on the sky, from GPS position and device orientation.
 * RA is in degrees, Dec in degrees; alpha/beta/gamma are the raw orientation angles.
 */
export function useDeviceAstronomy() {
  const [position, setPosition] = useState({ latitude: 0, longitude: 0 })
  const [orientation, setOrientation] = useState({ alpha: 0, beta: 0, gamma: 0 })

  useEffect(() => {
    let disposed = false
    let listener: PluginListenerHandle | undefined

    Geolocation.getCurrentPosition()
      .then((pos) => !disposed && setPosition({ latitude: pos.coords.latitude, longitude: pos.coords.longitude }))
      .catch(() => {}) // permission denied: keep pointing relative to lat/lon 0

    Motion.addListener('orientation', (event) => {
      setOrientation({ alpha: event.alpha ?? 0, beta: event.beta ?? 0, gamma: event.gamma ?? 0 })
    }).then((handle) => {
      if (disposed) handle.remove()
      else listener = handle
    })

    return () => {
      disposed = true
      listener?.remove()
    }
  }, [])

  const azimuth = (orientation.alpha + 360) % 360
  // Kept from the original app. Note: this maps beta −90…90 to 0…180, outside the valid
  // altitude range; `beta - 90` (rear camera pointing) may be intended. Verify on a device.
  const altitude = orientation.beta + 90
  const { ra, dec } = altAzToRaDec(altitude, azimuth, position.latitude, position.longitude)

  return { ...position, ...orientation, altitude, azimuth, ra, dec }
}

export default useDeviceAstronomy
