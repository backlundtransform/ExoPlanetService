using ExoPlanetHunter.Database;
using ExoPlanetHunter.Database.entity;
using ExoPlanetHunter.Service.Dto;
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

        public IQueryable<HertzsprungRussellDto> GetHertzsprungRussell()
        {
            return _context.Stars.AsQueryable()
                .Where(p => p.Mass != null && p.Luminosity != null
            && p.Teff != null && p.Type != null)
            .Select(HertzsprungRussellDto.FromEntities).Where(p => p.Color != null);
        }

        public IQueryable<IGrouping<string, Planet>> GetPlanetTypes()
        {
            return _context.Planets.AsQueryable().Where(p => p.MassClass != "").GroupBy(p => p.MassClass);
        }
    }
}