using AutoMapper.QueryableExtensions;
using ExoPlanetHunter.Database;
using ExoPlanetHunter.Service.Dto;
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

        public IEnumerable<ConstellationDto> GetConstellations()
        {
            return _context.Constellations.Include(c=> c.Stars).Where(p => true).ProjectTo<ConstellationDto>();
        }
        public async Task<ConstellationDto> GetConstellation(int cid)
        {
            return await _context.Constellations.Include(e => e.Stars).ProjectTo<ConstellationDto>().SingleOrDefaultAsync(c => c.Id == cid);
        }

        public async Task<ConstellationStarsDto> GetConstellationWithStars(int cid)
        {
            return await _context.Constellations.Include(e => e.Stars).ProjectTo<ConstellationStarsDto>().SingleOrDefaultAsync(c => c.Id == cid);
        }

      
    }
}