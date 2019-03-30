using ExoPlanetHunter.Database.entity;
using System;
using System.Linq.Expressions;

namespace ExoPlanetHunter.Service.Dto
{
    public class PlanetDistanceDto
    {
        public string Title { get; set; }
        public decimal? Distance { get; set; }
        public decimal? Angle { get; set; }

        public static Expression<Func<Planet, PlanetDistanceDto>> FromEntities
        {
            get
            {
                return p => new PlanetDistanceDto
                {
                    Title = p.Name,
                    Distance =p.Star.Distance,
                    Angle= -15 * (p.Star.Ra - 12)
                };
            }
        }
    }
}