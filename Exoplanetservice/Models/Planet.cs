using System.ComponentModel.DataAnnotations;

namespace ExoplanetService.Models
{
    public class Planet
    {
        [Key]
        public int Id { get; set; }

        public virtual Star Star { get; set; }
        public string Name { get; set; }
        public string NameKepler { get; set; }
        public string NameKOI { get; set; }
        public string ZoneClass { get; set; }
        public string MassClass { get; set; }
        public string CompositionClass { get; set; }
        public string AtmosphereClass { get; set; }
        public string HabitableClass { get; set; }
        public decimal? MinMass { get; set; }
        public decimal? Mass { get; set; }
        public decimal? MaxMass { get; set; }
        public decimal? Radius { get; set; }
        public decimal? Density { get; set; }
        public decimal? Gravity { get; set; }
        public decimal? EscVel { get; set; }

        public decimal? SFluxMin { get; set; }
        public decimal? SFluxMean { get; set; }
        public decimal? SFluxMax { get; set; }
        public decimal? TeqMin { get; set; }
        public decimal? TeqMean { get; set; }
        public decimal? TeqMax { get; set; }
        public decimal? TsMin { get; set; }
        public decimal? TsMean { get; set; }

        public decimal? TsMax { get; set; }
        public decimal? SurfPress { get; set; }
        public decimal? Mag { get; set; }
        public decimal? ApparSize { get; set; }
        public decimal? Period { get; set; }
        public decimal? SemMajorAxis { get; set; }

        public decimal? Eccentricity { get; set; }

        public decimal? MeanDistance { get; set; }

        public decimal? Inclination { get; set; }
        public decimal? Omega { get; set; }
        public decimal? Hzd { get; set; }
        public decimal? Hzc { get; set; }
        public decimal? Hza { get; set; }
        public decimal? Hzi { get; set; }

        public decimal? Sph { get; set; }

        public decimal? IntEsi { get; set; }
        public decimal? SurfEsi { get; set; }
        public decimal? Esi { get; set; }
        public bool? Habitable { get; set; }
        public bool? HabMoon { get; set; }

        public bool? Confirmed { get; set; }

        public string Disc_Method { get; set; }
        public int? Disc_Year { get; set; }
    }
}