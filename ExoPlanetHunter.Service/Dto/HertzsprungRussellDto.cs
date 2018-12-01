using ExoPlanetHunter.Database.entity;
using System;
using System.Linq.Expressions;

namespace ExoPlanetHunter.Service.Dto
{
    public class HertzsprungRussellDto
    {
        public string Title { get; set; }
        public string Type { get; set; }
        public decimal? Lum { get; set; }
        public decimal? Mag { get; set; }
        public string Color { get; set; }
        public decimal? Size { get; set; }
        public string Constellation { get; set; }

        public static Expression<Func<Star, HertzsprungRussellDto>> FromEntities
        {
            get
            {
                return p => new HertzsprungRussellDto
                {
                    Title = p.Name,
                    Type = p.Type,
                    Lum = p.Luminosity,
                    Mag = p.ApparMag,
                    Color = "#f49842",
                    Size = p.Mass,
                    Constellation = p.Constellation.Name
                };
            }
        }

        public static HertzsprungRussellDto FromEntity(Star star) =>
            FromEntities.Compile().Invoke(star);
    }
}
