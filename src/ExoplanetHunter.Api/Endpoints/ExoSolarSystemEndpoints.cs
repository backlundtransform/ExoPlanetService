using ExoplanetHunter.Api.Catalog;
using ExoplanetHunter.Api.Contracts;
using ExoplanetHunter.Api.Legacy;
using ExoplanetHunter.Api.Query;

namespace ExoplanetHunter.Api.Endpoints;

/// <summary>/api/ExoSolarSystems: the presentation endpoints used by the web client and the Android app.</summary>
public static class ExoSolarSystemEndpoints
{
    private const int PageSize = 30;

    public static IEndpointRouteBuilder MapExoSolarSystemEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/ExoSolarSystems").WithTags("ExoSolarSystems");

        group.MapGet("/ExoPlanets", (HttpRequest request, CatalogStore store) =>
            CatalogEndpoints.List(request, store.Current.ExoPlanets));

        group.MapGet("/GetExoSolarSystemByName", (string? name, CatalogStore store) =>
            SolarSystem(name, store.Current) is { } system
                ? Results.Ok(system)
                : CatalogEndpoints.NotFound("Star", name ?? ""));

        group.MapGet("/GetSolarSystemPerConstellation", (string? constellation, string? page, CatalogStore store) =>
        {
            var code = (int)Lenient.Enum(constellation, ConstellationCode.And);
            var systems = store.Current.ExoPlanets
                .Where(p => p.Star!.Constellation == code)
                .GroupBy(p => p.Star!.Name)
                .Select(g => new ExoSolarSystemDto { Name = g.Key, Color = g.First().Star!.Color, Radius = g.First().Star!.Radius });

            return Lenient.NullableInt(page) is { } p ? systems.Skip(p * PageSize).Take(PageSize) : systems;
        });

        group.MapGet("/GetHabitablePlanets", (CatalogStore store) =>
            store.Current.ExoPlanets
                .Where(p => p.Hab == true && p.Name != "Tellus")
                .OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase) // legacy query ran on the name index
                .Select(p => new ExoPlanetsDto { Name = p.Name, Coordinate = p.Coordinate, Star = p.Star }));

        group.MapGet("/GetAllPlanets", (CatalogStore store) =>
            store.Current.ExoPlanets.Select(p => new ExoPlanetsDto { Hab = p.Hab == true, Coordinate = p.Coordinate, Star = p.Star }));

        group.MapGet("/GetPaginatedPlanets", (string? page, string? hab, string? moon, string? type, string? key, string? name, CatalogStore store) =>
            Paginated(store.Current.ExoPlanets, Lenient.Int(page), Lenient.Bool(hab), Lenient.Bool(moon), type,
                Lenient.Enum(key, ChartType.Mass), name));

        group.MapGet("/GetImages", () => StaticData.Json("colors.json"));

        return app;
    }

    private static ExoSolarSystemDto? SolarSystem(string? name, CatalogView view)
    {
        var planets = view.ExoPlanets.Where(p => p.Star!.Name == name).ToArray();
        if (planets.Length == 0)
            return null;

        var star = planets[0].Star!;
        return new ExoSolarSystemDto
        {
            Name = star.Name,
            Color = star.Color,
            Luminosity = star.Luminosity,
            HabZoneMax = LegacyPresentation.SystemViewDistance(planets, star.HabZoneMax),
            HabZoneMin = LegacyPresentation.SystemViewDistance(planets, star.HabZoneMin),
            Radius = star.Radius,
            Coordinate = planets[0].Coordinate,
            Planets = planets
                .OrderBy(p => p.MeanDistance)
                .Select(p => new ExoSystemPlanetsDto(p.Id, p.Name!, p.Img, p.Radius, p.Eccentricity,
                    LegacyPresentation.SystemViewDistance(planets, p.MeanDistance) + p.Radius))
                .ToArray(),
        };
    }

    private static IEnumerable<ExoPlanetsDto> Paginated(
        IEnumerable<ExoPlanetsDto> planets, int page, bool hab, bool moon, string? type, ChartType key, string? name)
    {
        // Same precedence as the legacy service: name, then habitable/moon flags, then type.
        if (!string.IsNullOrEmpty(name))
        {
            planets = planets.Where(p => p.Name!.Contains(name, StringComparison.OrdinalIgnoreCase));
            if (hab) planets = planets.Where(p => p.Hab == true);
            else if (moon) planets = planets.Where(p => p.Moons == true);
        }
        else if (hab)
        {
            planets = planets.Where(p => p.Hab == true);
        }
        else if (moon)
        {
            planets = planets.Where(p => p.Moons == true);
        }
        else if (!string.IsNullOrEmpty(type))
        {
            planets = key switch
            {
                ChartType.Mass => planets.Where(p => p.MassType == (int)LegacyEnum.Parse<MassType>(type)),
                ChartType.DiscoveryMetod => planets.Where(p => p.DiscMethod == (int)LegacyEnum.Parse<DiscoveryMethodType>(type)),
                ChartType.Temperature => planets.Where(p => p.TempZone == (int)LegacyEnum.Parse<TemperatureZone>(type)),
                _ => planets,
            };
        }

        return planets.OrderByDescending(p => p.DiscYear).Skip(page * PageSize).Take(PageSize);
    }
}
