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
                    Distance= (decimal)3.26156 * (p.Star.Distance ?? 0),
                    Angle=p.Star.Ra==null?0: p.Star.Ra 
                };
            }
        }
    }
}