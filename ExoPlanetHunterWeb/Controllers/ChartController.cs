using ExoPlanetHunter.Service.Dto;
using ExoPlanetHunter.Service.Interfaces;
using ExoPlanetHunterWeb.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace ExoPlanetHunterWeb.Controllers
{
    [Produces("application/json")]
    [Route("api/Chart")]
    public class ChartController : Controller
    {
        private readonly IChartService _chart;

        public ChartController(IChartService chart)
        {
            _chart = chart;
        }

        [HttpGet("HertzsprungRussell")]
        public IQueryable<HertzsprungRussellDto> GetHertzsprungRussell(bool habitableOnly=true)
        {
            return _chart.GetHertzsprungRussell(habitableOnly);
        }

        [HttpGet("PlanetTypes")]
        public IQueryable<PlanetTypes> GetPlanetTypes()
        {
            return _chart.GetPlanetTypes().Select(p=>new PlanetTypes { Title=p.Key, Value=p.Count()});
        }
    }
}