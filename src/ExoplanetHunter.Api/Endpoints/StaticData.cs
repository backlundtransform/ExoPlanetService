namespace ExoplanetHunter.Api.Endpoints;

/// <summary>Serves the reference files in the Data folder (GeoJSON maps, planet images, OData metadata).</summary>
public static class StaticData
{
    public static IResult Json(string fileName) => File(fileName, "application/json; charset=utf-8");

    public static IResult File(string fileName, string contentType) =>
        Results.File(Path.Combine(AppContext.BaseDirectory, "Data", fileName), contentType);
}
