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

        [HttpGet("GetPaginatedPlanets")]
        public List<ExoPlanetsDto> GetPaginatedPlanets(int page, bool hab, bool moon, string type, ChartType key)
        {
            if (hab)
            {
                return _exoService.CacheExoPlanets().Where(p => p.Hab == true).OrderByDescending(p => p.DiscYear).Skip(page * 30).Take(30).ToList();
            }
            if (moon)
            {
                return _exoService.CacheExoPlanets().Where(p => p.Moons == true).OrderByDescending(p => p.DiscYear).Skip(page * 30).Take(30).ToList();
            }

            if (!string.IsNullOrEmpty(type))
            {
                switch (key)
                {
                    case ChartType.Mass: return _exoService.CacheExoPlanets().Where(p => p.MassType == (int)type.ToEnum<MassEnum>()).OrderByDescending(p => p.DiscYear).Skip(page * 30).Take(30).ToList();
                    case ChartType.Atmospere: return _exoService.CacheExoPlanets().Where(p => p.Atmosphere == (int)type.ToEnum<AtmosEnum>()).OrderByDescending(p => p.DiscYear).Skip(page * 30).Take(30).ToList();
                    case ChartType.DiscoveryMetod: return _exoService.CacheExoPlanets().Where(p => p.DiscMethod == (int)type.ToEnum<DiscEnum>()).OrderByDescending(p => p.DiscYear).Skip(page * 30).Take(30).ToList();
                    case ChartType.Hability: return _exoService.CacheExoPlanets().Where(p => p.HabType == (int)type.ToEnum<HabEnum>()).OrderByDescending(p => p.DiscYear).Skip(page * 30).Take(30).ToList();
                    case ChartType.Temperature: return _exoService.CacheExoPlanets().Where(p => p.TempZone == (int)type.ToEnum<TempEnum>()).OrderByDescending(p => p.DiscYear).Skip(page * 30).Take(30).ToList();
                    case ChartType.Composition: return _exoService.CacheExoPlanets().Where(p => p.Comp== (int)type.ToEnum<CompEnum>()).OrderByDescending(p => p.DiscYear).Skip(page * 30).Take(30).ToList();
                }
            }
            return _exoService.CacheExoPlanets().OrderByDescending(p => p.DiscYear).Skip(page * 30).Take(30).ToList();
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