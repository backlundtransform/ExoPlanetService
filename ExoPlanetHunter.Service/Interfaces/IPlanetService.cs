
using ExoPlanetHunter.Service.Dto;
using Microsoft.AspNet.OData.Query;
using System.Linq;
using System.Threading.Tasks;

using System.Collections.Generic;
namespace ExoPlanetHunter.Service.Interfaces
{
    public interface IPlanetService
    {
        IQueryable<PlanetDto> GetPlanets(ODataQueryOptions opts);

        Task<PlanetDto> GetPlanet(int id);
        
        IQueryable<ExoPlanetsDto> GetExoPlanets(ODataQueryOptions opts);
    }
}
