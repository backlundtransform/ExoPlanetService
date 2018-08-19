using ExoPlanetHunter.Database;
using ExoPlanetHunter.Service.Dto;
using ExoPlanetHunter.Service.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;

namespace ExoPlanetHunter.Service.Services
{
    public class StatisticsService : IStatisticsService
    {
        private IMemoryCache _cache;
        private readonly ExoContext _context;
        private readonly IExoService _exoService;
        public StatisticsService(ExoContext context, IMemoryCache memoryCache)
        {
            _cache = memoryCache;
            _context = context;
        }

        public StatisticsDto GetStatistics()
        {
            var confirmed = _context.Planets.Count();

            var confirmedhab = _context.Planets.Count(p => p.Habitable == true);
            var habMoons = _context.Planets.Count(p => p.HabMoon == true);
            var dateUpdated = DateTime.Now;
            _cache.TryGetValue("DateUpdated", out dateUpdated);

            return new StatisticsDto
            {
                ConfirmedHabitablePlanets = confirmedhab,
                ConfirmedPlanets = confirmed,
                PossibleHabitableMoons = habMoons,
                DateUpdated = dateUpdated,
            };
        }
    }
}