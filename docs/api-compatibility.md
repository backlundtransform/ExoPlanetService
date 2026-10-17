# API v1 compatibility notes

The .NET 10 service (`src/ExoplanetHunter.Api`) keeps every public route of the legacy
ExoPlanetService with the same URLs, field names and JSON structure. On the same data it
returns identical values for all endpoints except those listed below.

Compatibility is verified by `tests/contract/verify.py` against responses recorded from
production on 2026-10-11.

## Behaviour changes

| Endpoint | Before | Now |
|---|---|---|
| `GET /api/Stars/{id}`, `/api/Planets/{id}` and other lookups by id or name, when nothing matches | `204 No Content`, or `500` for `GetExoSolarSystemByName` | `404` with a Problem Details body |
| `$orderby` on `/api/Stars`, `/api/Planets`, `/api/ExoSolarSystems/ExoPlanets` | `204 No Content` | Works, e.g. `$orderby=distance desc,name` |
| `$filter`, `$select`, `$expand` and other OData options | `204 No Content` | `400` with a message naming the supported options (`$top`, `$skip`, `$orderby`) |
| `$skip` on `/api/ExoSolarSystems/ExoPlanets` | Skipped twice the requested number of items | Skips the requested number |
| `GET /api/Chart/HertzsprungRussell` | A JSON string holding an array that was cut off after ~90 KB | The complete JSON array |
| `GET /api/Chart/EsiDistance`, `/api/Chart/MassOrbit` | `color` was random on every request | `color` is stable per star |
| `GET /api/Statistics` | `dateUpdated` was always `0001-01-01T00:00:00` | The time the catalogue was generated |
| `GET /odata/Planets`, `/odata/Stars` | `500` | OData JSON (`@odata.context`, `value`) with `$top`, `$skip` and `$orderby` |
| `GET /api/Chart/Mast` | `500` after ~18 s (the MAST service it proxied no longer exists) | `501 Not Implemented`. Light curves will come from the Transit Lab API |
| `GET /api/GetRelatedContent` | Related blog posts | Always `[]`, since the blog has been retired |
| `POST /api/update`, `GET /api/newupdate` | Admin-only import triggers | Removed. The catalogue is refreshed by the ingest job |

## Data

The catalogue now comes from the NASA Exoplanet Archive (`pscomppars`) instead of the PHL
catalogue. With `Catalog:Refresh:Enabled` it is rebuilt on a schedule (weekly by default),
and ids stay the same across rebuilds. It has more planets (6,453 instead
of 5,649) and corrects the coordinates of about 1,170 stars that were wrong before. Derived
fields follow the same methodology, see `docs/field-mapping.md`.

`star.noHabPlanets` (in the `ExoSolarSystems` responses) now holds the number of potentially
habitable planets for every star. Before, it was only set for the Sun.

## New

- `GET /health`: health check.
- `GET /openapi/v1.json`: OpenAPI description.
- CORS: any origin may call the API with `GET`.
- Responses are compressed (Brotli or gzip). For example, `GetAllPlanets` goes from 4.5 MB to 0.8 MB.
- Rate limiting: 300 requests per minute per client IP. Over the limit, the API returns `429`.
