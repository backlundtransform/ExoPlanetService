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
using  ExoPlanetHunter.Service.Enum;
using System;
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

    public  IQueryable<ExoPlanetsDto> GetExoPlanets(ODataQueryOptions opts)
    {
      IQueryable results = opts.ApplyTo(_context.Planets.OrderByDescending(p=>p.Disc_Year).Include(z=>z.Star).Include(z=>z.Star.Planets).Select(p=>new ExoPlanetsDto{ 
           Name=p.Name,
           Img = new ImgDto(){ Uri= GetPlanetColor(p)},
           DiscYear =p.Disc_Year,
           Comp = p.CompositionClass.ToEnum<CompEnum>(),
           MassType =  p.MassClass.ToEnum<MassEnum>(),
           Atmosphere =p.AtmosphereClass.ToEnum<AtmosEnum>(),
           DiscMethod =p.Disc_Method.ToEnum<DiscEnum>(),
         
           MeanDistance= p.MeanDistance,
           StarDistance =GetStarDistance(p, p.MeanDistance),
           Star = new ExoStarDto(){
                Constellation =p.Star.Constellation.Name.ToEnum<ConstellationsEnum>(),
               Color =GetStarColor(p),
               HabZoneMax = GetStarDistance(p, p.Star.HabZoneMax),
               HabZoneMin = GetStarDistance(p, p.Star.HabZoneMin),
           }
           }).AsQueryable());
        return results as IQueryable<ExoPlanetsDto>;
    }

     private decimal? GetStarDistance(Planet p,decimal? distance)
     {

        var lastplanet =p.Star?.Planets.OrderByDescending(c=>c.MeanDistance).Last();
        var habzonemax =p.Star?.HabZoneMax;
        if(lastplanet?.MeanDistance>habzonemax)
        {

            return 600*distance/lastplanet.MeanDistance;
        }

        return 600*distance/habzonemax;
        
    }

    private int? GetStarColor(Planet p)
     {
          var type = p.Star?.Type;

          if(type==null)
          {
              return null;
          }
           if(type.StartsWith("O")||type.StartsWith("B")||type.StartsWith("A"))
        {
           return 0;
        }
       
        if(type.StartsWith("F"))
        {
            return 1;
        }
        if(type.StartsWith("G"))
        {
            return 2;
        }
        if(type.StartsWith("K") || type.StartsWith("M"))
        {
            return 3;
        }
        
      
        
        return null;
      
     }

    private string GetPlanetColor(Planet p)
     {

    	if(p.AtmosphereClass.Equals("no-atmosphere"))
        {
            if(p.MassClass.Equals("Jovian"))
            {

                return "jovian";
            }
	   		return "iron";
	   	}
	   	else
	   	{
	   	 
	   	  if(p.ZoneClass.Equals("Hot"))
             {

                    if(p.MassClass.Equals("Superterran"))
                    {
                        return "hotsuperearth";
                    }

                    if(p.MassClass.Equals("Jovian"))
                    {
                        return "hotjupiter";
                    }

                    if(p.MassClass.Equals("Terran") || p.MassClass.Equals("SubTerran"))
                    {
                        return "hotstone";
                    }
	   		  
	   	  }
           
           if(p.ZoneClass.Equals("Cold"))
             {

                     if(p.MassClass.Equals("Superterran"))
                    {
                        return "coldsuperearth";
                    }

                    if(p.MassClass.Equals("Jovian"))
                    {
                        return "jovian";
                    }
                    if(p.MassClass.Equals("Terran") || p.MassClass.Equals("SubTerran"))
                    {
                        return "coldstone";
                    }
	   	    else{
	   		  
                     if(p.MassClass.Equals("Superterran"))
                    {
                        return "superearth";
                    }

                    if(p.MassClass.Equals("Jovian"))
                    {
                        return "jovian";
                    }
                    if(p.MassClass.Equals("Terran") || p.MassClass.Equals("SubTerran"))
                    {
                        return "stone";
                    }
	   		 }
                
	   	  }
             return "noimg";
	   	}
        
    }

        public async Task<PlanetDto> GetPlanet(int id)
        {
            var planet = await _context.Planets.ProjectTo<PlanetDto>().SingleOrDefaultAsync(m => m.Id == id);
            return planet;
        }
    }
}