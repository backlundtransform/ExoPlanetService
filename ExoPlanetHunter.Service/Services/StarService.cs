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
    internal class StarService : IStarService
    {
        private readonly ExoContext _context;

        public StarService(ExoContext context)
        {
            _context = context;
        }

        public Task<IQueryable<StarDto>> GetStars(ODataQueryOptions opts)
        {
            IQueryable results = opts.ApplyTo(_context.Stars.ProjectTo<StarDto>().AsQueryable());
            return Task.FromResult(results as IQueryable<StarDto>);
        }

        public async Task<StarDto> GetStar(int id)
        {
            return await _context.Stars.ProjectTo<StarDto>().SingleOrDefaultAsync(m => m.Id == id);
        }

        public async Task<StarPlanetsDto> GetStarPlanets(int id)
        {
            return await _context.Stars.ProjectTo<StarPlanetsDto>().SingleOrDefaultAsync(m => m.Id == id);
        }
    }
}