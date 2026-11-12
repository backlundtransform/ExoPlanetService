# ExoplanetHunter

The service behind [exoplanethunter.com](https://exoplanethunter.com): a public API for
confirmed exoplanets and their host stars, a 3D web app, and the source of the
[Exoplanet Hunter Android app](https://play.google.com/store/apps/details?id=com.varoid.exoplanethunter).

Star and planet data come from the [NASA Exoplanet Archive](https://exoplanetarchive.ipac.caltech.edu/).
Derived values such as habitable zones, equilibrium temperatures and the Earth Similarity Index
are computed with [CSharpNumerics](https://csnumerics.com).

## Repository layout

| Path | Contents |
|---|---|
| `src/ExoplanetHunter.Api` | .NET 10 Minimal API. Also serves the website: the landing page at `/`, the 3D app at `/astro3d/` and Swagger UI at `/swagger` |
| `src/ExoplanetHunter.Domain` | Catalogue model and snapshot format |
| `src/ExoplanetHunter.Ingest` | Builds the catalogue from the NASA Exoplanet Archive (`pscomppars`) |
| `src/ExoplanetHunter.Ingest.Cli` | Command line ingest that writes `data/catalog.json.gz` |
| `web/` | React + Three.js app, published as a PWA and as the Android app (Capacitor). See [web/README.md](web/README.md) |
| `data/` | Catalogue snapshots served by the API |
| `external/CSharpNumerics` | Git submodule, used until the habitability helpers ship on NuGet |
| `tests/` | Unit tests and the API contract check (`tests/contract`) |
| `docs/` | [API compatibility notes](docs/api-compatibility.md), the [catalogue field mapping](docs/field-mapping.md) and [Transit Lab](docs/transit-lab.md) |

## Getting started

```powershell
git clone --recurse-submodules https://github.com/backlundtransform/ExoPlanetService.git
cd ExoPlanetService
cd web; npm ci; npm run build; cd ..      # builds the 3D app into the API's wwwroot
dotnet run --project src/ExoplanetHunter.Api --urls http://localhost:5099
```

Then open http://localhost:5099. Run the tests with `dotnet test ExoplanetHunter.slnx`, and
check the API against the recorded contract with `python -I tests/contract/verify.py`.

## Refreshing the catalogue

```powershell
dotnet run --project src/ExoplanetHunter.Ingest.Cli
```

This downloads the latest confirmed planets and writes `data/catalog.json.gz`. Stars and planets
keep their ids across rebuilds. The command prints a report comparing the new catalogue with the
previous one. In production the API can also rebuild the catalogue on a schedule
(`Catalog:Refresh` in `appsettings.json`).

## API

The API keeps the routes and JSON structure of the original service, so existing clients keep
working. See [docs/api-compatibility.md](docs/api-compatibility.md) for the differences. Browse the
endpoints at https://exoplanethunter.com/swagger.

## History

The original ASP.NET Core 2.x service, with its React 16 client, blog and PHL importer, was replaced
on 2026-11-01. Its code is preserved at the git tag
[`legacy-final`](https://github.com/backlundtransform/ExoPlanetService/tree/legacy-final).
