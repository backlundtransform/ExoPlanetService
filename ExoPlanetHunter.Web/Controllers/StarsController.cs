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
    [Route("api/Stars")]
    public class StarsController : Controller
    {
        private readonly IStarService _starService;
        /// <summary>
        /// </summary>

        public StarsController(IStarService starService)
        {
            _starService = starService;
        }

        /// <summary>
        /// </summary>
     
        [HttpGet]
      
        public async Task<IQueryable<StarDto>> Get(ODataQueryOptions opts=null)
        {
            return await _starService.GetStars(opts);
        }
        /// <summary>
        /// </summary>

        [HttpGet("{id}")]
        public async Task<StarDto> Get(int id)
        {
            var star = await _starService.GetStar(id);

            return star;
        }
        /// <summary>
        /// </summary>

        [HttpGet("{id}/planets")]
        public async Task<StarPlanetsDto> GetPlanets(int id)
        {
            var planets = await _starService.GetStarPlanets(id);
            return planets;
        }
    }
}