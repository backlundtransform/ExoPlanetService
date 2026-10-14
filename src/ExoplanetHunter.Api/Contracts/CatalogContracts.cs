using System.Text.Json.Serialization;
using ExoplanetHunter.Domain.Catalog;

namespace ExoplanetHunter.Api.Contracts;

// Wire contracts for /api/Stars, /api/Planets and /api/Constellations.
// Property names and order match the legacy service (see tests/contract/shapes).

public sealed record StarDto(
    int Id, string Name, string? NameHD, string? NameHIP, string? Constellation, string? Type,
    double? Mass, double? Radius, double? Teff, double? Luminosity, double? FeH, double? Age,
    double? ApparMag, double? Distance, double? Ra, double? Dec, double? MagfromPlanet, double? SizefromPlanet,
    double? HabZoneMin, double? HabZoneMax, bool? HabCat, int? Planets, string? Message = null)
{
    public static StarDto From(Star s, int planets) => new(
        s.Id, s.Name, s.NameHD, s.NameHIP, s.Constellation, s.Type,
        s.Mass, s.Radius, s.Teff, s.Luminosity, s.FeH, s.Age,
        s.ApparMag, s.Distance, s.Ra, s.Dec, s.MagfromPlanet, s.SizefromPlanet,
        s.HabZoneMin, s.HabZoneMax, s.HabCat, planets);
}

public sealed record PlanetDto(
    int Id, string Name, string? NameKepler, string? NameKOI, string? ZoneClass, string? MassClass,
    string? CompositionClass, string? AtmosphereClass, string? HabitableClass,
    double? MinMass, double? Mass, double? MaxMass, double? Radius, double? Density, double? Gravity, double? EscVel,
    double? SFluxMin, double? SFluxMean, double? SFluxMax, double? TeqMin, double? TeqMean, double? TeqMax,
    double? TsMin, double? TsMean, double? TsMax, double? SurfPress, double? Mag, double? ApparSize,
    double? Period, double? SemMajorAxis, double? Eccentricity, double? MeanDistance, double? Inclination, double? Omega,
    double? Hzd, double? Hzc, double? Hza, double? Hzi, double? Sph, double? IntEsi, double? SurfEsi, double? Esi,
    bool? Habitable, bool? HabMoon, bool? Confirmed,
    [property: JsonPropertyName("disc_Method")] string? DiscoveryMethod,
    [property: JsonPropertyName("disc_Year")] int? DiscoveryYear,
    string? Message = null)
{
    public static PlanetDto From(Planet p) => new(
        p.Id, p.Name, p.NameKepler, p.NameKOI, p.ZoneClass, p.MassClass,
        p.CompositionClass, p.AtmosphereClass, p.HabitableClass,
        p.MinMass, p.Mass, p.MaxMass, p.Radius, p.Density, p.Gravity, p.EscVel,
        p.SFluxMin, p.SFluxMean, p.SFluxMax, p.TeqMin, p.TeqMean, p.TeqMax,
        p.TsMin, p.TsMean, p.TsMax, p.SurfPress, p.Mag, p.ApparSize,
        p.Period, p.SemMajorAxis, p.Eccentricity, p.MeanDistance, p.Inclination, p.Omega,
        p.Hzd, p.Hzc, p.Hza, p.Hzi, p.Sph, p.IntEsi, p.SurfEsi, p.Esi,
        p.Habitable, p.HabMoon, p.Confirmed, p.DiscoveryMethod, p.DiscoveryYear);
}

public sealed record StarPlanetsDto(int Id, string Name, IReadOnlyList<PlanetDto> Planets, string? Message = null);

public sealed record ConstellationDto(int Id, string Name, int? Stars, string? Message = null);

public sealed record ConstellationStarsDto(int Id, string Name, IReadOnlyList<StarDto> Stars, string? Message = null);

public sealed record StatisticsDto(int ConfirmedPlanets, int ConfirmedHabitablePlanets, int PossibleHabitableMoons, DateTimeOffset DateUpdated);

public sealed record RelatedContentDto(string Image, string Description, string Url, DateTime Created);
