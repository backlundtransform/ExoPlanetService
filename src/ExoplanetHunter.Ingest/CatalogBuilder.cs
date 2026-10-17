using CSharpNumerics.Physics.Astro;
using ExoplanetHunter.Domain.Catalog;
using ExoplanetHunter.Ingest.Classification;
using ExoplanetHunter.Ingest.Nasa;

namespace ExoplanetHunter.Ingest;

/// <summary>
/// Builds a catalogue snapshot from NASA Exoplanet Archive rows. Stars and planets keep the
/// ids they had in the previous snapshot (matched by name), new ones get fresh ids, and the
/// hand-curated Solar System is carried over unchanged. Field rules: docs/field-mapping.md.
/// </summary>
public static class CatalogBuilder
{
    public const string SolarSystemStar = "Sun";

    public static CatalogSnapshot Build(IReadOnlyList<NasaPlanetRow> rows, CatalogSnapshot previous, DateTimeOffset generatedAt)
    {
        var starIds = previous.Stars.ToDictionary(s => s.Name, s => s.Id, StringComparer.Ordinal);
        var planetIds = previous.Planets.ToDictionary(p => p.Name, p => p.Id, StringComparer.Ordinal);
        int nextStarId = Math.Max(previous.NextStarId ?? 0, previous.Stars.Select(s => s.Id).DefaultIfEmpty().Max() + 1);
        int nextPlanetId = Math.Max(previous.NextPlanetId ?? 0, previous.Planets.Select(p => p.Id).DefaultIfEmpty().Max() + 1);

        var stars = new List<Star>();
        var planets = new List<Planet>();

        var sun = previous.Stars.SingleOrDefault(s => s.Name == SolarSystemStar);
        if (sun is not null)
        {
            stars.Add(sun);
            planets.AddRange(previous.Planets.Where(p => p.StarId == sun.Id));
        }

        // Ordinal order keeps id assignment for new objects deterministic.
        foreach (var system in rows.GroupBy(r => r.HostName).OrderBy(g => g.Key, StringComparer.Ordinal))
        {
            if (system.Key == SolarSystemStar)
                continue;

            int starId = starIds.TryGetValue(system.Key, out var existingStar) ? existingStar : nextStarId++;
            var host = system.First();
            var systemPlanets = system
                .OrderBy(r => r.PlanetName, StringComparer.Ordinal)
                .Select(r => BuildPlanet(r, planetIds.TryGetValue(r.PlanetName, out var id) ? id : nextPlanetId++, starId))
                .ToArray();

            planets.AddRange(systemPlanets);
            stars.Add(BuildStar(host, starId, systemPlanets.Count(p => p.Habitable == true)));
        }

        var constellations = previous.Constellations.ToList();
        int nextConstellationId = constellations.Select(c => c.Id).DefaultIfEmpty().Max() + 1;
        foreach (var name in stars.Select(s => s.Constellation).OfType<string>().Distinct().Except(constellations.Select(c => c.Name)).Order())
            constellations.Add(new Constellation { Id = nextConstellationId++, Name = name });

        return new CatalogSnapshot
        {
            Source = "nasa-exoplanet-archive/pscomppars",
            GeneratedAt = generatedAt,
            Constellations = constellations,
            Stars = stars.OrderBy(s => s.Id).ToArray(),
            Planets = planets.OrderBy(p => p.Id).ToArray(),
            NextStarId = nextStarId,
            NextPlanetId = nextPlanetId,
        };
    }

    public static Star BuildStar(NasaPlanetRow host, int id, int habitablePlanets)
    {
        double? luminosity = StellarLuminosity(host);
        var habitableZone = PlanetClassifier.HabitableZone(luminosity, host.StellarTeff);

        return new Star
        {
            Id = id,
            Name = host.HostName,
            NameHD = host.HdName,
            NameHIP = host.HipName,
            Constellation = host.RaDegrees is { } ra && host.DecDegrees is { } dec ? ConstellationLocator.Default.Locate(ra, dec) : null,
            Type = host.SpectralType ?? PlanetClassifier.SpectralClass(host.StellarTeff),
            Mass = host.StellarMass,
            Radius = host.StellarRadius,
            Teff = host.StellarTeff,
            Luminosity = luminosity,
            FeH = host.StellarMetallicity,
            Age = host.StellarAge,
            ApparMag = host.VMagnitude,
            Distance = host.Distance,
            Ra = host.RaDegrees / 15.0,
            Dec = host.DecDegrees,
            HabZoneMin = habitableZone?.Min,
            HabZoneMax = habitableZone?.Max,
            NoPlanetsHZ = habitablePlanets,
        };
    }

