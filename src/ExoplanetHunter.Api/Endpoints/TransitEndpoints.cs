using ExoplanetHunter.Api.Transit;

namespace ExoplanetHunter.Api.Endpoints;

/// <summary>
/// /api/v2/Transits: precomputed Transit Lab results (TESS light curve, BLS periodogram and
/// transit candidates per star), written by ExoplanetHunter.Transit.Cli into data/transit.
/// The files are served as they are, so the API does not depend on the analysis code. They come
/// from the deployment or, with Transit:Refresh enabled, from GitHub (TransitRefreshService).
/// </summary>
public static class TransitEndpoints
{
    public static IEndpointRouteBuilder MapTransitEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v2/Transits").WithTags("Transit Lab");

        group.MapGet("/", (TransitStore store) =>
        {
            var index = store.PathOf(TransitIndex.FileName);
            return File.Exists(index)
                ? Results.File(index, "application/json; charset=utf-8")
                : Results.Content("[]", "application/json; charset=utf-8");
        }).WithSummary("Stars with a precomputed transit search.");

        group.MapGet("/{starId:int}", (int starId, TransitStore store) =>
        {
            var path = store.PathOf($"{starId}.json");
            return File.Exists(path)
                ? Results.File(path, "application/json; charset=utf-8")
                : CatalogEndpoints.NotFound("Transit analysis for star", starId);
        }).WithSummary("Light curve, BLS periodogram and transit candidates for one star.");

        return app;
    }
}
