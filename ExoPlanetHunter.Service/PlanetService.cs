using ExoPlanetHunter.Database;
using ExoPlanetHunter.Pocos;
using ExoPlanetHunter.Service.Interfaces;
using Microsoft.AspNet.OData.Query;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ExoPlanetHunter.Service
{
    public class PlanetService : IPlanetService
    {
        private readonly ExoContext _context;

        public PlanetService(ExoContext context)
        {
            _context = context;
        }

        public IQueryable<Planet> GetPlanets(ODataQueryOptions opts)
        {
            IQueryable results = opts.ApplyTo(_context.Planets.AsQueryable());
            return results as IQueryable<Planet>;
        }

        public async Task<Planet> GetPlanet(int id)
        {
            var planet = await _context.Planets.SingleOrDefaultAsync(m => m.Id == id);
            return planet;
        }
    }
}