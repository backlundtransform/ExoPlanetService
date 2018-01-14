
using ExoPlanetHunter.Pocos;
using ExoPlanetHunter.Service.Interfaces;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;

using System.Linq;
using System.Threading.Tasks;

namespace ExoPlanetHunter.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/Planets")]
    public class PlanetsController : Controller
    {
        private readonly IPlanetService _planetService;
        
        public PlanetsController(IPlanetService planetService)  
        {
            _planetService = planetService;
        }

        // GET: api/Planets/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlanet([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var planet = await _planetService.GetPlanet(id);

            if (planet == null)
            {
                return NotFound();
            }

            return Ok(planet);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public IQueryable<Planet> GetPlanets(ODataQueryOptions opts)
        {

            return _planetService.GetPlanets(opts);
        }
    }
}