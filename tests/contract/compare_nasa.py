"""Check that fields we got from PHL can be derived from NASA Exoplanet Archive data.

Downloads pscomppars via TAP, derives the PHL-style fields with the formulas planned
for the new ingest, and compares them with the production snapshot in recordings/.

Usage:  python compare_nasa.py   (run record.py first)
"""
import csv
import io
import json
import math
import statistics
import urllib.parse
import urllib.request
from pathlib import Path

HERE = Path(__file__).parent
TAP = "https://exoplanetarchive.ipac.caltech.edu/TAP/sync"
COLUMNS = ("pl_name,hostname,pl_rade,pl_bmasse,pl_dens,pl_insol,pl_eqt,pl_orbsmax,pl_orbeccen,pl_orbper,"
           "pl_orbincl,pl_orblper,st_lum,st_teff,st_rad,st_mass,st_spectype,sy_dist,sy_vmag,ra,dec,disc_year,discoverymethod")
EARTH_DENSITY = 5.514  # g/cm3


def num(v):
    try:
        return float(v)
    except (TypeError, ValueError):
        return None


def download():
    query = urllib.parse.urlencode({"query": f"select {COLUMNS} from pscomppars", "format": "csv"})
    with urllib.request.urlopen(f"{TAP}?{query}", timeout=300) as resp:
        text = resp.read().decode("utf-8")
    return {row["pl_name"]: row for row in csv.DictReader(io.StringIO(text))}


def derive(r):
    radius, mass = num(r["pl_rade"]), num(r["pl_bmasse"])
    a, e = num(r["pl_orbsmax"]), num(r["pl_orbeccen"]) or 0.0
    log_lum = num(r["st_lum"])
    lum = 10 ** log_lum if log_lum is not None else None
    flux = num(r["pl_insol"])
    if flux is None and lum is not None and a:
        flux = lum / a ** 2
    out = {
        "density": mass / radius ** 3 if mass and radius else None,
        "gravity": mass / radius ** 2 if mass and radius else None,
        "escVel": math.sqrt(mass / radius) if mass and radius else None,
        "sFluxMean": flux,
        "teqMean": 254.6 * flux ** 0.25 if flux else None,  # Bond albedo 0.3, as PHL
        "esi": None,
        "meanDistance": a * (1 + e ** 2 / 2) if a else None,
        "luminosity": lum,
    }
    if radius and flux:
        out["esi"] = 1 - math.sqrt(0.5 * (((flux - 1) / (flux + 1)) ** 2 + ((radius - 1) / (radius + 1)) ** 2))
    return out


def main():
    planets = json.loads((HERE / "recordings" / "planets_list.json").read_text(encoding="utf-8"))["body"]
    stars = json.loads((HERE / "recordings" / "stars_list.json").read_text(encoding="utf-8"))["body"]
    star_by_name = {s["name"]: s for s in stars}
    nasa = download()

    matched = [p for p in planets if p["name"] in nasa]
    prod_names = {p["name"] for p in planets}
    print(f"NASA pscomppars: {len(nasa)} planets | production: {len(planets)} | matched by name: {len(matched)}")
    print(f"Only in production: {len(prod_names - nasa.keys())} | only in NASA: {len(nasa.keys() - prod_names)}\n")

    fields = ["density", "gravity", "escVel", "sFluxMean", "teqMean", "esi", "meanDistance", "luminosity"]
    print(f"{'field':<14}{'compared':>9}{'within 5%':>11}{'within 20%':>12}{'median rel diff':>17}")
    for field in fields:
        diffs = []
        for p in matched:
            derived = derive(nasa[p["name"]])[field]
            prod = p.get(field) if field != "luminosity" else (star_by_name.get(nasa[p["name"]]["hostname"]) or {}).get("luminosity")
            if derived is None or prod in (None, 0):
                continue
            diffs.append(abs(derived - prod) / abs(prod))
        if diffs:
            n = len(diffs)
            print(f"{field:<14}{n:>9}{sum(d <= 0.05 for d in diffs) / n:>10.0%}{sum(d <= 0.20 for d in diffs) / n:>12.0%}{statistics.median(diffs):>17.3f}")


if __name__ == "__main__":
    main()
