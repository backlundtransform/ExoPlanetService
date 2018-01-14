using ExoPlanetHunter.Pocos;
using Microsoft.AspNet.OData.Query;
using System.Linq;
using System.Threading.Tasks;

namespace ExoPlanetHunter.Service.Interfaces
{
    public interface IPlanetService
    {
        IQueryable<Planet> GetPlanets(ODataQueryOptions opts);

        Task<Planet> GetPlanet(int id);
    }
}
