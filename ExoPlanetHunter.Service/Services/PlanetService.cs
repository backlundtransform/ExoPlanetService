using AutoMapper.QueryableExtensions;
using ExoPlanetHunter.Database;

using ExoPlanetHunter.Service.Dto;

using ExoPlanetHunter.Service.Interfaces;
using Microsoft.AspNet.OData.Query;
using Microsoft.EntityFrameworkCore;

using System.Linq;
using System.Threading.Tasks;

namespace ExoPlanetHunter.Service.Services
{
    public class PlanetService : IPlanetService
    {
        private readonly ExoContext _context;

        public PlanetService(ExoContext context)
        {
            _context = context;
        }

        public IQueryable<PlanetDto> GetPlanets(ODataQueryOptions opts)
        {
            IQueryable results = opts.ApplyTo(_context.Planets.ProjectTo<PlanetDto>().AsQueryable());
            return results as IQueryable<PlanetDto>;
        }

        public async Task<PlanetDto> GetPlanet(int id)
        {
            var planet = await _context.Planets.ProjectTo<PlanetDto>().SingleOrDefaultAsync(m => m.Id == id);
            return planet;
        }
    }
}