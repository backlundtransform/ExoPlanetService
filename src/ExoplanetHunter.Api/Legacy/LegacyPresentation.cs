using ExoplanetHunter.Api.Contracts;
using ExoplanetHunter.Domain.Catalog;

namespace ExoplanetHunter.Api.Legacy;

/// <summary>
/// Display values the legacy service derived from the catalogue: planet textures, star
/// colour classes, clamped display radii and Celsius temperatures. Ported rule for rule
/// from ExoPlanetHunter.Service so existing clients render the same.
/// Arithmetic is done in decimal, as in the legacy code, so values serialize identically.
/// </summary>
public static class LegacyPresentation
{
    private const decimal LightYearsPerParsec = 3.26156m;
    private const decimal ZeroCelsius = 273.15m;

    private static readonly Dictionary<char, string> HrColors = new()
    {
        ['O'] = "#19147A", ['B'] = "#1D6AAE", ['A'] = "#A3D4E2", ['F'] = "#ECF2EE", ['G'] = "#FCDF21",
        ['K'] = "#FB830B", ['M'] = "#FB5908", ['L'] = "#FB110A", ['T'] = "#9b0000",
    };

    public static ExoPlanetsDto ToExoPlanet(Planet p, Star star, int planetsOfStar) => new()
    {
        Id = p.Id,
        Name = EmptyToNull(p.Name),
        Img = new ImgDto(PlanetTexture(p)),
        Type = EmptyToNull(p.MassClass),
        Esi = p.Esi,
        Hab = p.Habitable,
        Gravity = p.Gravity,
        TempZone = (int)LegacyEnum.Parse<TemperatureZone>(p.ZoneClass),
        Density = p.Density,
        Period = p.Period,
        SurfacePressure = p.SurfPress,
        RadiusEu = p.Radius ?? 0,
        EscapeVelocity = p.EscVel,
        Distance = LightYears(star.Distance),
        Temp = Celsius(p.TsMean),
        TempMax = Celsius(p.TsMax),
        TempMin = Celsius(p.TsMin),
        Coordinate = Coordinate(star),
        DiscYear = p.DiscoveryYear,
        Comp = (int)CompositionType.Nodata,
        Mass = p.Mass,
        HabType = (int)LegacyEnum.Parse<HabitableType>(p.HabitableClass),
        MassType = (int)LegacyEnum.Parse<MassType>(p.MassClass),
        DiscMethod = (int)LegacyEnum.Parse<DiscoveryMethodType>(p.DiscoveryMethod),
        Radius = Clamp(15m * D(p.Radius), 10, 50) ?? 30,
        MeanDistance = p.MeanDistance,
        Eccentricity = p.Eccentricity,
        Star = ToExoStar(star, planetsOfStar),
    };

    public static ExoStarDto ToExoStar(Star s, int planets) => new()
    {
        Constellation = s.Constellation is null ? (int)ConstellationCode.Nodata : (int)LegacyEnum.Parse<ConstellationCode>(s.Constellation),
        Radius = StarDisplayRadius(s),
        Color = StarColor(s.Type),
        HabZoneMax = s.HabZoneMax,
        HabZoneMin = s.HabZoneMin,
        Name = EmptyToNull(s.Name),
        Temp = Celsius(s.Teff),
        Age = s.Age,
        Luminosity = (int)LuminosityClassOf(s.Type),
        NoHabPlanets = s.NoPlanetsHZ,
        RadiusSu = s.Radius ?? 0,
        Mass = s.Mass,
        NoPlanets = planets,
        Type = EmptyToNull(s.Type),
        Magnitude = s.ApparMag < 7 ? (int)MagnitudeType.visible : (int)MagnitudeType.unvisible,
    };

    public static CoordinateDto Coordinate(Star s) =>
        new(Longitude: s.Ra is null ? null : (double)(-15m * (D(s.Ra)!.Value - 12m)), Latitude: s.Dec);

    public static double StarDisplayRadius(Star s) => Clamp(75m * D(s.Radius), 60, 100) ?? 75;

