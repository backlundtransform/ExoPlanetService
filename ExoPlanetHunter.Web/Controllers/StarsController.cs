using ExoPlanetHunter.Service.Dto;
using ExoPlanetHunter.Service.Interfaces;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ExoPlanetHunter.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/Stars")]
    public class StarsController : Controller
    {
        private readonly IStarService _starService;

        public StarsController(IStarService starService)
        {
            _starService = starService;
        }
        [HttpGet]
      
        public async Task<IQueryable<StarDto>> Get(ODataQueryOptions opts)
        {
            return await _starService.GetStars(opts);
        }

        [HttpGet("{id}")]
        public async Task<StarDto> Get(int id)
        {
            var star = await _starService.GetStar(id);

            return star;
        }

        [HttpGet("{id}/planets")]
        public async Task<StarPlanetsDto> GetPlanets(int id)
        {
            var planets = await _starService.GetStarPlanets(id);
            return planets;
        }
    }
}