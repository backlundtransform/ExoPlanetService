using System.Globalization;
using ExoplanetHunter.Api.Catalog;
using ExoplanetHunter.Api.Contracts;
using ExoplanetHunter.Api.Legacy;
using ExoplanetHunter.Api.Query;

namespace ExoplanetHunter.Api.Endpoints;

/// <summary>/api/Chart: aggregates for the dashboard charts.</summary>
public static class ChartEndpoints
{
    public static IEndpointRouteBuilder MapChartEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/Chart").WithTags("Chart");

        group.MapGet("/HertzsprungRussell", (string? habitableOnly, CatalogStore store) =>
        {
            var catalog = store.Current.Catalog;
            bool onlyHabitable = Lenient.Bool(habitableOnly, fallback: true);

            // Legacy filter, operator precedence included: stars missing any of mass,
            // luminosity, temperature or type are always kept; complete stars are kept
            // when habitableOnly is false or when they host a habitable planet.
            return catalog.Stars
                .Where(s =>
                {
                    bool complete = s.Mass is not null && s.Luminosity is not null && s.Teff is not null && s.Type is not null;
                    return !(complete && onlyHabitable) || catalog.PlanetsOf(s).Any(p => p.Habitable == true);
                })
                .Select(s => new HertzsprungRussellDto(s.Name, s.Type, s.Luminosity,
                    LegacyPresentation.HertzsprungRussellColor(s.Type),
                    s.Mass is null ? null : (double)((decimal)s.Mass.Value * 100), s.Teff, s.Constellation))
                .Where(d => d.Color is not null);
        });

        group.MapGet("/PlanetTypes", (string? type, CatalogStore store) =>
        {
            var planets = store.Current.Catalog.Planets;
            var groups = Lenient.Enum(type, ChartType.Mass) switch
            {
                ChartType.DiscoveryMetod => planets
                    .Where(p => LegacyEnum.Parse<DiscoveryMethodType>(p.DiscoveryMethod) != DiscoveryMethodType.Nodata)
                    .GroupBy(p => p.DiscoveryMethod!),
                ChartType.DiscoveryYear => planets
                    .Where(p => p.DiscoveryYear is not null)
                    .OrderBy(p => p.DiscoveryYear)
                    .GroupBy(p => p.DiscoveryYear!.Value.ToString(CultureInfo.InvariantCulture)),
                ChartType.Temperature => planets
                    .Where(p => LegacyEnum.Parse<TemperatureZone>(p.ZoneClass) != TemperatureZone.Nodata)
                    .GroupBy(p => p.ZoneClass!),
                _ => planets
                    .Where(p => LegacyEnum.Parse<MassType>(p.MassClass) != MassType.Nodata)
                    .GroupBy(p => p.MassClass!),
            };

            return groups.Select(g => new PlanetTypesDto(g.Key, g.Count()));
        });

        group.MapGet("/PlanetDistance", (string? max, CatalogStore store) =>
        {
            var catalog = store.Current.Catalog;
            double? maxLightYears = Lenient.NullableDouble(max);

            return catalog.Planets
                .Where(p => p.Habitable == true)
                .Select(p => (Planet: p, Star: catalog.StarOf(p)))
                .Where(x => maxLightYears is null || (x.Star.Distance is not null && LegacyPresentation.LightYears(x.Star.Distance) < maxLightYears))
                .Select(x => new PlanetDistanceDto(x.Planet.Name, LegacyPresentation.LightYears(x.Star.Distance), x.Star.Ra ?? 0));
        });

        group.MapGet("/EsiDistance", (CatalogStore store) =>
            store.Current.ExoPlanets
                .Where(p => p.Distance is not null && p.Esi is not null && p.Hab == true)
                .Select(p => new EsiDistanceDto(p.Distance!.Value, p.Star!.Name, p.Name, p.Esi!.Value, SeriesColor(p.Star.Name))));

        group.MapGet("/MassOrbit", (CatalogStore store) =>
            store.Current.ExoPlanets
                .Where(p => p.Mass is not null && p.Period is not null)
                .Select(p => new MassOrbitDto(p.Mass!.Value, p.Star!.Name, p.Name, SeriesColor(p.Star.Name), p.Period!.Value)));

        // The legacy MAST proxy depended on a MAST service that no longer exists and has
        // returned 500 for a long time. Light curves come back with the Transit Lab (v1.1).
        group.MapGet("/Mast", () => Results.Problem(
            "The MAST light-curve proxy has been retired. Light curves will be served by the Transit Lab API.",
            statusCode: StatusCodes.Status501NotImplemented));

        return app;
    }

    /// <summary>
    /// One colour per star, so all planets of a system share a series colour. The legacy
    /// service picked random colours per request; this is stable across requests.
    /// </summary>
    private static string SeriesColor(string? starName)
    {
        uint hash = 2166136261;
        foreach (char c in starName ?? "")
            hash = (hash ^ c) * 16777619;
        return $"#{hash & 0xFFFFFF:X6}";
    }
}
