using ExoPlanetHunter.Database;
using ExoPlanetHunter.Pocos;
using ExoPlanetHunter.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExoPlanetHunter.Service.Services
{
    public class ConstellationService : IConstellationService
    {
        private readonly ExoContext _context;

        public ConstellationService(ExoContext context)
        {
            _context = context;
        }

        public IEnumerable<Constellation> GetConstellations()
        {
            return _context.Constellations.Include(c=> c.Stars).Where(p => true);
        }
        public async Task<Constellation> GetConstellation(int cid)
        {
            return await _context.Constellations.Include(e => e.Stars).SingleOrDefaultAsync(c => c.Id == cid);
        }

        public IEnumerable<Star> GetStarsByConstellation(int cid)
        {

            return _context.Stars.Include(e => e.Planets).Where(c => c.Constellation.Id == cid);
        }
    }
}