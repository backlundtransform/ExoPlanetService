using ExoPlanetHunter.Service.Dto;
using ExoPlanetHunter.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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
        public List<HertzsprungRussellDto> GetHertzsprungRussell()
        {
            return _chart.GetHertzsprungRussell();
        }
    }
}