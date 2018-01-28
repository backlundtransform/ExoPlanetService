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
    

        public StarsController(IStarService starService)
        {
            _starService = starService;
        }

        /// <summary>
        /// </summary>
        /// <remarks>
        /// E.g if you want to find all planets at a distance greater then 110 an less than 140 then use "$filter=Distance gt 110 and Distance lt 140".
        /// 
        ///Find star with name SR 12 AB "$filter=Name eq 'SR 12 AB'".
        /// 
        ///Read more here: https://msdn.microsoft.com/en-us/library/hh169248(v=nav.90).aspx
        ///
        ///Skip and top E.g takes 30 and skips 10 "$top=30 $skip=10".
        /// 
        ///Order by decending number of planets $orderby=Planets desc.
        /// </remarks>


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