    public static Planet BuildPlanet(NasaPlanetRow r, int id, int starId)
    {
        double? mass = r.PlanetMass, radius = r.PlanetRadius;
        double eccentricity = r.Eccentricity is >= 0 and < 1 ? r.Eccentricity.Value : 0;
        double? a = SemiMajorAxis(r);

        double? fluxLuminosity = FluxLuminosity(r);
        double? fluxMean = r.Insolation;
        (double Min, double Max)? fluxRange = fluxMean is { } published ? (published, published) : null;
        if (fluxLuminosity is { } lum && a is { } axis)
        {
            var range = AstronomyExtensions.InsolationFluxRange(lum, axis, eccentricity);
            fluxMean = AstronomyExtensions.InsolationFlux(lum, axis);
            fluxRange = (range.Minimum, range.Maximum);
        }

        double? teqMean = Teq(fluxMean), teqMin = Teq(fluxRange?.Min), teqMax = Teq(fluxRange?.Max);
        bool massAndRadius = mass is > 0 && radius is > 0;

        return new Planet
        {
            Id = id,
            StarId = starId,
            Name = r.PlanetName,
            ZoneClass = PlanetClassifier.ZoneClass(fluxMean, r.StellarTeff, teqMean),
            MassClass = PlanetClassifier.MassClass(mass, radius),
            Mass = mass,
            Radius = radius,
            Density = massAndRadius ? mass!.Value / Math.Pow(radius!.Value, 3) : null,
            Gravity = massAndRadius ? mass!.Value / (radius!.Value * radius.Value) : null,
            EscVel = massAndRadius ? Math.Sqrt(mass!.Value / radius!.Value) : null,
            SFluxMin = fluxRange?.Min,
            SFluxMean = fluxMean,
            SFluxMax = fluxRange?.Max,
            TeqMin = teqMin,
            TeqMean = teqMean,
            TeqMax = teqMax,
            // No surface temperatures are published; like the legacy catalogue, use Teq.
            TsMin = teqMin,
            TsMean = teqMean,
            TsMax = teqMax,
            Period = r.Period,
            SemMajorAxis = r.SemiMajorAxis,
            Eccentricity = r.Eccentricity,
            MeanDistance = a is { } d ? d * (1 + eccentricity * eccentricity / 2) : null,
            Inclination = r.Inclination,
            Omega = r.ArgumentOfPeriastron,
            Esi = radius is > 0 && fluxMean is { } s ? AstronomyExtensions.CalculateEsiFromFlux(radius.Value, s) : null,
            Habitable = PlanetClassifier.IsHabitable(fluxMean, r.StellarTeff, radius, mass),
            DiscoveryMethod = r.DiscoveryMethod?.Replace(" ", string.Empty),
            DiscoveryYear = r.DiscoveryYear,
        };
    }

    /// <summary>Stellar luminosity from radius and temperature (as PHL), else the published value.</summary>
    private static double? StellarLuminosity(NasaPlanetRow r) =>
        r.StellarRadius is > 0 && r.StellarTeff is > 0
            ? AstronomyExtensions.StellarLuminosity(r.StellarRadius.Value, r.StellarTeff.Value)
            : r.StellarLogLuminosity is { } log ? Math.Pow(10, log) : null;

    /// <summary>Luminosity used for insolation: the published value (as PHL), else Stefan–Boltzmann.</summary>
    private static double? FluxLuminosity(NasaPlanetRow r) =>
        r.StellarLogLuminosity is { } log
            ? Math.Pow(10, log)
            : r.StellarRadius is > 0 && r.StellarTeff is > 0
                ? AstronomyExtensions.StellarLuminosity(r.StellarRadius.Value, r.StellarTeff.Value)
                : null;

    /// <summary>Published semi-major axis, else Kepler's third law from period and stellar mass.</summary>
    private static double? SemiMajorAxis(NasaPlanetRow r) =>
        r.SemiMajorAxis is > 0 ? r.SemiMajorAxis
        : r.Period is > 0 && r.StellarMass is > 0 ? KeplerOrbit.SemiMajorAxisAU(r.Period.Value, r.StellarMass.Value)
        : null;

    private static double? Teq(double? flux) =>
        flux is > 0 ? AstronomyExtensions.EquilibriumTemperature(flux.Value, PlanetClassifier.BondAlbedo) : null;
}
