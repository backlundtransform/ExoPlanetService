import React, { useEffect, useState } from "react"
import StarMap from "./StarMap"
import { ExoplanetSystem } from "./ExoplanetSystem"
import { fetchStars, fetchSystem, fetchHabitablePlanets } from "./api"
import type { StarApi, HabitablePlanetApi } from "./types"
import "./App.css"
import { useTranslation } from "react-i18next"
import './i18n'

const App: React.FC = () => {
  const { t } = useTranslation()
  const [stars, setStars] = useState<StarApi[]>([])
  const [habitablePlanets, setHabitablePlanets] = useState<HabitablePlanetApi[]>([])
  const [selectedSystem, setSelectedSystem] = useState<any | null>(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [hoveredStarId, setHoveredStarId] = useState<number | null>(null)

  const [searchQuery, setSearchQuery] = useState("")
  const [showHabitableOnly, setShowHabitableOnly] = useState(false)

  // Mobil drawer state
  const [sidebarOpen, setSidebarOpen] = useState(false)

  useEffect(() => {
    fetchStars()
      .then((data: any[]) => setStars(data))
      .catch((err: any) => setError(err.message))

    fetchHabitablePlanets()
      .then((data: HabitablePlanetApi[]) => setHabitablePlanets(data))
      .catch(() => {})
  }, [])

  const handleSelectStar = async (starId: number) => {
    setLoading(true)
    setError(null)
    try {
      const system = await fetchSystem(starId)
      setSelectedSystem(system)
      setSidebarOpen(false) // stäng drawer på mobil när man väljer
    } catch (err: any) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  if (selectedSystem) {
    return <ExoplanetSystem system={selectedSystem} onBack={() => setSelectedSystem(null)} />
  }

  const filteredStars = stars
    .filter((star) => {
      const hasHabitablePlanet =
        habitablePlanets.some((p) => p.star?.name === star.name) || star.name === "Sun"
      const matchesSearch = star.name.toLowerCase().includes(searchQuery.toLowerCase())
      return matchesSearch && (!showHabitableOnly || hasHabitablePlanet)
    })
    .sort((a, b) => a.name.localeCompare(b.name))

  return (
    <div className="app-container" onClick={() => sidebarOpen && setSidebarOpen(false)}>
      {/* Toggle-knapp på mobil */}
      <button
        className="sidebar-toggle"
        onClick={(e) => { e.stopPropagation(); setSidebarOpen(!sidebarOpen) }}
      >
        ☰ 
      </button>
 

      {/* 🌌 SIDOPANEL */}
      <div
        className={`sidebar ${sidebarOpen ? "open" : ""}`}
        onClick={(e) => e.stopPropagation()}
      >
        <h2 style={{ marginTop: 0, color: "#58a6ff" }}>{t("app.title")}</h2>

        {/* 🔍 Sökfält */}
        <input
          type="text"
          placeholder={t("app.search_placeholder")}
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          style={{
            width: "100%",
            padding: "8px 10px",
            marginBottom: "10px",
            borderRadius: "6px",
            border: "1px solid #30363d",
            backgroundColor: "#0d1117",
            color: "#e6edf3",
            fontSize: "14px",
            outline: "none",
          }}
        />

        {/* 🌍 Checkbox */}
        <label
          style={{
            display: "flex",
            alignItems: "center",
            marginBottom: "10px",
            fontSize: "14px",
            cursor: "pointer",
            userSelect: "none",
          }}
        >
          <input
            type="checkbox"
            checked={showHabitableOnly}
            onChange={(e) => setShowHabitableOnly(e.target.checked)}
            style={{
              marginRight: "8px",
              accentColor: "#58a6ff",
            }}
          />
          {t("app.show_habitable_only")} 🌍
        </label>

        {/* Status */}
        {loading && <p style={{ color: "#8b949e" }}>{t("app.loading")}</p>}
        {error && <p style={{ color: "#f85149" }}>{t("app.error", { message: error })}</p>}

        {/* Antal resultat */}
        <p style={{ fontSize: "13px", color: "#8b949e", marginBottom: "8px" }}>
          {t("app.showing_count", { count: filteredStars.length, total: stars.length })}
        </p>


        {/* ⭐ Lista */}
        <ul style={{ listStyle: "none", padding: 0, margin: 0 }}>
          {filteredStars.map((star) => {
            const hasHabitablePlanet =
              habitablePlanets.some((p) => p.star?.name === star.name) || star.name === "Sun"
            const isHovered = hoveredStarId === star.id

            return (
              <li key={star.id} style={{ marginBottom: "6px" }}>
                <button
                  onClick={() => handleSelectStar(star.id)}
                  onMouseEnter={() => setHoveredStarId(star.id)}
                  onMouseLeave={() => setHoveredStarId(null)}
                  style={{
                    width: "100%",
                    padding: "8px 10px",
                    textAlign: "left",
                    cursor: "pointer",
                    borderRadius: "6px",
                    border: "1px solid #30363d",
                    backgroundColor: isHovered ? "#1f6feb22" : "#21262d",
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "space-between",
                    color: "#e6edf3",
                    transition: "background-color 0.2s ease, transform 0.1s ease",
                  }}
                >  
                  <span style={{ display: "flex", alignItems: "center", gap: "8px" }}>
                    {hasHabitablePlanet && <>🌍</>}
                    {star.name}
                  </span>

                  {hasHabitablePlanet && (
                    <span
                      style={{
                        width: "10px",
                        height: "10px",
                        borderRadius: "50%",
                        backgroundColor: "#2ea043",
                        display: "inline-block",
                        boxShadow: "0 0 6px #2ea043aa",
                      }}
                      title={t("app.habitable_tooltip")}
                    />
                  )}
                </button>
              </li>
            )
          })}
        </ul>
      </div>

      {/* Overlay för mobil */}
      {sidebarOpen && (
        <div
          className="overlay"
          onClick={() => setSidebarOpen(false)}
        />
      )}


      {/* 🪐 3D-stjärnkarta */}
      <div className="starmap-container">
        <StarMap
          stars={stars}
          habitablePlanets={habitablePlanets}
          onSelectStar={handleSelectStar}
        />
      </div>
    </div>
  )
}

export default App
