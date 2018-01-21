using ExoPlanetHunter.Service.Dto;
using ExoPlanetHunter.Service.Interfaces;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;

using System.Linq;
using System.Threading.Tasks;

namespace ExoPlanetHunter.Web.Controllers
{
    /// <summary>
    /// </summary>

    [Produces("application/json")]
    [Route("api/Planets")]
    public class PlanetsController : Controller
    {
        private readonly IPlanetService _planetService;
        /// <summary>
        /// </summary>

        public PlanetsController(IPlanetService planetService)  
        {
            _planetService = planetService;
        }
        /// <summary>
        /// </summary>

        [HttpGet("{id}")]
        public async Task<PlanetDto> GetPlanet(int id)
        {
          
            var planet = await _planetService.GetPlanet(id);

          
            return planet;
        }
        /// <summary>
        /// </summary>

        [HttpGet]
        public IQueryable<PlanetDto> Get(ODataQueryOptions opts)
        {

            return _planetService.GetPlanets(opts);
        }
    }
}