    /// <summary>Distance in light-years; unknown distances become 0, as in the legacy cache.</summary>
    public static double LightYears(double? parsecs) => (double)(LightYearsPerParsec * (D(parsecs) ?? 0));

    public static string? HertzsprungRussellColor(string? type) =>
        string.IsNullOrEmpty(type) ? null : HrColors.GetValueOrDefault(type[0]);

    /// <summary>0 = O/B/A, 1 = F, 2 = G/K, 3 = M/L/T.</summary>
    public static int? StarColor(string? type)
    {
        if (string.IsNullOrEmpty(type))
            return null;

        return type[0] switch
        {
            'O' or 'B' or 'A' => 0,
            'F' => 1,
            'G' or 'K' => 2,
            'M' or 'L' or 'T' => 3,
            _ => null,
        };
    }

    public static LuminosityClass LuminosityClassOf(string? type)
    {
        if (type is null) return LuminosityClass.Nodata;
        if (type.EndsWith('0')) return LuminosityClass.very_luminous_supergiant;
        if (type.EndsWith("1a")) return LuminosityClass.luminous_supergiant;
        if (type.EndsWith("Ib")) return LuminosityClass.less_luminous_supergiant;
        if (type.EndsWith("II")) return LuminosityClass.luminous_giant;
        if (type.EndsWith("III")) return LuminosityClass.giant;
        if (type.EndsWith("IV")) return LuminosityClass.subgiant;
        if (type.EndsWith('V')) return LuminosityClass.dwarf;
        if (type.EndsWith("VI")) return LuminosityClass.subdwarf;
        if (type.EndsWith("VII")) return LuminosityClass.white_dwarf;
        if (type == "sdB+M") return LuminosityClass.pulsar;
        return LuminosityClass.Nodata;
    }

    /// <summary>
    /// Texture name used by the clients. Mirrors the legacy rules, including that a missing
    /// atmosphere class falls through to the zone-based branch.
    /// </summary>
    public static string PlanetTexture(Planet p)
    {
        string? mass = p.MassClass;
        if (mass == "Neptunian")
            return "neptunian";

        if (p.AtmosphereClass == "no-atmosphere")
            return mass == "Jovian" ? "jovian" : "iron";

        bool rocky = mass is "Terran" or "Subterran";
        if (p.ZoneClass == "Hot")
        {
            if (mass == "Superterran") return "hotsuperearth";
            if (mass == "Jovian") return "hotjupiter";
            if (rocky) return "hotstone";
        }

        if (p.ZoneClass == "Cold")
        {
            if (mass == "Superterran") return "coldsuperearth";
            if (mass == "Jovian") return "jovian";
            if (rocky) return "coldstone";
        }
        else
        {
            if (mass == "Superterran") return "superearth";
            if (mass == "Jovian") return "jovian";
            if (rocky) return "stone";
        }

        return "noimg";
    }

    /// <summary>
    /// Scales an orbital distance (AU) into the legacy system view's pixel space: the
    /// outermost planet or the habitable zone's outer edge maps to 600 px.
    /// </summary>
    public static double SystemViewDistance(IReadOnlyList<ExoPlanetsDto> planets, double? distance)
    {
        var star = planets[0].Star!;
        var outermost = planets.OrderBy(p => p.MeanDistance).Last();
        decimal? scaleTo = star.HabZoneMax is null || outermost.MeanDistance > star.HabZoneMax
            ? D(outermost.MeanDistance)
            : D(star.HabZoneMax);
        if (scaleTo == 0)
            scaleTo = null;

        decimal? scaled = 600 * D(distance) / scaleTo + 2 * (decimal)star.Radius;
        return (double?)scaled ?? 300;
    }

    private static double? Celsius(double? kelvin) => kelvin is null ? null : (double)(D(kelvin)!.Value - ZeroCelsius);

    private static double? Clamp(decimal? value, decimal min, decimal max) =>
        value is null ? null : (double)Math.Min(max, Math.Max(min, value.Value));

    // The legacy presentation records were cached in LiteDB, which stores empty strings as null.
    private static string? EmptyToNull(string? value) => string.IsNullOrEmpty(value) ? null : value;

    private static decimal? D(double? value) => value is null ? null : (decimal)value.Value;
}
