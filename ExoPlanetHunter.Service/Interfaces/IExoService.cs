using ExoPlanetHunter.Service.Dto;
using ExoPlanetHunter.Service.Enum;
using Microsoft.AspNet.OData.Query;
using System.Collections.Generic;
using System.Linq;

namespace ExoPlanetHunter.Service.Interfaces
{
    public interface IExoService
    {

        List<ExoSolarSystemDto> GetSolarSystemPerConstellation(ConstellationsEnum id, int? page=null);

        ExoSolarSystemDto  GetSolarSystemPerStar(string name);

        IQueryable<ExoPlanetsDto> GetExoPlanets(ODataQueryOptions opts);

        IEnumerable<ExoPlanetsDto> CacheExoPlanets();

        List<ExoPlanetsDto> GetHabitablePlanets();
    }
}
