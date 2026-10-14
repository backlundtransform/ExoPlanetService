using ExoplanetHunter.Api.Catalog;
using ExoplanetHunter.Api.Contracts;

namespace ExoplanetHunter.Api.Endpoints;

/// <summary>/api/Statistics, /api/Maps, /api/GetRelatedContent and the /odata routes.</summary>
public static class MiscEndpoints
{
    public static IEndpointRouteBuilder MapMiscEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/Statistics", (CatalogStore store) =>
        {
            var catalog = store.Current.Catalog;
            return new StatisticsDto(
                ConfirmedPlanets: catalog.Planets.Count,
                ConfirmedHabitablePlanets: catalog.Planets.Count(p => p.Habitable == true),
                PossibleHabitableMoons: catalog.Planets.Count(p => p.HabMoon == true),
                DateUpdated: catalog.GeneratedAt);
        }).WithTags("Statistics");

        var maps = app.MapGroup("/api/Maps").WithTags("Maps");
        maps.MapGet("/ConstellationsLines", () => StaticData.Json("constellations.json"));
        maps.MapGet("/StarMarkers", () => StaticData.Json("stars.json"));

        // Related blog posts. The blog has been retired, so there is never related content.
        app.MapGet("/api/GetRelatedContent", (string? tag) => Array.Empty<RelatedContentDto>()).WithTags("Content");

        var odata = app.MapGroup("/odata").WithTags("OData");
        odata.MapGet("/$metadata", () => StaticData.File("odata-metadata.xml", "application/xml; charset=utf-8"));
        odata.MapGet("/Planets", (HttpRequest request, CatalogStore store) =>
            ODataCollection(request, "Planets", store.Current.Catalog.Planets.Select(PlanetDto.From)));
        odata.MapGet("/Stars", (HttpRequest request, CatalogStore store) =>
            ODataCollection(request, "Stars", store.Current.Catalog.Stars.Select(store.Current.ToDto)));

        return app;
    }

    private static IResult ODataCollection<T>(HttpRequest request, string entitySet, IEnumerable<T> items)
    {
        var result = CatalogEndpoints.List(request, items);
        if (result is not IValueHttpResult { Value: IEnumerable<T> page })
            return result;

        var context = $"{request.Scheme}://{request.Host}/odata/$metadata#{entitySet}";
        return Results.Ok(new Dictionary<string, object> { ["@odata.context"] = context, ["value"] = page });
    }
}
