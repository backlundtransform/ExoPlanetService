using ExoplanetHunter.Ingest.Classification;

namespace ExoplanetHunter.Ingest.Tests;

[TestClass]
public sealed class ConstellationLocatorTests
{
    [TestMethod]
    [DataRow(346.6223687, -5.0413976, "Aqr", DisplayName = "TRAPPIST-1")]
    [DataRow(288.3974260, 40.2132000, "Lyr", DisplayName = "Kepler-1556")]
    [DataRow(217.3934657, -62.6761821, "Cen", DisplayName = "Proxima Centauri")]
    [DataRow(37.9545607, 89.2641094, "UMi", DisplayName = "Polaris")]
    [DataRow(101.2871553, -16.7161159, "CMa", DisplayName = "Sirius")]
    [DataRow(266.4168371, -29.0078106, "Sgr", DisplayName = "Galactic centre")]
    public void Locate_KnownPositions(double ra, double dec, string expected)
    {
        Assert.AreEqual(expected, ConstellationLocator.Default.Locate(ra, dec));
    }

    [TestMethod]
    public void Precess_SameEpoch_IsIdentity()
    {
        var (ra, dec) = ConstellationLocator.Precess(123.4, -45.6, 2000, 2000);

        Assert.AreEqual(123.4, ra, 1e-9);
        Assert.AreEqual(-45.6, dec, 1e-9);
    }

    [TestMethod]
    public void Precess_J2000ToB1875_MovesRightAscensionBack()
    {
        // General precession is ~50.3"/yr in longitude; over 125 years an equatorial
        // point near RA 0h moves by roughly 1.6° in right ascension.
        var (ra, _) = ConstellationLocator.Precess(10.0, 0.0, 2000, 1875);

        Assert.AreEqual(10.0 - 1.6, ra, 0.1);
    }
}
