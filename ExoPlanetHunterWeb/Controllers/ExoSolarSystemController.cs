using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExoPlanetHunter.Service.Dto;
using ExoPlanetHunter.Service.Enum;
using ExoPlanetHunter.Service.Interfaces;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ExoPlanetHunterWeb.Controllers
{
    [Produces("application/json")]
    [Route("api/ExoSolarSystems")]
  
    public class ExoSolarSystemController : Controller
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ExoSolarSystemController));
        private readonly IExoService _exoService;

        public ExoSolarSystemController(IExoService exoService)
        {
            _exoService = exoService;
        }


        [HttpGet("ExoPlanets")]
        public IQueryable<ExoPlanetsDto> GetExoPlanets(ODataQueryOptions opts)
        {

            return _exoService.GetExoPlanets(opts);


        }

        [HttpGet("GetExoSolarSystemByName")]
        public ExoSolarSystemDto GetExoSolarSystemByName(string name)
        {

            return _exoService.GetSolarSystemPerStar(name);


        }

        [HttpGet("GetSolarSystemPerConstellation")]
        public List<ExoSolarSystemDto> GetSolarSystemPerConstellation(ConstellationsEnum constellation)
        {

            return _exoService.GetSolarSystemPerConstellation(constellation);


        }

        [HttpGet("GetHabitablePlanets")]
        public List<ExoPlanetsDto> GetHabitablePlanets()
        {

            return _exoService.GetHabitablePlanets();

        }
      

        [HttpGet("GetImages")]
        public Dictionary<string, string> GetImages()
        {
            try
            {
                string json = System.IO.File.ReadAllText("colors.json");
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            }
            catch (Exception e)
            {
                log.Info(e.Message);
                return null;
            }

        }
    }
}