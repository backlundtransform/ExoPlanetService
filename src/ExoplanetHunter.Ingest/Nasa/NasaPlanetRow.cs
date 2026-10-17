namespace ExoplanetHunter.Ingest.Nasa;

/// <summary>
/// One row of the NASA Exoplanet Archive "Planetary Systems Composite Parameters" table
/// (pscomppars). Units as published: RA/Dec in degrees, st_lum as log10(L/L☉),
/// distance in parsecs, planet mass and radius in Earth units.
/// </summary>
public sealed record NasaPlanetRow
{
    public required string PlanetName { get; init; }
    public required string HostName { get; init; }
    public string? HdName { get; init; }
    public string? HipName { get; init; }

    public double? PlanetRadius { get; init; }
    public double? PlanetMass { get; init; }
    public double? Insolation { get; init; }
    public double? SemiMajorAxis { get; init; }
    public double? Eccentricity { get; init; }
    public double? Period { get; init; }
    public double? Inclination { get; init; }
    public double? ArgumentOfPeriastron { get; init; }

    public string? SpectralType { get; init; }
    public double? StellarTeff { get; init; }
    public double? StellarRadius { get; init; }
    public double? StellarMass { get; init; }
    public double? StellarLogLuminosity { get; init; }
    public double? StellarMetallicity { get; init; }
    public double? StellarAge { get; init; }

    public double? Distance { get; init; }
    public double? VMagnitude { get; init; }
    public double? RaDegrees { get; init; }
    public double? DecDegrees { get; init; }

    public string? DiscoveryMethod { get; init; }
    public int? DiscoveryYear { get; init; }
}
