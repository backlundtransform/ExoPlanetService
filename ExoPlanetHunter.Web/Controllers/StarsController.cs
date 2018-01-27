using ExoPlanetHunter.Service.Dto;
using ExoPlanetHunter.Service.Interfaces;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using System;
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
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(StarsController));
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
        public async Task<IQueryable<StarDto>> Get(ODataQueryOptions opts)
        {
            try
            {
                return await _starService.GetStars(opts);
            }
            catch (Exception e)
            {
                log.Info(e.Message);
                return null;
            }
        }

        /// <summary>
        /// </summary>

        [HttpGet("{id}")]
        public async Task<StarDto> Get(int id)
        {
            try
            {
                return await _starService.GetStar(id);
            }
            catch (Exception e)
            {
                log.Info(e.Message);
                return new StarDto() { Message = e.Message };
            }
        }

        /// <summary>
        /// </summary>

        [HttpGet("{id}/planets")]
        public async Task<StarPlanetsDto> GetPlanets(int id)
        {

            try
            {
                return await _starService.GetStarPlanets(id);
            }
            catch (Exception e)
            {
                log.Info(e.Message);
                return new StarPlanetsDto() { Message = e.Message };
            }
          
        }
    }
}