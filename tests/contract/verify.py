"""Verify a running API against the recorded legacy contract.

For every case in record.py:
  - shape: every field the legacy response had must exist with a compatible type
  - values: when the new API serves the legacy snapshot, bodies must match the recordings
    (floats compared with a relative tolerance)

Known, deliberate deviations are listed in DEVIATIONS and reported separately.

Usage:  python verify.py --base http://localhost:5099 [--values] [--only substr]
Exit code is 1 if any case fails.
"""
import argparse
import json
import math
import sys
from pathlib import Path

HERE = Path(__file__).parent
sys.path.insert(0, str(HERE))

import record  # noqa: E402

# case -> reason. Status codes and bodies of these cases are not compared.
DEVIATIONS = {
    "star_not_found": "unknown id returns 404 ProblemDetails instead of an empty 204",
    "stars_odata_top_orderby": "$orderby now works instead of returning 204",
    "planets_odata_top_orderby": "$orderby now works instead of returning 204",
    "stars_odata_filter": "$filter is rejected with 400 instead of returning 204",
    "stars_odata_select": "$select is rejected with 400 instead of returning 204",
    "chart_mast": "retired MAST proxy returns 501 instead of 500",
    "odata_planets": "OData collection works instead of returning 500",
    "odata_stars": "OData collection works instead of returning 500",
}

# Cases where the legacy service returned its JSON array as a string that is cut off
# after ~90 KB (the endpoint fails mid-response in production). The new API returns the
# full array; the complete items of the recording are compared with its first items.
DECODED_STRING = {"chart_hr", "chart_hr_all"}

# case -> field names whose values legitimately differ (shape still checked).
VALUE_EXEMPT = {
    "chart_esi_distance": {"color"},  # random per request in legacy, stable per star now
    "chart_mass_orbit": {"color"},
    "statistics": {"dateUpdated"},  # legacy always returned 0001-01-01
}


def compatible(expected, actual, path, problems):
    """Check that `actual` (a value) fits the recorded shape `expected`."""
    if expected is None or actual is None:
        return
    types = expected["type"] if isinstance(expected["type"], list) else [expected["type"]]
    if "null" in types and len(types) == 1:
        return  # field was always null in the recording: any type is acceptable
    actual_type = record.shape_of(actual)["type"]
    if actual_type not in types:
        problems.append(f"{path}: expected {'/'.join(types)}, got {actual_type}")
        return
    if actual_type == "object":
        for key, sub in expected["properties"].items():
            if key not in actual:
                if not sub.get("optional"):
                    problems.append(f"{path}.{key}: missing")
                continue
            compatible(sub, actual[key], f"{path}.{key}", problems)
    elif actual_type == "array" and expected.get("items"):
        for i, item in enumerate(actual[:200]):
            compatible(expected["items"], item, f"{path}[{i}]", problems)


def equal(expected, actual, path, exempt, problems, limit=10):
    if len(problems) >= limit:
        return
    if path.rsplit(".", 1)[-1] in exempt:
        return
    if isinstance(expected, float) or isinstance(actual, float):
        if isinstance(expected, (int, float)) and isinstance(actual, (int, float)) and not isinstance(expected, bool):
            if math.isclose(expected, actual, rel_tol=1e-9, abs_tol=1e-9):
                return
        problems.append(f"{path}: expected {expected!r}, got {actual!r}")
        return
    if isinstance(expected, dict) and isinstance(actual, dict):
        for key in expected:
            if key not in actual:
                problems.append(f"{path}.{key}: missing")
            else:
                equal(expected[key], actual[key], f"{path}.{key}", exempt, problems, limit)
        return
    if isinstance(expected, list) and isinstance(actual, list):
        if len(expected) != len(actual):
            problems.append(f"{path}: expected {len(expected)} items, got {len(actual)}")
            return
        for i, (e, a) in enumerate(zip(expected, actual)):
            equal(e, a, f"{path}[{i}]", exempt, problems, limit)
        return
    if expected != actual:
        problems.append(f"{path}: expected {str(expected)[:80]!r}, got {str(actual)[:80]!r}")


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--base", default="http://localhost:5099")
    parser.add_argument("--values", action="store_true", help="also compare values with recordings/")
    parser.add_argument("--only")
    args = parser.parse_args()

    ids = {"sid": None, "pid": None}
    failures = 0
    for name, path in record.CASES:
        if args.only and args.only not in name:
            continue
        shape_file = HERE / "shapes" / f"{name}.json"
        if not shape_file.exists():
            print(f"SKIP  {name}: no recorded shape")
            continue
        recorded = json.loads(shape_file.read_text(encoding="utf-8"))
        path = recorded["path"]  # the exact path that was recorded, ids resolved

        response = record.fetch(args.base.rstrip("/"), path)
        if name in DEVIATIONS:
            print(f"DEV   {name}: {response['status']} - {DEVIATIONS[name]}")
            continue

        problems = []
        if name in DECODED_STRING:
            recording = HERE / "recordings" / f"{name}.json"
            truncated = json.loads(recording.read_text(encoding="utf-8"))["body"] if recording.exists() else "[]"
            expected = json.loads(truncated[:truncated.rfind("},{") + 1] + "]") if truncated != "[]" else []
            if response["status"] != 200 or not isinstance(response["body"], list):
                problems.append(f"expected a JSON array, got {response['status']}")
            elif not expected:
                pass  # no recording available (CI): the array check above is all we can do
            elif len(response["body"]) < len(expected):
                problems.append(f"expected at least {len(expected)} items, got {len(response['body'])}")
            else:
                print(f"        legacy response cut off after {len(expected)} items; new API returns {len(response['body'])}")
                response["body"] = response["body"][:len(expected)]
                compatible(record.shape_of(expected), response["body"], "$", problems)
                if args.values and not problems:
                    equal(expected, response["body"], "$", set(), problems)
        elif response["status"] != recorded["status"]:
            problems.append(f"status {response['status']}, expected {recorded['status']}")
        elif recorded["isJson"]:
            compatible(recorded["shape"], response["body"], "$", problems)
            if args.values and not problems:
                expected = json.loads((HERE / "recordings" / f"{name}.json").read_text(encoding="utf-8"))["body"]
                equal(expected, response["body"], "$", VALUE_EXEMPT.get(name, set()), problems)

        if problems:
            failures += 1
            print(f"FAIL  {name}")
            for p in problems[:10]:
                print(f"        {p}")
        else:
            print(f"OK    {name}")

    print(f"\n{failures} failing case(s)")
    sys.exit(1 if failures else 0)


if __name__ == "__main__":
    main()
