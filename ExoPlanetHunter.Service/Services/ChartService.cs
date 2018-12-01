using ExoPlanetHunter.Database;
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

        public List<HertzsprungRussellDto> GetHertzsprungRussell()
        {
            return _context.Stars.AsQueryable().Select(HertzsprungRussellDto.FromEntities).ToList();
        }
    }
}