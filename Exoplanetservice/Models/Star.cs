
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ExoplanetService.Models
{
    public class Star
    {

        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string NameHD { get; set; }
        public string NameHIP { get; set; }
        public Constellation Constellation { get; set; }
        public string Type { get; set; }

        public decimal? Mass { get; set; }
     
        public decimal? Radius { get; set; }
        public decimal? Teff { get; set; }
        public decimal? Luminosity { get; set; }
        public decimal? FeH { get; set; }
        public decimal? Age { get; set; }


        public decimal? ApparMag { get; set; }
        public decimal? Distance { get; set; }
        public decimal? Ra { get; set; }
        public decimal? Dec { get; set; }
        public decimal? MagfromPlanet { get; set; }
        public decimal? SizefromPlanet { get; set; }
        public int? NoPlanets { get; set; }
        public int? NoPlanetsHZ { get; set; }


        public decimal? HabZoneMin { get; set; }
        public decimal? HabZoneMax { get; set; }

        public bool? HabCat { get; set; }
     
        public virtual ICollection<Planet> Planets { get; set; }
    }
}
