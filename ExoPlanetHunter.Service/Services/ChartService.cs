using ExoPlanetHunter.Database;
using ExoPlanetHunter.Database.entity;
using ExoPlanetHunter.Service.Dto;
using ExoPlanetHunter.Service.Enum;
using ExoPlanetHunter.Service.Interfaces;
using LiteDB;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExoPlanetHunter.Service.Services
{
    public class ChartService : IChartService
    {
        private readonly ExoContext _context;

        private IHostingEnvironment _env;

        public ChartService(ExoContext context, IHostingEnvironment env)
        {
            _context = context;

            _env = env;
        }

        public List<EsiDistanceDto> GetEsiDistance()
        {
            using (var db = new LiteDatabase($@"{_env.ContentRootPath}\nosqlexo.db"))
            {
                var col = db.GetCollection<ExoPlanetsDto>("exoplanet");


              var cols= col.Find(p => p.Distance != null && p.Esi != null &&p.Hab==true).Select(p => new EsiDistanceDto()
                {
                    StarName = p.Star.Name,
                    PlanetName = p.Name,
                    Esi= (double)p.Esi,
                    Distance = (double)p.Distance
                }).ToList();
                var groupedstars = cols.GroupBy(p => p.StarName).Select(p => p.Key).ToList();
                var colors = GetColors(cols.GroupBy(p => p.StarName).Select(p => p.Key).ToList());

                foreach (var item in cols)
                {
                    item.Color = colors[groupedstars.IndexOf(item.StarName)];
                }

                return cols;

            }
        }

        public IQueryable<HertzsprungRussellDto> GetHertzsprungRussell(bool habitableOnly)
        {
            return _context.Stars.AsQueryable()
                .Where(p => p.Mass != null && p.Luminosity != null
            && p.Teff != null && p.Type != null && habitableOnly?p.Planets.Any(o=>o.Habitable==true):true)
            .Select(HertzsprungRussellDto.FromEntities).Where(p => p.Color != null);
        }

        public IQueryable<PlanetDistanceDto> GetPlanetDistance(double? max=null)
        {
            if (max != null)
            {
                return _context.Planets.AsQueryable().Where(p => p.Habitable == true && 3.26156 * (double)p.Star.Distance < max).Select(PlanetDistanceDto.FromEntities);

            }
            return _context.Planets.AsQueryable().Where(p => p.Habitable == true).Select(PlanetDistanceDto.FromEntities);
        }

        public IQueryable<IGrouping<string, Planet>> GetPlanetTypes(ChartType type)
        {
                switch (type) {
                case ChartType.Mass:  return _context.Planets.AsQueryable().Where(p => p.MassClass != "" && p.MassClass.ToEnum<MassEnum>() != MassEnum.Nodata).GroupBy(p => p.MassClass);
              
                case ChartType.DiscoveryMetod: return _context.Planets.AsQueryable().Where(p => p.Disc_Method != ""&& p.Disc_Method.ToEnum<DiscEnum>() != DiscEnum.Nodata).GroupBy(p => p.Disc_Method);
                case ChartType.DiscoveryYear: return _context.Planets.AsQueryable().Where(p => p.Disc_Year != null).OrderBy(p => p.Disc_Year).GroupBy(p => p.Disc_Year.ToString());
                case ChartType.Temperature: return _context.Planets.AsQueryable().Where(p => p.ZoneClass != "" && p.ZoneClass.ToEnum<TempEnum>() != TempEnum.Nodata).GroupBy(p => p.ZoneClass);
              
                default: return _context.Planets.GroupBy(p => p.Name);
            }
          
        }

        public List<MassOrbitDto> GetMassOrbit()
        {
          
           using (var db = new LiteDatabase($@"{_env.ContentRootPath}\nosqlexo.db"))
            {
                var col = db.GetCollection<ExoPlanetsDto>("exoplanet");


               var cols = col.Find(p => p.Mass != null && p.Period != null).Select(p => new MassOrbitDto()
                {
                    StarName = p.Star.Name,
                    PlanetName = p.Name,
                    Orbit = (double)p.Period,
                    Mass = (double)p.Mass
                }).ToList();
                var groupedstars = cols.GroupBy(p => p.StarName).Select(p => p.Key).ToList();
                var colors = GetColors(cols.GroupBy(p => p.StarName).Select(p => p.Key).ToList());

                foreach (var item in cols)
                {
                  item.Color = colors[groupedstars.IndexOf(item.StarName)];
                }

                return cols;
            }
        }
        private List<string> GetColors(List<string> groupedstars)
        {
            var colors = new List<string>();
            var random = new Random();
            for (int i = 0; i < groupedstars.Count(); i++)
            {
                colors.Add(String.Format("#{0:X6}", random.Next(0x1000000)));
            }

            return colors;
        }
    }
}