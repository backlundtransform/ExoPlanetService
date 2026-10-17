namespace ExoplanetHunter.Domain.Catalog;

/// <summary>
/// A host star. Units follow the catalogue: solar mass/radius/luminosity, Kelvin,
/// distance in parsecs, right ascension in hours, declination in degrees.
/// </summary>
public sealed record Star
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public string? NameHD { get; init; }
    public string? NameHIP { get; init; }

    /// <summary>IAU three-letter constellation abbreviation, e.g. "Lyr".</summary>
    public string? Constellation { get; init; }

    /// <summary>Spectral type, e.g. "G2V" or just "K".</summary>
    public string? Type { get; init; }

    public double? Mass { get; init; }
    public double? Radius { get; init; }
    public double? Teff { get; init; }
    public double? Luminosity { get; init; }
    public double? FeH { get; init; }
    public double? Age { get; init; }
    public double? ApparMag { get; init; }
    public double? Distance { get; init; }
    public double? Ra { get; init; }
    public double? Dec { get; init; }
    public double? MagfromPlanet { get; init; }
    public double? SizefromPlanet { get; init; }
    public double? HabZoneMin { get; init; }
    public double? HabZoneMax { get; init; }
    public bool? HabCat { get; init; }
    public int? NoPlanetsHZ { get; init; }
}
