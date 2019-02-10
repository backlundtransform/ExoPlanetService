using ExoPlanetHunter.Database;
using ExoPlanetHunter.Database.entity;
using ExoPlanetHunter.Service.Dto;
using ExoPlanetHunter.Service.Enum;
using ExoPlanetHunter.Service.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace ExoPlanetHunter.Service.Services
{
    public class ChartService : IChartService
    {
        private readonly ExoContext _context;

        public ChartService(ExoContext context)
        {
            _context = context;
        }

        public IQueryable<HertzsprungRussellDto> GetHertzsprungRussell(bool habitableOnly)
        {
            return _context.Stars.AsQueryable()
                .Where(p => p.Mass != null && p.Luminosity != null
            && p.Teff != null && p.Type != null && habitableOnly?p.Planets.Any(o=>o.Habitable==true):true)
            .Select(HertzsprungRussellDto.FromEntities).Where(p => p.Color != null);
        }

        public IQueryable<IGrouping<string, Planet>> GetPlanetTypes(ChartType type)
        {
                switch (type) {
                case ChartType.Mass:  return _context.Planets.AsQueryable().Where(p => p.MassClass != "").GroupBy(p => p.MassClass.ToLower());
                case ChartType.Atmospere: return _context.Planets.AsQueryable().Where(p => p.AtmosphereClass!= "").GroupBy(p => p.AtmosphereClass.ToLower());
                case ChartType.DiscoveryMetod: return _context.Planets.AsQueryable().Where(p => p.Disc_Method != "").GroupBy(p => p.Disc_Method.ToLower());
                case ChartType.DiscoveryYear: return _context.Planets.AsQueryable().Where(p => p.Disc_Year != null).GroupBy(p => p.Disc_Year.ToString().ToLower());
                case ChartType.Hability: return _context.Planets.AsQueryable().Where(p => p.HabitableClass != "").GroupBy(p => p.HabitableClass.ToLower());
                case ChartType.Temperature: return _context.Planets.AsQueryable().Where(p => p.ZoneClass != "").GroupBy(p => p.ZoneClass.ToLower());
                case ChartType.Composition: return _context.Planets.AsQueryable().Where(p => p.CompositionClass != "").GroupBy(p => p.CompositionClass.ToLower());
                default: return _context.Planets.GroupBy(p => p.Name);
            }
          
        }
    }
}