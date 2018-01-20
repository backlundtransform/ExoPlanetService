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
            return _context.Constellations.Where(p => true);
        }
        public async Task<Constellation> GetConstellation(int cid)
        {
            return await _context.Constellations.SingleOrDefaultAsync(c => c.Id == cid);
        }

        public IEnumerable<Star> GetStarsByConstellation(int cid)
        {
            return _context.Stars.Where(c => c.Constellation.Id == cid);
        }
    }
}