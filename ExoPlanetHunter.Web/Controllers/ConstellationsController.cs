using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExoPlanetHunter.Pocos;
using ExoPlanetHunter.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExoPlanetHunter.Web.Controllers
{
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
            public async Task<IEnumerable<Constellation>> Get()
            {
                return await Task.FromResult(_constellationService.GetConstellations());
            }

            [HttpGet("{cid}")]
            public async Task<object> Get(int cid)
            {
                var constellation = await _constellationService.GetConstellation(cid);

                var stars = await Task.FromResult(_constellationService.GetStarsByConstellation(cid));

                return new { Constellation = constellation, Stars = stars.ToList().Count() };
            }


            [HttpGet("{cid}/stars")]
            public async Task<object> GetStars(int cid)
            {
            var constellation = await _constellationService.GetConstellation(cid);

            var stars = await Task.FromResult(_constellationService.GetStarsByConstellation(cid));

            return new { Constellation = constellation, Stars = stars };
            }

        [HttpGet("{cid}/stars/{sid}")]
        public async Task<object> GetStar(int id)
        {
            var star = await _starService.GetStar(id);

            var planets = await Task.Run(() => _starService.GetStarPlanets(id));

            return new { Star = star, Planets = planets.Count() };
        }

        [HttpGet("{cid}/stars/{sid}/planets")]
        public async Task<object> GetStarPlanets(int id)
        {
            var planets = await Task.Run(() => _starService.GetStarPlanets(id)); ;
            return new { Star = id, Planets = planets };
        }

       
        }
}