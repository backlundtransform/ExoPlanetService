using ExoplanetHunter.Domain.Catalog;
using ExoplanetHunter.Ingest.Nasa;

namespace ExoplanetHunter.Ingest.Tests;

[TestClass]
public sealed class CatalogBuilderTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 15, 0, 0, 0, TimeSpan.Zero);

    private static CatalogSnapshot Previous(int? nextStarId = null, int? nextPlanetId = null) => new()
    {
        Source = "test",
        GeneratedAt = Now.AddDays(-7),
        Constellations = [new Constellation { Id = 1, Name = "Lyr" }],
        Stars =
        [
            new Star { Id = 1, Name = "Sun", Type = "G2V" },
            new Star { Id = 7, Name = "Kepler-1556", Constellation = "Lyr" },
        ],
        Planets =
        [
            new Planet { Id = 1, StarId = 1, Name = "Tellus", Habitable = true },
            new Planet { Id = 20, StarId = 7, Name = "Kepler-1556 b" },
        ],
        NextStarId = nextStarId,
        NextPlanetId = nextPlanetId,
    };

    private static NasaPlanetRow Row(string planet, string host, double ra = 288.397426, double dec = 40.2132) => new()
    {
        PlanetName = planet,
        HostName = host,
        RaDegrees = ra,
        DecDegrees = dec,
        PlanetRadius = 1.5,
        PlanetMass = 3.0,
        SemiMajorAxis = 1.0,
        Eccentricity = 0.0,
        Period = 365,
        StellarTeff = 5772,
        StellarRadius = 1.0,
        StellarMass = 1.0,
        StellarLogLuminosity = 0.0,
        Distance = 10,
        DiscoveryMethod = "Radial Velocity",
        DiscoveryYear = 2020,
    };

    [TestMethod]
    public void Build_ExistingNames_KeepTheirIds()
    {
        var snapshot = CatalogBuilder.Build([Row("Kepler-1556 b", "Kepler-1556")], Previous(), Now);

        Assert.AreEqual(7, snapshot.Stars.Single(s => s.Name == "Kepler-1556").Id);
        Assert.AreEqual(20, snapshot.Planets.Single(p => p.Name == "Kepler-1556 b").Id);
    }

    [TestMethod]
    public void Build_NewNames_GetIdsAboveThePrevious()
    {
        var snapshot = CatalogBuilder.Build([Row("New b", "New"), Row("New c", "New")], Previous(), Now);

        Assert.AreEqual(8, snapshot.Stars.Single(s => s.Name == "New").Id);
        CollectionAssert.AreEqual(new[] { 21, 22 }, snapshot.Planets.Where(p => p.Name.StartsWith("New")).Select(p => p.Id).ToArray());
        Assert.AreEqual(9, snapshot.NextStarId);
        Assert.AreEqual(23, snapshot.NextPlanetId);
    }

    [TestMethod]
    public void Build_RemovedIds_AreNotReused()
    {
        // Ids 100+ were handed out before and later removed from the catalogue.
        var snapshot = CatalogBuilder.Build([Row("New b", "New")], Previous(nextStarId: 100, nextPlanetId: 200), Now);

        Assert.AreEqual(100, snapshot.Stars.Single(s => s.Name == "New").Id);
        Assert.AreEqual(200, snapshot.Planets.Single(p => p.Name == "New b").Id);
    }

    [TestMethod]
    public void Build_CarriesOverTheSolarSystem()
    {
        var snapshot = CatalogBuilder.Build([Row("Kepler-1556 b", "Kepler-1556")], Previous(), Now);

        var sun = snapshot.Stars.Single(s => s.Name == "Sun");
        Assert.AreEqual(1, sun.Id);
        Assert.AreEqual("Tellus", snapshot.Planets.Single(p => p.StarId == sun.Id).Name);
    }

    [TestMethod]
    public void Build_StarFields()
    {
        var star = CatalogBuilder.Build([Row("Kepler-1556 b", "Kepler-1556")], Previous(), Now).Stars.Single(s => s.Id == 7);

        Assert.AreEqual(288.397426 / 15, star.Ra!.Value, 1e-9);   // hours, as in the API contract
        Assert.AreEqual("Lyr", star.Constellation);
        Assert.AreEqual(1.0, star.Luminosity!.Value, 1e-9);      // Stefan–Boltzmann, R = 1, T = 5772 K
        Assert.AreEqual("G", star.Type);                        // from temperature, no published type
        Assert.AreEqual(1, star.NoPlanetsHZ);
    }

    [TestMethod]
    public void Build_PlanetFields()
    {
        var planet = CatalogBuilder.Build([Row("Kepler-1556 b", "Kepler-1556")], Previous(), Now).Planets.Single(p => p.Id == 20);

        Assert.AreEqual(3.0 / Math.Pow(1.5, 3), planet.Density!.Value, 1e-12);
        Assert.AreEqual(3.0 / (1.5 * 1.5), planet.Gravity!.Value, 1e-12);
        Assert.AreEqual(Math.Sqrt(3.0 / 1.5), planet.EscVel!.Value, 1e-12);
        Assert.AreEqual(1.0, planet.SFluxMean!.Value, 1e-12);
        Assert.AreEqual(254.6, planet.TeqMean!.Value, 0.1);
        Assert.AreEqual("Terran", planet.MassClass);
        Assert.AreEqual("Warm", planet.ZoneClass);
        Assert.IsTrue(planet.Habitable);
        Assert.AreEqual("RadialVelocity", planet.DiscoveryMethod);
    }

    [TestMethod]
    public void Build_MissingSemiMajorAxis_UsesKeplersThirdLaw()
    {
        var row = Row("Kepler-1556 b", "Kepler-1556") with { SemiMajorAxis = null, Period = 365.25 };

        var planet = CatalogBuilder.Build([row], Previous(), Now).Planets.Single(p => p.Id == 20);

        Assert.AreEqual(1.0, planet.MeanDistance!.Value, 0.01);
        Assert.IsNull(planet.SemMajorAxis);
    }

    [TestMethod]
    public void Build_NewConstellation_IsAdded()
    {
        var snapshot = CatalogBuilder.Build([Row("T b", "TRAPPIST-1", ra: 346.6223687, dec: -5.0413976)], Previous(), Now);

        Assert.AreEqual(2, snapshot.Constellations.Single(c => c.Name == "Aqr").Id);
    }
}
