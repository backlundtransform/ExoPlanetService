# Fältmappning: PHL → NASA Exoplanet Archive

Underlag för importen i den nya tjänsten (fas 2). Målet är att inte längre vara beroende av
PHL:s katalog och att i stället läsa `pscomppars` från NASA Exoplanet Archive (TAP) och räkna
fram resten med CSharpNumerics.

Siffrorna nedan kommer från `tests/contract/compare_nasa.py` (2026-10-12). Skriptet jämför
NASA-härledda värden med produktionens ögonblicksbild från `record.py`.

## Sammanfattning

- **PHL behövs inte.** Alla fält som produktionen faktiskt fyller i kan hämtas eller räknas
  fram från NASA. De PHL-specifika fälten (`hzd`, `hzc`, `hza`, `hzi`, `sph`, `intEsi`,
  `surfEsi`, `compositionClass`, `atmosphereClass`, `habitableClass`) är tomma i produktion
  redan idag, utom för Solsystemets 8 planeter.
- **Mer och färskare data.** NASA har 6 445 planeter (upptäckter t.o.m. 2026). Produktionen
  har 5 649 (t.o.m. 2024). 880 planeter är nya.
- **84 planeter försvinner.** 8 av dem är Solsystemet, som läggs in som fast data. Resten har
  dragits tillbaka eller bytt namn (t.ex. GJ 832 c, HD 85512 b, TOI 150.01 → TOI-150 b).

## Planet

| Kontraktsfält | Täckning i prod | Ny källa | Överens med prod (±5 % / ±20 %) |
|---|---|---|---|
| name | 100 % | `pl_name` | – |
| mass | 99 % | `pl_bmasse` (M⊕, massa eller M·sin i) | – |
| radius | 99 % | `pl_rade` (R⊕; NASA fyller luckor med massa–radie-relation) | – |
| density | 99 % | M/R³ (jordenheter). **Inte** `pl_dens`, som är i g/cm³ | 93 % / 95 % |
| gravity | 99 % | M/R² | 93 % / 96 % |
| escVel | 99 % | √(M/R) | 94 % / 97 % |
| sFluxMean | 95 % | L/a², där L = 10^`st_lum`. Reserv: `pl_insol` | 85 % / 97 % |
| sFluxMin / sFluxMax | 95 % | L/(a(1+e))² resp. L/(a(1−e))² | – |
| teqMean / Min / Max | 95 % | 254,6 · S^¼ (Bond-albedo 0,3, som PHL) | 85 % / 97 % |
| tsMean / Min / Max | 95 % | Samma som teq (som idag) | – |
| period | 95 % | `pl_orbper` | – |
| semMajorAxis | **0 %** | `pl_orbsmax`. Var trasig i gamla importen, blir nu ifylld | – |
| meanDistance | 99 % | a·(1 + e²/2). Reserv: Kepler III från period och stjärnmassa | 97 % / 99 % |
| eccentricity | 33 % | `pl_orbeccen` | – |
| inclination | 76 % | `pl_orbincl` | – |
| omega | 28 % | `pl_orblper` | – |
| esi | 95 % | 1 − √(½[((S−1)/(S+1))² + ((R−1)/(R+1))²]) | 90 % / 98 % |
| massClass | 99 % | Trösklar på M (reserv R), oförändrad logik | – |
| zoneClass | 95 % | Hot/Warm/Cold utifrån S mot den beboeliga zonen | – |
| habitable | 1 % | Se *Beboelighet* nedan | 62/72 |
| disc_Method | 99 % | `discoverymethod` utan mellanslag (samma värden som idag) | – |
| disc_Year | 99 % | `disc_year` | – |
| nameKepler, nameKOI, minMass, maxMass, compositionClass, atmosphereClass, habitableClass, surfPress, mag, apparSize, hzd, hzc, hza, hzi, sph, intEsi, surfEsi, habMoon, confirmed | 0 % | `null`, utom Solsystemet (fast data) | – |

## Stjärna

