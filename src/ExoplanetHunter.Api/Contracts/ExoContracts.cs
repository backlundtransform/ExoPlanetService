namespace ExoplanetHunter.Api.Contracts;

// Wire contracts for /api/ExoSolarSystems and /api/Chart: the presentation model the
// legacy web client and the Android app were built against. Integer fields hold the
// ordinal of the matching enum in Legacy/LegacyEnums.cs.

public sealed record ImgDto(string Uri);

public sealed record CoordinateDto(double? Longitude, double? Latitude);

public sealed record ExoStarDto
{
    public int? NoHabPlanets { get; init; }
    public int? NoPlanets { get; init; }
    public double? Temp { get; init; }
    public string? Name { get; init; }
    public int Constellation { get; init; }
    public string? Type { get; init; }
    public int? Color { get; init; }
    public double? Mass { get; init; }
    public int Magnitude { get; init; }
    public double Radius { get; init; }
    public double RadiusSu { get; init; }
    public int? Luminosity { get; init; }
    public double? Age { get; init; }
    public double? HabZoneMin { get; init; }
    public double? HabZoneMax { get; init; }
    public string? Message { get; init; }
}

public sealed record ExoPlanetsDto
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public ImgDto? Img { get; init; }
    public double? Period { get; init; }
    public double? Hzd { get; init; }
    public double? Hzc { get; init; }
    public double? Hza { get; init; }
    public double? Hzi { get; init; }
    public string? Type { get; init; }
    public int Comp { get; init; }
    public int Atmosphere { get; init; }
    public int HabType { get; init; }
    public double? MeanDistance { get; init; }
    public double? Distance { get; init; }
    public double? Esi { get; init; }
    public double? Sph { get; init; }
    public double? DiscYear { get; init; }
    public int DiscMethod { get; init; }
    public double Radius { get; init; }
    public double? Eccentricity { get; init; }
    public double RadiusEu { get; init; }
    public CoordinateDto? Coordinate { get; init; }
    public double? StarDistance { get; init; }
    public ExoStarDto? Star { get; init; }
    public double? Temp { get; init; }
    public double? TempMin { get; init; }
    public double? Mass { get; init; }
    public double? Density { get; init; }
    public double? Gravity { get; init; }
    public double? SurfacePressure { get; init; }
    public double? EscapeVelocity { get; init; }
    public int MassType { get; init; }
    public double? TempMax { get; init; }
    public int TempZone { get; init; }
    public bool? Hab { get; init; }
    public bool? Moons { get; init; }
    public string? Message { get; init; }
}

public sealed record ExoSystemPlanetsDto(int Id, string Name, ImgDto? Img, double Radius, double? Eccentricity, double? StarDistance);

public sealed record ExoSolarSystemDto
{
    public string? Name { get; init; }
    public CoordinateDto? Coordinate { get; init; }
    public int? Color { get; init; }
    public int? Luminosity { get; init; }
    public double Radius { get; init; }
    public double? HabZoneMin { get; init; }
    public double? HabZoneMax { get; init; }
    public IReadOnlyList<ExoSystemPlanetsDto>? Planets { get; init; }
    public string? Message { get; init; }
}

public sealed record HertzsprungRussellDto(string Title, string? Type, double? Lum, string? Color, double? Size, double? Temp, string? Constellation);

public sealed record PlanetTypesDto(string Title, int Value);

public sealed record PlanetDistanceDto(string Title, double? Distance, double? Angle);

public sealed record EsiDistanceDto(double Distance, string? StarName, string? PlanetName, double Esi, string Color);

public sealed record MassOrbitDto(double Mass, string? StarName, string? PlanetName, string Color, double Orbit);
