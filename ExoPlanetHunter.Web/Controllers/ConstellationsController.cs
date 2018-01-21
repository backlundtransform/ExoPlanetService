using ExoPlanetHunter.Pocos;
using ExoPlanetHunter.Service.Dto;
using ExoPlanetHunter.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExoPlanetHunter.Web.Controllers
{
    public class ConsResponse
    {
    }

    [Produces("application/json")]
    [Route("api/Constellations")]
    public class ConstellationsController : Controller
    {
        private readonly IConstellationService _constellationService;
        private readonly IStarService _starService;

        public ConstellationsController(IConstellationService constellationService, IStarService starService)
        {
            _starService = starService;
            _constellationService = constellationService;
        }

        [HttpGet]
        public async Task<IEnumerable<ConstellationDto>> Get()
        {
            return await Task.FromResult(_constellationService.GetConstellations().Select(p=> new ConstellationDto(p)));
        }

        [HttpGet("{cid}")]
        public async Task<ConstellationDto> Get(int cid)
        {
            var constellation = await _constellationService.GetConstellation(cid);

            return new ConstellationDto(constellation);
        }

        [HttpGet("{cid}/stars")]
        public async Task<Constellation> GetStars(int cid)
        {
            var constellation = await _constellationService.GetConstellation(cid);

            return constellation;
        }

        [HttpGet("{cid}/stars/{sid}")]
        public async Task<StarDto> GetStar(int sid)
        {
            var star = await _starService.GetStar(sid);

            var planets = await Task.FromResult(_starService.GetStarPlanets(sid));

            return  new StarDto(star);
        }

        [HttpGet("{cid}/stars/{sid}/planets")]
        public async Task<StarPlanetsDto> GetStarPlanets(int sid)
        {
            var planets = await _starService.GetStarPlanets(sid); 
            return new StarPlanetsDto(planets.ToList(), sid);
        }
    }
}