| Kontraktsfält | Täckning i prod | Ny källa |
|---|---|---|
| name | 100 % | `hostname` |
| nameHD / nameHIP | **0 %** | `hd_name` / `hip_name`. Fylls nu i |
| type | 94 % | `st_spectype`. Reserv: spektralklass från `st_teff` (`GetSpectralFromTemp`) |
| mass / radius / teff / age | 92 / 90 / 92 / 52 % | `st_mass` / `st_rad` / `st_teff` / `st_age` |
| luminosity | 83 % | R²·(T/5772)⁴ (Stefan–Boltzmann, 78 % inom ±5 %). Reserv: 10^`st_lum`. **OBS:** `st_lum` är log₁₀. Den gamla NASA-importen sparade logaritmen rakt av |
| feH | **0 %** | `st_met`. Fylls nu i |
| apparMag | 92 % | `sy_vmag` |
| distance | 99 % | `sy_dist` (pc) |
| ra | 99 % | `ra` / 15. **Kontraktet använder timmar**, NASA ger grader |
| dec | 96 % | `dec` |
| constellation | 84 % | Beräknas från RA/Dec med egen gränstabell (Roman 1987). Gamla koden precesserar från `DateTime.Now.Year` i stället för J2000, vilket ska rättas |
| habZoneMin / habZoneMax | 92 / 95 % | Kopparapu et al. (2013), **optimistiska** gränser (Recent Venus–Early Mars). Produktionens värden motsvarar dessa, t.ex. Kepler-1556: 0,678–1,605 AU ⇒ Seff ≈ 1,74 / 0,31. CSharpNumerics `CalculateGoldilocksZone` ger idag bara de konservativa gränserna |
| planets | 100 % | Antal planeter efter import |
| magfromPlanet, sizefromPlanet, habCat | 0 % | `null` |

## Beboelighet

`habitable = true` när båda villkoren gäller:

1. Strålningsflödet S ligger inom Kopparapus *optimistiska* beboeliga zon, alltså mellan
   gränserna Recent Venus och Early Mars. Stjärnans temperatur begränsas till intervallet
   2 600–7 200 K.
2. 0,5 ≤ R ≤ 2,5 R⊕. Saknas radie används M ≤ 10 M⊕.

Det är PHL:s metod. Med NASA:s parametrar ger den 77 planeter. 62 av dem finns bland
produktionens 72. Två av de övriga 10 har dragits tillbaka, och resten skiljer sig för att
parametrarna har uppdaterats sedan PHL:s senaste körning.

Gamla `PlanetClassifier` krävde dessutom ESI > 0,6. Det kravet tas bort, eftersom ESI redan
härleds från S och R.

| Variant | Antal | Överlapp med prod |
|---|---|---|
| Optimistisk zon, R 0,5–2,5 (**vald**) | 77 | 62 / 72 |
| Optimistisk zon, R 0,5–2,0 | 51 | 43 / 72 |
| Optimistisk zon, R 0,5–1,6 | 32 | 29 / 72 |
| Konservativ zon, R 0,5–1,6 | 18 | 18 / 72 |

## Id-stabilitet

API:et exponerar heltals-id:n (`/api/Stars/{id}`, `/api/Planets/{id}`), och externa klienter
kan ha sparat dem. Importen ska därför behålla de id:n som finns i ögonblicksbilden:

- Befintliga stjärnor och planeter matchas på namn och behåller sitt id. NASA:s alias kan
  användas för att följa namnbyten.
- Nya objekt får id:n över nuvarande maxvärde.
- Borttagna objekt ger 404. Dagens tjänst svarar 204 för okända id:n.

## Förslag på tillägg i CSharpNumerics

Följande formler behövs i importen men finns inte i `AstronomyExtensions` idag:

- `StellarLuminosity(radiusSolar, teffK)`: Stefan–Boltzmann i solenheter.
- `InsolationFlux(luminositySolar, semiMajorAxisAU, eccentricity)`: medel, min och max.
- `EquilibriumTemperature(flux, bondAlbedo)`.
- `CalculateEsi(radiusEarth, fluxEarth)`: ESI med två parametrar (radie och flöde), som PHL
  använder. Den befintliga överlagringen tar fyra parametrar.
- `HabitableZoneFlux(teffK, model)`: Seff-gränserna enligt Kopparapu (konservativa och
  optimistiska), så att klassningen kan göras i flöde i stället för i AU.
- Ett modellval i `CalculateGoldilocksZone`, eftersom den idag bara ger de konservativa
  gränserna (Runaway/Maximum greenhouse). API:et behöver de optimistiska (Recent Venus/Early
  Mars) för att behålla dagens `habZoneMin`/`habZoneMax`.
