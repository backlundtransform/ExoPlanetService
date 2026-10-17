import { useState, useRef, useEffect } from "react";
import { Canvas } from "@react-three/fiber";
import { OrbitControls, Stars as DreiStars } from "@react-three/drei";
import type { StarApi } from "./types";
import type { HabitablePlanetApi } from './types';
import { useTranslation } from "react-i18next";
import "./i18n";
import { significantDigits } from "./utils";
import useDeviceAstronomy from "./hooks/useDeviceAstronomy";

export default function StarMap({
  stars,
  habitablePlanets,
  onSelectStar,
}: {
  stars: StarApi[];
  habitablePlanets: HabitablePlanetApi[];
  onSelectStar: (id: number) => void;
}) {
  const { t } = useTranslation();
  const [hoveredStarId, setHoveredStarId] = useState<number | null>(null);
  const [gpsActive, setGpsActive] = useState(false);
const isAndroid = /Android/i.test(navigator.userAgent);
const deviceAstronomy = isAndroid ? useDeviceAstronomy() : { ra: 0, dec: 0, alpha: 0, beta: 0, gamma: 0 };

  const orbitRef = useRef<any>(null);

  const hoveredStar = stars.find((s) => s.id === hoveredStarId) ?? null;

  const raDecToXYZ = (ra: number, dec: number, radius: number) => {
    const raRad = (ra / 24) * 2 * Math.PI;
    const decRad = (dec / 180) * Math.PI;
    return [
      radius * Math.cos(decRad) * Math.cos(raRad),
      radius * Math.sin(decRad),
      radius * Math.cos(decRad) * Math.sin(raRad),
    ];
  };

  // Flytta kameran mot RA/Dec
  const zoomToRaDec = (ra: number, dec: number, radius = 40) => {
    if (!orbitRef.current) return;
    const [x, y, z] = raDecToXYZ(ra, dec, radius);
    orbitRef.current.object.position.set(x, y, z);
    orbitRef.current.target.set(0, 0, 0); // kan ändras om man vill zooma mot stjärnan
    orbitRef.current.update();
  };

  // Exempel: zooma till GPS-position när GPS-knappen aktiveras
  useEffect(() => {
    if (gpsActive && deviceAstronomy.ra != null && deviceAstronomy.dec != null) {
      zoomToRaDec(deviceAstronomy.ra, deviceAstronomy.dec);
    }
  }, [gpsActive, deviceAstronomy]);

  return (
    <div style={{ width: "100%", height: "100%", backgroundColor: "#0d1117" }}>
      {/* 🌐 GPS/Device info */}
      {gpsActive&&isAndroid && (
        <div style={{
          position: "absolute",
          bottom: 50,
          right: 10,
          zIndex: 20,
          backgroundColor: "rgba(0,0,0,0.75)",
          color: "white",
          padding: "12px 16px",
          borderRadius: "8px",
          width: "90px",
          fontSize: "14px",
          lineHeight: "1.4",
        }}>
          RA: {significantDigits((deviceAstronomy.ra ?? 0) / 15)}h<br />
          Dec: {significantDigits(deviceAstronomy.dec ?? 0)}°<br />
          Azimuth: {significantDigits(deviceAstronomy.alpha ?? 0)}<br />
          Pitch: {significantDigits(deviceAstronomy.beta ?? 0)}<br />
          Roll: {significantDigits(deviceAstronomy.gamma ?? 0)}
        </div>
      )}

      {/* 🪐 Stjärninfo */}
      {hoveredStar && (
        <div style={{
          position: "absolute",
          top: 10,
          right: 80,
          zIndex: 10,
          backgroundColor: "rgba(0,0,0,0.75)",
          color: "white",
          padding: "12px 16px",
          borderRadius: "8px",
          width: "140px",
          fontSize: "14px",
          lineHeight: "1.4",
        }}>
          <b>{hoveredStar.name}</b><br />
          RA: {significantDigits((hoveredStar.ra ?? 0) / 15)}h<br />
          Dec: {significantDigits(hoveredStar.dec ?? 0)}°<br />
          {habitablePlanets.some((p) => p.star?.name === hoveredStar.name) && (
            <div style={{ marginTop: "4px", color: "lime" }}>
              🌍 {t("app.habitable_tooltip")}
            </div>
          )}
        </div>
      )}

      {/* 🎇 3D-kartan */}
      <Canvas camera={{ position: [0, 0, 100], fov: 60 }} style={{ width: "100%", height: "100%" }}>
        <ambientLight intensity={0.5} />
        <pointLight position={[0, 0, 0]} intensity={1} />
        <DreiStars radius={100} depth={50} count={1000} factor={4} saturation={0} fade />

        {stars.map((star) => {
          const [x, y, z] = raDecToXYZ(star.ra ?? 0, star.dec ?? 0, 40);
          const isHovered = hoveredStarId === star.id;
          const isHabitable = habitablePlanets.some((p) => p.star?.name === star.name);
          return (
            <group key={star.id} position={[x, y, z]}>
              {isHabitable && (
                <mesh>
                  <sphereGeometry args={[0.65, 16, 16]} />
                  <meshBasicMaterial color={isHovered ? "yellow" : "green"} transparent opacity={0.5} />
                </mesh>
              )}
              <mesh
                onPointerOver={() => setHoveredStarId(star.id)}
                onPointerOut={() => setHoveredStarId(null)}
                onClick={() => onSelectStar(star.id)}
              >
                <sphereGeometry args={[0.5, 16, 16]} />
                <meshBasicMaterial color={isHovered ? "yellow" : "white"} />
              </mesh>
            </group>
          );
        })}

        <OrbitControls ref={orbitRef} />
      </Canvas>

      {/* GPS-knapp */}
      {isAndroid && (
      <button
        className="gps-button"
        onClick={() => setGpsActive(!gpsActive)}
        style={{ cursor: "pointer",
          backgroundColor: gpsActive ? "#7a99c9ff" : "#30363d", border: "none", color: "white", fontSize: "20px" }}
      >
        📡
      </button>)}
    </div>
  );
}
