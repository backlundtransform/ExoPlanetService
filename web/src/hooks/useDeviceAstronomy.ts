import { useState, useEffect } from "react";
import { Geolocation } from "@capacitor/geolocation";
import { Motion } from "@capacitor/motion";
import type { PluginListenerHandle } from "@capacitor/core";

const toRad = (d: number) => d * Math.PI / 180;
const toDeg = (r: number) => r * 180 / Math.PI;

// Local Sidereal Time
const localSiderealTime = (lon: number) => {
    const now = new Date();
    const JD = now.getTime() / 86400000 + 2440587.5;
    const D = JD - 2451545.0;
    const GST = 18.697374558 + 24.06570982441908 * D;
    const LST = (GST + lon / 15) % 24;
    return (LST + 24) % 24;
};

// Alt/Az → RA/Dec
const altAzToRaDec = (altDeg: number, azDeg: number, latDeg: number, lon: number) => {
    const az = toRad((450 - azDeg) % 360);
    const alt = toRad(altDeg);
    const lat = toRad(latDeg);

    const dec = Math.asin(
        Math.sin(lat) * Math.sin(alt) +
        Math.cos(lat) * Math.cos(alt) * Math.cos(az)
    );

    const H = Math.atan2(
        -Math.sin(az) * Math.cos(alt),
        Math.sin(alt) - Math.sin(lat) * Math.sin(dec)
    );

    const lst = localSiderealTime(lon);
    let ra = lst * 15 - toDeg(H);
    ra = (ra + 360) % 360;

    return { ra, dec: toDeg(dec) };
};

export const useDeviceAstronomy = () => {
    const [coords, setCoords] = useState({ lat: 0, lon: 0 });
    const [orientation, setOrientation] = useState({ alpha: 0, beta: 0, gamma: 0 });
    const [astro, setAstro] = useState({ altitude: 0, azimuth: 0, ra: 0, dec: 0 });

useEffect(() => {
    // 1. GPS
    Geolocation.getCurrentPosition().then(pos => {
        setCoords({ lat: pos.coords.latitude, lon: pos.coords.longitude });
    });

    // 2. Orientation
    let listenerHandle: PluginListenerHandle;

    Motion.addListener("orientation", (event: any) => {
        setOrientation({
            alpha: event.alpha ?? 0,
            beta: event.beta ?? 0,
            gamma: event.gamma ?? 0
        });
    }).then(handle => {
        listenerHandle = handle;
    });

    return () => {
        if (listenerHandle) listenerHandle.remove(); // ✅ korrekt
    };
}, []);
    useEffect(() => {
        const { alpha, beta } = orientation;
        const { lat, lon } = coords;

        const azimuth = (alpha + 360) % 360;
        const altitude = beta + 90; // pitch −90→+90 blir 0→180

        const { ra, dec } = altAzToRaDec(altitude, azimuth, lat, lon);
        setAstro({ altitude, azimuth, ra, dec });
    }, [orientation, coords]);

        return { latitude: coords.lat, longitude: coords.lon, ...astro, alpha: orientation.alpha, beta: orientation.beta, gamma: orientation.gamma };
};

export default useDeviceAstronomy;


