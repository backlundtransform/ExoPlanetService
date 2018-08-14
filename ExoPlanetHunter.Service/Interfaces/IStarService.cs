using ExoPlanetHunter.Service.Dto;
using ExoPlanetHunter.Service.Enum;
using Microsoft.AspNet.OData.Query;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExoPlanetHunter.Service.Interfaces
{
    public interface IStarService
    {
        Task<IQueryable<StarDto>> GetStars(ODataQueryOptions opts);

        Task<StarDto> GetStar(int id);

        Task<StarPlanetsDto> GetStarPlanets(int id);

    }
}