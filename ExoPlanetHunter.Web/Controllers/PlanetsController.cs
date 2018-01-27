using ExoPlanetHunter.Service.Dto;
using ExoPlanetHunter.Service.Interfaces;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ExoPlanetHunter.Web.Controllers
{
    /// <summary>
    /// </summary>

    [Produces("application/json")]
    [Route("api/Planets")]
    public class PlanetsController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(PlanetsController));
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
            try
            {
                return await _planetService.GetPlanet(id);
            }
            catch (Exception e)
            {
                log.Info(e.Message);
                return new PlanetDto() { Message = e.Message };
            }
        }

        /// <summary>
        /// </summary>

        [HttpGet]
        public IQueryable<PlanetDto> Get(ODataQueryOptions opts)
        {

            try
            {
                return _planetService.GetPlanets(opts); 
            }
            catch (Exception e)
            {
                log.Info(e.Message);
                return null;
            }
         
        }
    }
}