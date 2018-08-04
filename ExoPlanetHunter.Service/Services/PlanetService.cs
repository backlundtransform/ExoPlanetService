using AutoMapper.QueryableExtensions;
using ExoPlanetHunter.Database;
using ExoPlanetHunter.Service.Dto;
using ExoPlanetHunter.Service.Interfaces;
using Microsoft.AspNet.OData.Query;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using ExoPlanetHunter.Database.entity;
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

    public async Task<List<ExoPlanetsDto>> GetExoPlanets(int skip=0)
    {
      return await _context.Planets.OrderByDescending(p=>p.Disc_Year).Include(z=>z.Star).Skip(skip).Take(500).Select(p=>new ExoPlanetsDto{ 
           Name=p.Name,
           DiscYear =p.Disc_Year,
           MeanDistance= p.MeanDistance,
           StarDistance =GetStarDistance(p, p.MeanDistance),
           Star = new ExoStarDto(){

               HabZoneMax = GetStarDistance(p, p.Star.HabZoneMax),
               HabZoneMin = GetStarDistance(p, p.Star.HabZoneMin),
           }
           }).ToListAsync();
        
    }

     private decimal? GetStarDistance(Planet p,decimal? distance)
    {

        var lastplanet =p.Star.Planets.OrderByDescending(c=>c.MeanDistance).Last();
        var habzonemax =p.Star.HabZoneMax;
        if(lastplanet.MeanDistance>habzonemax)
        {

            return 600*distance/lastplanet.MeanDistance;
        }

        return 600*distance/habzonemax;
        
    }

        public async Task<PlanetDto> GetPlanet(int id)
        {
            var planet = await _context.Planets.ProjectTo<PlanetDto>().SingleOrDefaultAsync(m => m.Id == id);
            return planet;
        }
    }
}