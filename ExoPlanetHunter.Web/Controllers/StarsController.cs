using ExoPlanetHunter.Pocos;
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
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IQueryable<Star>> Get(ODataQueryOptions opts)
        {
            return await Task.Run(() => _starService.GetStars(opts));
        }

        [HttpGet("{id}")]
        public async Task<object> Get(int id)
        {
            var star = await _starService.GetStar(id);

            var planets = await Task.Run(() => _starService.GetStarPlanets(id));

            return new { Star = star, Planets = planets.Count() };
        }

        [HttpGet("{id}/planets")]
        public async Task<object> GetPlanets(int id)
        {
            var planets = await Task.Run(() => _starService.GetStarPlanets(id)); ;
            return new { Star = id, Planets = planets };
        }
    }
}