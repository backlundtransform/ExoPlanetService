using ExoPlanetHunter.Database;
using ExoPlanetHunter.Pocos;
using ExoPlanetHunter.Service.Interfaces;
using Microsoft.AspNet.OData.Query;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExoPlanetHunter.Service
{
    internal class StarService : IStarService
    {
        private readonly ExoContext _context;

        public StarService(ExoContext context)
        {
            _context = context;
        }

        public Task<IQueryable<Star>> GetStars(ODataQueryOptions opts)
        {
            IQueryable results = opts.ApplyTo(_context.Stars.AsQueryable());
            return Task.FromResult(results as IQueryable<Star>);
        }

        public async Task<Star> GetStar(int id)
        {
            return await _context.Stars.SingleOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Planet>> GetStarPlanets(int id)
        {
            return await Task.Run(() => _context.Planets.Where(c => c.Star.Id == id));
        }
    }
}