"""Record the public API contract of the running ExoPlanetService.

For every endpoint case this writes
  recordings/<case>.json   full response (status, content type, body) - a data snapshot
  shapes/<case>.json       inferred structure (fields, types, nullability) - the contract

The new service is verified against shapes/; recordings/ is reference data.

Usage:  python record.py [--base https://exoplanethunter.com] [--only substr]
"""
import argparse
import json
import sys
import time
import urllib.error
import urllib.parse
import urllib.request
from pathlib import Path

HERE = Path(__file__).parent
DELAY_SECONDS = 0.5  # be gentle with production

# (case name, path). "{sid}" / "{pid}" are resolved from earlier responses.
CASES = [
    # Stars (used by the Android app)
    ("stars_list", "/api/Stars"),
    ("stars_odata_top", "/api/Stars?$top=5"),
    ("stars_odata_top_orderby", "/api/Stars?$top=5&$orderby=name"),
    ("stars_odata_filter", "/api/Stars?$filter=constellation eq 'Lyr'&$top=5"),
    ("stars_odata_select", "/api/Stars?$top=5&$select=id,name,distance"),
    ("star_by_id", "/api/Stars/2"),
    ("star_planets", "/api/Stars/2/planets"),
    ("star_not_found", "/api/Stars/99999999"),
    # Planets
    ("planets_list", "/api/Planets"),
    ("planets_odata_top", "/api/Planets?$top=5"),
    ("planets_odata_top_orderby", "/api/Planets?$top=5&$orderby=name"),
    ("planet_by_id", "/api/Planets/{pid}"),
    # Constellations
    ("constellations_list", "/api/Constellations"),
    ("constellation_by_id", "/api/Constellations/1"),
    ("constellation_stars", "/api/Constellations/1/stars"),
    ("constellation_star", "/api/Constellations/1/stars/{sid}"),
    ("constellation_star_planets", "/api/Constellations/1/stars/{sid}/planets"),
    # ExoSolarSystems
    ("exo_planets", "/api/ExoSolarSystems/ExoPlanets"),
    ("exo_planets_odata_top", "/api/ExoSolarSystems/ExoPlanets?$top=5"),
    ("exo_system_by_name", "/api/ExoSolarSystems/GetExoSolarSystemByName?name=TRAPPIST-1"),
    ("exo_systems_per_constellation", "/api/ExoSolarSystems/GetSolarSystemPerConstellation?constellation=Lyr"),
    ("exo_systems_per_constellation_page", "/api/ExoSolarSystems/GetSolarSystemPerConstellation?constellation=Lyr&page=1"),
    ("exo_habitable_planets", "/api/ExoSolarSystems/GetHabitablePlanets"),
    ("exo_all_planets", "/api/ExoSolarSystems/GetAllPlanets"),
    ("exo_paginated", "/api/ExoSolarSystems/GetPaginatedPlanets?page=1&hab=false&moon=false&type=&key=Mass&name="),
    ("exo_paginated_habitable", "/api/ExoSolarSystems/GetPaginatedPlanets?page=1&hab=true&moon=false&type=&key=Temperature&name="),
    ("exo_images", "/api/ExoSolarSystems/GetImages"),
    # Charts
    ("chart_hr", "/api/Chart/HertzsprungRussell"),
    ("chart_hr_all", "/api/Chart/HertzsprungRussell?habitableOnly=false"),
    ("chart_types_mass", "/api/Chart/PlanetTypes?type=Mass"),
    ("chart_types_method", "/api/Chart/PlanetTypes?type=DiscoveryMetod"),
    ("chart_types_temperature", "/api/Chart/PlanetTypes?type=Temperature"),
    ("chart_types_year", "/api/Chart/PlanetTypes?type=DiscoveryYear"),
    ("chart_planet_distance", "/api/Chart/PlanetDistance"),
    ("chart_planet_distance_max", "/api/Chart/PlanetDistance?max=100"),
    ("chart_esi_distance", "/api/Chart/EsiDistance"),
    ("chart_mass_orbit", "/api/Chart/MassOrbit"),
    ("chart_mast", "/api/Chart/Mast?ra=288.397&dec=40.2132&radius=0.02"),
    # Maps, statistics, misc
    ("maps_constellation_lines", "/api/Maps/ConstellationsLines"),
    ("maps_star_markers", "/api/Maps/StarMarkers"),
    ("statistics", "/api/Statistics"),
    ("related_content", "/api/GetRelatedContent?tag=exoplanet"),
    # OData route
    ("odata_metadata", "/odata/$metadata"),
    ("odata_planets", "/odata/Planets?$top=1"),
    ("odata_stars", "/odata/Stars?$top=1"),
]


