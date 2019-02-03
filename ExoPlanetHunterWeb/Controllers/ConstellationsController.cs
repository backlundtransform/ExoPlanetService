using ExoPlanetHunter.Service.Dto;
using ExoPlanetHunter.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

using System.Threading.Tasks;

namespace ExoPlanetHunter.Web.Controllers
{
    
    [Produces("application/json")]
    [Route("api/Constellations")]
    public class ConstellationsController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ConstellationsController));
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
            
            try
            {
                return await Task.FromResult(_constellationService.GetConstellations());
            }
            catch (Exception e)
            {
                log.Info(e.Message);
                return new List<ConstellationDto>();
            }
        }

  

        [HttpGet("{cid}")]
        public async Task<ConstellationDto> Get(int cid)
        {

            try
            {
                var constellation = await _constellationService.GetConstellation(cid);

                return constellation;
            }
            catch (Exception e)
            {
                log.Info(e.Message);
                return new ConstellationDto() { Message = e.Message };
            }
            
        }

     

        [HttpGet("{cid}/stars")]
        public async Task<ConstellationStarsDto> GetStars(int cid)
        {
            try
            {
                var constellation = await _constellationService.GetConstellationWithStars(cid);

                return constellation;
            }
            catch (Exception e)
            {
                log.Info(e.Message);
                return new ConstellationStarsDto() { Message = e.Message };
            }
        }

     

        [HttpGet("{cid}/stars/{sid}")]
        public async Task<StarDto> GetStar(int sid)
        {
            try
            {
                var star = await _starService.GetStar(sid);

                return star;
            }
            catch (Exception e)
            {
                log.Info(e.Message);
                return new StarDto() { Message = e.Message };
            }
        }

        

        [HttpGet("{cid}/stars/{sid}/planets")]
        public async Task<StarPlanetsDto> GetStarPlanets(int sid)
        {
            try
            {
                var planets = await _starService.GetStarPlanets(sid);
                return planets;
            }
            catch (Exception e)
            {
                log.Info(e.Message);
                return new StarPlanetsDto() { Message = e.Message };
            }
        }
    }
}