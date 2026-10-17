namespace ExoplanetHunter.Api.Legacy;

// Enums from the legacy service. The public API exposes their ordinals, so member
// order must never change.

public enum ConstellationCode
{
    And, Ant, Aps, Aqr, Aql, Ara, Ari, Aur, Boo, Cae, Cam, Cnc, CVn, CMa, CMi, Cap, Car, Cas, Cen, Cep, Cet, Cha, Cir, Col, Com, CrA, CrB, Crv, Crt, Cru,
    Cyg, Del, Dor, Dra, Equ, Eri, For, Gem, Gru, Her, Hor, Hya, Hyi, Ind, Lac, Leo, LMi, Lep, Lib, Lup, Lyn, Lyr, Men, Mic, Mon, Mus, Nor, Oct, Oph, Ori,
    Pav, Peg, Per, Phe, Pic, Psc, PsA, Pup, Pyx, Ret, Sge, Sgr, Sco, Scl, Sct, Ser, Sex, Tau, Tel, Tri, TrA, Tuc, UMa, UMi, Vel, Vir, Vol, Vul, Nodata
}

public enum ChartType { Mass, DiscoveryMetod, Temperature, DiscoveryYear }

public enum CompositionType { gas, water_gas, rocky_water, rocky_iron, iron, Nodata }

public enum DiscoveryMethodType { RadialVelocity, Transit, Microlensing, Astrometry, PulsarTiming, Imaging, PrimaryTransit, Nodata }

public enum HabitableType { mesoplanet, thermoplanet, psychroplanet, hypopsychroplanet, hyperthermoplanet, non_habitable, Nodata }

public enum LuminosityClass
{
    very_luminous_supergiant, luminous_supergiant, less_luminous_supergiant, luminous_giant, giant, subgiant, dwarf, subdwarf, white_dwarf, pulsar, Nodata
}

public enum MagnitudeType { visible, unvisible }

public enum MassType { Jovian, Neptunian, Superterran, Terran, Subterran, Mercurian, Nodata }

public enum TemperatureZone { Hot, Warm, Cold, Nodata }

public static class LegacyEnum
{
    /// <summary>
    /// Parses catalogue text the way the legacy service did: "-" becomes "_", spaces are
    /// removed, matching is case-sensitive, and anything unknown maps to Nodata.
    /// </summary>
    public static T Parse<T>(string? value) where T : struct, Enum
    {
        if (value is not null
            && Enum.TryParse<T>(value.Replace("-", "_").Replace(" ", string.Empty), ignoreCase: false, out var result)
            && Enum.IsDefined(result))
        {
            return result;
        }

        return Enum.Parse<T>("Nodata");
    }
}
