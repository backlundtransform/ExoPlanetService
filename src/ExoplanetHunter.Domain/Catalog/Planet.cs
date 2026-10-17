using System.Text.Json.Serialization;

namespace ExoplanetHunter.Domain.Catalog;

/// <summary>
/// A planet. Units follow the catalogue: Earth mass/radius/density/gravity/escape velocity,
/// stellar flux relative to Earth, temperatures in Kelvin, period in days, distances in AU.
/// </summary>
public sealed record Planet
{
    public required int Id { get; init; }
    public required int StarId { get; init; }
    public required string Name { get; init; }
    public string? NameKepler { get; init; }
    public string? NameKOI { get; init; }

    /// <summary>Thermal zone: "Hot", "Warm" or "Cold".</summary>
    public string? ZoneClass { get; init; }

    /// <summary>Mass class: "Mercurian", "Subterran", "Terran", "Superterran", "Neptunian" or "Jovian".</summary>
    public string? MassClass { get; init; }

    public string? CompositionClass { get; init; }
    public string? AtmosphereClass { get; init; }
    public string? HabitableClass { get; init; }
    public double? MinMass { get; init; }
    public double? Mass { get; init; }
    public double? MaxMass { get; init; }
    public double? Radius { get; init; }
    public double? Density { get; init; }
    public double? Gravity { get; init; }
    public double? EscVel { get; init; }
    public double? SFluxMin { get; init; }
    public double? SFluxMean { get; init; }
    public double? SFluxMax { get; init; }
    public double? TeqMin { get; init; }
    public double? TeqMean { get; init; }
    public double? TeqMax { get; init; }
    public double? TsMin { get; init; }
    public double? TsMean { get; init; }
    public double? TsMax { get; init; }
    public double? SurfPress { get; init; }
    public double? Mag { get; init; }
    public double? ApparSize { get; init; }
    public double? Period { get; init; }
    public double? SemMajorAxis { get; init; }
    public double? Eccentricity { get; init; }
    public double? MeanDistance { get; init; }
    public double? Inclination { get; init; }
    public double? Omega { get; init; }
    public double? Hzd { get; init; }
    public double? Hzc { get; init; }
    public double? Hza { get; init; }
    public double? Hzi { get; init; }
    public double? Sph { get; init; }
    public double? IntEsi { get; init; }
    public double? SurfEsi { get; init; }
    public double? Esi { get; init; }
    public bool? Habitable { get; init; }
    public bool? HabMoon { get; init; }
    public bool? Confirmed { get; init; }

    /// <summary>Discovery method without spaces, e.g. "Transit" or "RadialVelocity".</summary>
    [JsonPropertyName("disc_Method")]
    public string? DiscoveryMethod { get; init; }

    [JsonPropertyName("disc_Year")]
    public int? DiscoveryYear { get; init; }
}
