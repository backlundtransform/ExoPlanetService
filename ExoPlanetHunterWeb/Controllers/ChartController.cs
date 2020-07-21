using ExoPlanetHunter.Service.Dto;
using ExoPlanetHunter.Service.Enum;
using ExoPlanetHunter.Service.Interfaces;
using ExoPlanetHunterWeb.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExoPlanetHunterWeb.Controllers
{
    [Produces("application/json")]
    [Route("api/Chart")]
    public class ChartController : Controller
    {
        private readonly IChartService _chart;
        private readonly IMastService _mast;

        public ChartController(IChartService chart, IMastService mast)
        {
            _chart = chart;
            _mast = mast;
        }

        [HttpGet("HertzsprungRussell")]
        public IQueryable<HertzsprungRussellDto> GetHertzsprungRussell(bool habitableOnly=true)
        {
            return _chart.GetHertzsprungRussell(habitableOnly);
        }

        [HttpGet("PlanetTypes")]
        public IQueryable<PlanetTypes> GetPlanetTypes(ChartType type)
        {
            return _chart.GetPlanetTypes(type).Select(p=>new PlanetTypes { Title=p.Key, Value=p.Count()});
        }

        
        [HttpGet("PlanetDistance")]
        public IQueryable<PlanetDistanceDto> GetPlanetDistance(double? max)
        {
            return _chart.GetPlanetDistance(max);
        }

        [HttpGet("EsiDistance")]
        public List<EsiDistanceDto> GetEsiDistance()
        {
            return _chart.GetEsiDistance();
        }
        [HttpGet("MassOrbit")]
        public List<MassOrbitDto> GetMassOrbit()
        {
            return _chart.GetMassOrbit();
        }

        [HttpGet("Mast")]
        public async Task<List<TransitTimeserie>> GetMast(double ra, double dec, double radius)
        {
            return await _mast.GetMast(ra,  dec, radius);
        }

    }
}