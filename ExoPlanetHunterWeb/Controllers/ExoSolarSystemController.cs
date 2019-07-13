using ExoPlanetHunter.Service.Dto;
using ExoPlanetHunter.Service.Enum;
using ExoPlanetHunter.Service.Interfaces;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExoPlanetHunterWeb.Controllers
{
    [Produces("application/json")]
    [Route("api/ExoSolarSystems")]
    [ApiExplorerSettings(IgnoreApi = true)]
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
        public List<ExoSolarSystemDto> GetSolarSystemPerConstellation(ConstellationsEnum constellation, int? page = null)
        {
            return _exoService.GetSolarSystemPerConstellation(constellation, page);
        }

        [HttpGet("GetHabitablePlanets")]
        public List<ExoPlanetsDto> GetHabitablePlanets()
        {
            return _exoService.GetHabitablePlanets();
        }
        [HttpGet("GetHabitablePlanets")]
        public List<ExoPlanetsDto> GetAllPlanets()
        {
            return _exoService.GetAllPlanets();
        }

        [HttpGet("GetPaginatedPlanets")]
        public IEnumerable<ExoPlanetsDto> GetPaginatedPlanets(int page, bool hab, bool moon, string type, ChartType key, string name)
        {
            return _exoService.GetPaginatedPlanets(page, hab, moon, type, key, name);
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