using ExoplanetHunter.Ingest.Classification;

namespace ExoplanetHunter.Ingest.Tests;

[TestClass]
public sealed class PlanetClassifierTests
{
    [TestMethod]
    [DataRow(1.49, 2.83, "Terran", DisplayName = "Radius decides when both are known")]
    [DataRow(3.13, 9.96, "Neptunian", DisplayName = "Radius 3.13 is Neptunian despite mass < 10")]
    [DataRow(0.3, null, "Mercurian")]
    [DataRow(12.0, null, "Jovian")]
    public void MassClass_PrefersRadius(double? radius, double? mass, string expected)
    {
        Assert.AreEqual(expected, PlanetClassifier.MassClass(mass, radius));
    }

    [TestMethod]
    [DataRow(5.0, "Superterran")]
    [DataRow(60.0, "Jovian")]
    public void MassClass_FallsBackToMass(double mass, string expected)
    {
        Assert.AreEqual(expected, PlanetClassifier.MassClass(mass, null));
    }

    [TestMethod]
    public void MassClass_NothingKnown_IsNull()
    {
        Assert.IsNull(PlanetClassifier.MassClass(null, null));
    }

    [TestMethod]
    public void ZoneClass_UsesHabitableZoneFlux()
    {
        Assert.AreEqual("Warm", PlanetClassifier.ZoneClass(1.0, 5772, null));
        Assert.AreEqual("Hot", PlanetClassifier.ZoneClass(10.0, 5772, null));
        Assert.AreEqual("Cold", PlanetClassifier.ZoneClass(0.1, 5772, null));
    }

    [TestMethod]
    public void ZoneClass_WithoutStellarTemperature_FallsBackToEquilibriumTemperature()
    {
        Assert.AreEqual("Warm", PlanetClassifier.ZoneClass(1.0, null, 290));
        Assert.AreEqual("Hot", PlanetClassifier.ZoneClass(null, null, 400));
        Assert.AreEqual("Cold", PlanetClassifier.ZoneClass(null, null, 200));
        Assert.IsNull(PlanetClassifier.ZoneClass(null, null, null));
    }

    [TestMethod]
    public void IsHabitable_EarthAnalog()
    {
        Assert.IsTrue(PlanetClassifier.IsHabitable(1.0, 5772, 1.0, 1.0));
    }

    [TestMethod]
    public void IsHabitable_TooLarge_IsFalse()
    {
        Assert.IsFalse(PlanetClassifier.IsHabitable(1.0, 5772, 3.0, null));
    }

    [TestMethod]
    public void IsHabitable_NoRadius_UsesMassLimit()
    {
        Assert.IsTrue(PlanetClassifier.IsHabitable(1.0, 5772, null, 8.0));
        Assert.IsFalse(PlanetClassifier.IsHabitable(1.0, 5772, null, 12.0));
    }

    [TestMethod]
    public void IsHabitable_OutsideZone_IsFalse()
    {
        Assert.IsFalse(PlanetClassifier.IsHabitable(2.5, 5772, 1.0, null));
        Assert.IsFalse(PlanetClassifier.IsHabitable(0.2, 5772, 1.0, null));
    }

    [TestMethod]
    public void HabitableZoneFluxes_ClampsTemperatureToFitRange()
    {
        // A white dwarf or hot star outside 2600–7200 K uses the nearest valid limit.
        Assert.AreEqual(PlanetClassifier.HabitableZoneFluxes(7200), PlanetClassifier.HabitableZoneFluxes(12000));
        Assert.AreEqual(PlanetClassifier.HabitableZoneFluxes(2600), PlanetClassifier.HabitableZoneFluxes(2000));
    }

    [TestMethod]
    public void HabitableZone_Kepler1556_MatchesLegacyCatalogue()
    {
        var zone = PlanetClassifier.HabitableZone(0.7997751, 5608);

        Assert.IsNotNull(zone);
        Assert.AreEqual(0.67795352, zone.Value.Min, 1e-6);
        Assert.AreEqual(1.6048713, zone.Value.Max, 1e-6);
    }

    [TestMethod]
    public void SpectralClass_FromTemperature()
    {
        Assert.AreEqual("G", PlanetClassifier.SpectralClass(5772));
        Assert.AreEqual("M", PlanetClassifier.SpectralClass(3000));
        Assert.IsNull(PlanetClassifier.SpectralClass(null));
    }
}
