using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExoPlanetHunter.Service.Dto;
using ExoPlanetHunter.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExoPlanetHunterWeb.Controllers
{
    [Produces("application/json")]
    [Route("api/Statistics")]
    public class StatisticsController : Controller
    {

        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService =statisticsService;
        }


        [HttpGet]
        public StatisticsDto Get()
        {

            return _statisticsService.GetStatistics();
          
        }
    }
}