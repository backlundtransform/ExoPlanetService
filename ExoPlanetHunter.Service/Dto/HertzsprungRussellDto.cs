using ExoPlanetHunter.Database.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ExoPlanetHunter.Service.Dto
{
    public class HertzsprungRussellDto
    {
        public string Title { get; set; }
        public string Type { get; set; }
        public decimal? Lum { get; set; }
        public string Color { get; set; }
        public decimal? Size { get; set; }
        public decimal? Temp { get; set; }
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
                    Color = dict.ContainsKey(p.Type.FirstOrDefault()) ? dict[p.Type[0]] : null,
                    Size = p.Mass * 100,
                    Constellation = p.Constellation.Name,
                    Temp = p.Teff
                };
            }
        }

        private static Dictionary<char, string> dict = new Dictionary<char, string>() {
            { 'O', "#19147A" },
            { 'B', "#1D6AAE" },
            { 'A', "#A3D4E2" },
            { 'F', "#ECF2EE" },
            { 'G', "#FCDF21" },
            { 'K', "#FB830B" },
            { 'M', "#FB5908" },
            { 'L', "#FB110A" },
            { 'T', "#9b0000" },
        };

        public static HertzsprungRussellDto FromEntity(Star star) =>
            FromEntities.Compile().Invoke(star);
    }
}