using ExoPlanetHunter.Pocos;
using Microsoft.AspNet.OData.Query;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExoPlanetHunter.Service.Interfaces
{
    public interface IStarService
    {
        Task<IQueryable<Star>> GetStars(ODataQueryOptions opts);

        Task<Star> GetStar(int id);

        Task<IEnumerable<Planet>> GetStarPlanets(int id);
    }
}