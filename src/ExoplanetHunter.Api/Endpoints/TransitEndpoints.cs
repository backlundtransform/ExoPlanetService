namespace ExoplanetHunter.Api.Endpoints;

/// <summary>
/// /api/v2/Transits: precomputed Transit Lab results (TESS light curve, BLS periodogram and
/// transit candidates per star), written by ExoplanetHunter.Transit.Cli into data/transit.
/// The files are served as they are, so the API does not depend on the analysis code.
/// </summary>
public static class TransitEndpoints
{
    private static readonly string Directory = Path.Combine(AppContext.BaseDirectory, "Data", "transit");

    public static IEndpointRouteBuilder MapTransitEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v2/Transits").WithTags("Transit Lab");

        group.MapGet("/", () =>
        {
            var index = Path.Combine(Directory, "index.json");
            return File.Exists(index)
                ? Results.File(index, "application/json; charset=utf-8")
                : Results.Content("[]", "application/json; charset=utf-8");
        }).WithSummary("Stars with a precomputed transit search.");

        group.MapGet("/{starId:int}", (int starId) =>
        {
            var path = Path.Combine(Directory, $"{starId}.json");
            return File.Exists(path)
                ? Results.File(path, "application/json; charset=utf-8")
                : CatalogEndpoints.NotFound("Transit analysis for star", starId);
        }).WithSummary("Light curve, BLS periodogram and transit candidates for one star.");

        return app;
    }
}
