using ExoplanetHunter.Api.Catalog;
using ExoplanetHunter.Api.Contracts;
using ExoplanetHunter.Api.Query;

namespace ExoplanetHunter.Api.Endpoints;

/// <summary>/api/Stars, /api/Planets and /api/Constellations.</summary>
public static class CatalogEndpoints
{
    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        var stars = app.MapGroup("/api/Stars").WithTags("Stars");

        stars.MapGet("/", (HttpRequest request, CatalogStore store) =>
            List(request, store.Current.Catalog.Stars.Select(store.Current.ToDto)));

        stars.MapGet("/{id:int}", (int id, CatalogStore store) =>
            store.Current.Catalog.FindStar(id) is { } star
                ? Results.Ok(store.Current.ToDto(star))
                : NotFound("Star", id));

        stars.MapGet("/{id:int}/planets", (int id, CatalogStore store) => StarPlanets(id, store));

        var planets = app.MapGroup("/api/Planets").WithTags("Planets");

        planets.MapGet("/", (HttpRequest request, CatalogStore store) =>
            List(request, store.Current.Catalog.Planets.Select(PlanetDto.From)));

        planets.MapGet("/{id:int}", (int id, CatalogStore store) =>
            store.Current.Catalog.FindPlanet(id) is { } planet
                ? Results.Ok(PlanetDto.From(planet))
                : NotFound("Planet", id));

        var constellations = app.MapGroup("/api/Constellations").WithTags("Constellations");

        constellations.MapGet("/", (CatalogStore store) =>
        {
            var catalog = store.Current.Catalog;
            return catalog.Constellations.Select(c => new ConstellationDto(c.Id, c.Name, catalog.StarsIn(c).Count()));
        });

        constellations.MapGet("/{cid:int}", (int cid, CatalogStore store) =>
        {
            var catalog = store.Current.Catalog;
            return catalog.FindConstellation(cid) is { } c
                ? Results.Ok(new ConstellationDto(c.Id, c.Name, catalog.StarsIn(c).Count()))
                : NotFound("Constellation", cid);
        });

        constellations.MapGet("/{cid:int}/stars", (int cid, CatalogStore store) =>
        {
            var view = store.Current;
            return view.Catalog.FindConstellation(cid) is { } c
                ? Results.Ok(new ConstellationStarsDto(c.Id, c.Name, view.Catalog.StarsIn(c).Select(view.ToDto).ToArray()))
                : NotFound("Constellation", cid);
        });

        // The legacy routes ignore the constellation id and look the star up directly.
        constellations.MapGet("/{cid:int}/stars/{sid:int}", (int sid, CatalogStore store) =>
            store.Current.Catalog.FindStar(sid) is { } star
                ? Results.Ok(store.Current.ToDto(star))
                : NotFound("Star", sid));

        constellations.MapGet("/{cid:int}/stars/{sid:int}/planets", (int sid, CatalogStore store) => StarPlanets(sid, store));

        return app;
    }

    internal static IResult List<T>(HttpRequest request, IEnumerable<T> items)
    {
        if (!ODataQuery.TryParse(request.Query, out var query, out var error) || !query.TryApply(items, out items, out error))
            return Results.Problem(error, statusCode: StatusCodes.Status400BadRequest);

        return Results.Ok(items);
    }

    internal static IResult NotFound(string kind, object id) =>
        Results.Problem($"{kind} '{id}' was not found.", statusCode: StatusCodes.Status404NotFound);

    private static IResult StarPlanets(int id, CatalogStore store)
    {
        var catalog = store.Current.Catalog;
        if (catalog.FindStar(id) is not { } star)
            return NotFound("Star", id);

        return Results.Ok(new StarPlanetsDto(star.Id, star.Name, catalog.PlanetsOf(star).Select(PlanetDto.From).ToArray()));
    }
}