def fetch(base, path):
    url = base + urllib.parse.quote(path, safe="/?&=$',:")
    req = urllib.request.Request(url, headers={"Accept": "application/json", "User-Agent": "exoplanethunter-contract-recorder"})
    started = time.monotonic()
    try:
        with urllib.request.urlopen(req, timeout=120) as resp:
            status, ctype, raw = resp.status, resp.headers.get("Content-Type", ""), resp.read()
    except urllib.error.HTTPError as e:
        status, ctype, raw = e.code, e.headers.get("Content-Type", ""), e.read()
    elapsed_ms = int((time.monotonic() - started) * 1000)
    text = raw.decode("utf-8", errors="replace")
    try:
        body = json.loads(text)
        is_json = True
    except ValueError:
        body, is_json = text, False
    return {"url": url, "status": status, "contentType": ctype, "elapsedMs": elapsed_ms, "isJson": is_json, "body": body}


def shape_of(value):
    if value is None:
        return {"type": "null"}
    if isinstance(value, bool):
        return {"type": "boolean"}
    if isinstance(value, (int, float)):
        return {"type": "number"}
    if isinstance(value, str):
        return {"type": "string"}
    if isinstance(value, list):
        items = None
        for v in value:
            items = merge(items, shape_of(v))
        return {"type": "array", "items": items}
    return {"type": "object", "properties": {k: shape_of(v) for k, v in value.items()}}


def merge(a, b):
    """Union of two shapes, so a field seen as both null and number becomes nullable number."""
    if a is None:
        return b
    if b is None:
        return a
    if a["type"] == "null" or b["type"] == "null":
        other = b if a["type"] == "null" else a
        return {**other, "nullable": True} if other["type"] != "null" else other
    if a["type"] != b["type"]:
        return {"type": sorted({a["type"], b["type"]}), "nullable": a.get("nullable", False) or b.get("nullable", False)}
    out = dict(a)
    if a.get("nullable") or b.get("nullable"):
        out["nullable"] = True
    if a["type"] == "array":
        out["items"] = merge(a.get("items"), b.get("items"))
    elif a["type"] == "object":
        props = {}
        for key in a["properties"].keys() | b["properties"].keys():
            pa, pb = a["properties"].get(key), b["properties"].get(key)
            m = merge(pa, pb)
            if pa is None or pb is None:
                m = {**m, "optional": True}
            props[key] = m
        out["properties"] = dict(sorted(props.items()))
    return out


def first_id(response):
    body = response["body"] if response and response["isJson"] else None
    if isinstance(body, dict):
        body = body.get("stars") or body.get("planets") or body.get("value")
    if isinstance(body, list) and body and isinstance(body[0], dict):
        return body[0].get("id")
    return None


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--base", default="https://exoplanethunter.com")
    parser.add_argument("--only", help="record only cases whose name contains this")
    args = parser.parse_args()

    rec_dir, shape_dir = HERE / "recordings", HERE / "shapes"
    rec_dir.mkdir(exist_ok=True)
    shape_dir.mkdir(exist_ok=True)

    ids = {"sid": None, "pid": None}
    summary = []
    for name, path in CASES:
        if args.only and args.only not in name:
            continue
        if "{" in path:
            missing = [k for k in ids if "{" + k + "}" in path and ids[k] is None]
            if missing:
                print(f"SKIP {name}: no {missing} resolved", file=sys.stderr)
                continue
            path = path.format(**ids)

        response = fetch(args.base.rstrip("/"), path)
        if name == "constellation_stars":
            ids["sid"] = first_id(response)
        if name == "planets_odata_top":
            ids["pid"] = first_id(response)

        response["path"] = path
        (rec_dir / f"{name}.json").write_text(json.dumps(response, indent=1, ensure_ascii=False), encoding="utf-8")
        shape = {"path": path, "status": response["status"], "isJson": response["isJson"],
                 "shape": shape_of(response["body"]) if response["isJson"] else None}
        (shape_dir / f"{name}.json").write_text(json.dumps(shape, indent=2), encoding="utf-8")

        size = len(response["body"]) if isinstance(response["body"], list) else ""
        line = f"{response['status']}  {response['elapsedMs']:>6} ms  {name}  {size}"
        summary.append(line)
        print(line)
        time.sleep(DELAY_SECONDS)

    if not args.only:
        (HERE / "summary.txt").write_text("\n".join(summary) + "\n", encoding="utf-8")


if __name__ == "__main__":
    main()
