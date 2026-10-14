"""Build data/catalog.legacy.json.gz from the production recordings.

The result is a catalog snapshot with the exact ids, names and values served by the
legacy service. It seeds the new API until the NASA ingest (phase 2) exists, and it is
the reference the ingest uses to keep ids stable.

Usage:  python tools/seed/build_legacy_seed.py   (run tests/contract/record.py first)
"""
import gzip
import json
from datetime import datetime, timezone
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
RECORDINGS = ROOT / "tests" / "contract" / "recordings"
OUTPUT = ROOT / "data" / "catalog.legacy.json.gz"


def body(name):
    return json.loads((RECORDINGS / f"{name}.json").read_text(encoding="utf-8"))["body"]


def main():
    stars = body("stars_list")
    planets = body("planets_list")
    exo = body("exo_planets")
    constellations = body("constellations_list")

    star_id_by_name = {s["name"]: s["id"] for s in stars}
    if len(star_id_by_name) != len(stars):
        raise SystemExit("Star names are not unique; cannot link planets to stars by name.")

    star_name_by_planet_id = {p["id"]: p["star"]["name"] for p in exo}
    hab_planets_by_star = {p["star"]["name"]: p["star"]["noHabPlanets"] for p in exo}

    out_stars = []
    for s in stars:
        star = {k: v for k, v in s.items() if k not in ("planets", "message")}
        star["noPlanetsHZ"] = hab_planets_by_star.get(s["name"])
        out_stars.append(star)

    out_planets = []
    for p in planets:
        planet = {k: v for k, v in p.items() if k != "message"}
        planet["starId"] = star_id_by_name[star_name_by_planet_id[p["id"]]]
        out_planets.append(planet)

    # Sanity checks against counts the legacy API reports.
    per_star = {}
    for p in out_planets:
        per_star[p["starId"]] = per_star.get(p["starId"], 0) + 1
    mismatched = [s["name"] for s in stars if per_star.get(s["id"], 0) != s["planets"]]
    if mismatched:
        raise SystemExit(f"Planet counts differ for {len(mismatched)} stars, e.g. {mismatched[:5]}")

    per_constellation = {}
    for s in stars:
        per_constellation[s["constellation"]] = per_constellation.get(s["constellation"], 0) + 1
    bad = [c["name"] for c in constellations if per_constellation.get(c["name"], 0) != c["stars"]]
    if bad:
        raise SystemExit(f"Star counts differ for constellations {bad}")

    snapshot = {
        "source": "legacy-production",
        "generatedAt": datetime.now(timezone.utc).isoformat(timespec="seconds"),
        "constellations": [{"id": c["id"], "name": c["name"]} for c in constellations],
        "stars": out_stars,
        "planets": out_planets,
    }
    OUTPUT.parent.mkdir(exist_ok=True)
    with gzip.open(OUTPUT, "wt", encoding="utf-8") as f:
        json.dump(snapshot, f, ensure_ascii=False, separators=(",", ":"))
    print(f"{OUTPUT.relative_to(ROOT)}: {len(out_stars)} stars, {len(out_planets)} planets, "
          f"{len(constellations)} constellations, {OUTPUT.stat().st_size // 1024} KB")


if __name__ == "__main__":
    main()
