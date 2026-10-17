using CSharpNumerics.Physics.Astro;
using CSharpNumerics.Physics.Astro.Enums;

namespace ExoplanetHunter.Ingest.Classification;

/// <summary>
/// Derives the catalogue's classification fields from measured parameters, following
/// docs/field-mapping.md (the methodology of the PHL Habitable Worlds Catalog).
/// All quantities are in catalogue units: Earth mass/radius, flux relative to Earth, K, AU.
/// </summary>
public static class PlanetClassifier
{
    /// <summary>Bond albedo assumed for equilibrium temperatures (Earth-like, as PHL).</summary>
    public const double BondAlbedo = 0.3;

    /// <summary>The Kopparapu et al. (2014) fit is valid for stars between these temperatures.</summary>
    public const double MinHabitableZoneTeff = 2600, MaxHabitableZoneTeff = 7200;

    /// <summary>Planet size range considered potentially rocky (Earth radii).</summary>
    public const double MinHabitableRadius = 0.5, MaxHabitableRadius = 2.5;

    /// <summary>Mass limit used when a planet has no radius (Earth masses).</summary>
    public const double MaxHabitableMassWithoutRadius = 10;

    /// <summary>
    /// Mass class from radius, or from mass when radius is unknown. PHL classifies by radius
    /// first: 97% of the classes in the legacy catalogue follow the radius thresholds.
    /// </summary>
    public static string? MassClass(double? massEarth, double? radiusEarth)
    {
        if (radiusEarth is { } r)
        {
            return r switch
            {
                <= 0.4 => "Mercurian",
                <= 0.8 => "Subterran",
                <= 1.6 => "Terran",
                <= 2.5 => "Superterran",
                <= 7.0 => "Neptunian",
                _ => "Jovian",
            };
        }

        if (massEarth is { } m)
        {
            return m switch
            {
                <= 0.1 => "Mercurian",
                <= 0.5 => "Subterran",
                <= 2 => "Terran",
                <= 10 => "Superterran",
                <= 50 => "Neptunian",
                _ => "Jovian",
            };
        }

        return null;
    }

    /// <summary>
    /// "Hot" inside the optimistic habitable zone, "Warm" within it, "Cold" beyond it.
    /// Without a usable stellar temperature, falls back to the equilibrium temperature
    /// (warm between 0 °C and 50 °C, as the legacy classifier did).
    /// </summary>
    public static string? ZoneClass(double? fluxEarth, double? stellarTeff, double? equilibriumTemperature)
    {
        if (fluxEarth is { } flux && HabitableZoneFluxes(stellarTeff) is { } zone)
        {
            if (flux > zone.Inner) return "Hot";
            if (flux < zone.Outer) return "Cold";
            return "Warm";
        }

        return equilibriumTemperature switch
        {
            null => null,
            > 323.15 => "Hot",
            < 273.15 => "Cold",
            _ => "Warm",
        };
    }

    /// <summary>
    /// Potentially habitable: receives a flux inside the optimistic habitable zone and is
    /// small enough to be rocky.
    /// </summary>
    public static bool IsHabitable(double? fluxEarth, double? stellarTeff, double? radiusEarth, double? massEarth)
    {
        if (fluxEarth is not { } flux || HabitableZoneFluxes(stellarTeff) is not { } zone)
            return false;

        bool rockySize = radiusEarth is { } r
            ? r is >= MinHabitableRadius and <= MaxHabitableRadius
            : massEarth is <= MaxHabitableMassWithoutRadius;

        return rockySize && flux <= zone.Inner && flux >= zone.Outer;
    }

    /// <summary>Optimistic habitable zone limits as stellar flux (Recent Venus, Early Mars).</summary>
    public static (double Inner, double Outer)? HabitableZoneFluxes(double? stellarTeff)
    {
        if (stellarTeff is not { } teff || teff <= 0)
            return null;

        teff = Math.Clamp(teff, MinHabitableZoneTeff, MaxHabitableZoneTeff);
        return (AstronomyExtensions.HabitableZoneFlux(teff, HabitableZoneLimit.RecentVenus),
                AstronomyExtensions.HabitableZoneFlux(teff, HabitableZoneLimit.EarlyMars));
    }

    /// <summary>Optimistic habitable zone in AU for a star of the given luminosity and temperature.</summary>
    public static (double Min, double Max)? HabitableZone(double? luminositySolar, double? stellarTeff)
    {
        if (luminositySolar is not { } lum || stellarTeff is not { } teff || teff <= 0)
            return null;

        teff = Math.Clamp(teff, MinHabitableZoneTeff, MaxHabitableZoneTeff);
        var (inner, outer) = AstronomyExtensions.CalculateGoldilocksZone(lum, teff, HabitableZoneModel.Optimistic);
        return (inner, outer);
    }

    /// <summary>Spectral class letter from effective temperature, for stars without a published type.</summary>
    public static string? SpectralClass(double? teff)
    {
        if (teff is not { } t)
            return null;

        var type = AstronomyExtensions.GetSpectralFromTemp(t);
        return type == SpectralType.Unknown ? null : type.ToString();
    }
}
