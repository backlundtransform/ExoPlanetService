using ExoPlanetHunter.Service.Dto;
using ExoPlanetHunter.Service.Interfaces;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ExoPlanetHunter.Web.ViewModel;
using Newtonsoft.Json;
namespace ExoPlanetHunter.Web.Controllers
{
   
    [Produces("application/json")]
    [Route("api/Planets")]
    public class PlanetsController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(PlanetsController));
        private readonly IPlanetService _planetService;
        
        public PlanetsController(IPlanetService planetService)
        {
            _planetService = planetService;
        }

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
        /// <remarks>
        ///E.g if you want to find habitable planets "$filter= Habitable eq true".
        /// 
        ///Find planet with name 14 And b "$filter=Name eq '14 And b'".
        /// 
        ///Read more here: https://msdn.microsoft.com/en-us/library/hh169248(v=nav.90).aspx.
        ///
        ///Skip and top E.g takes 30 and skips 10 "$top=30 $skip=10".
        /// 
        ///Order by ascending discovery year $orderby=Disc_Year asc.
        /// </remarks>

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

        [HttpGet("ExoPlanets")]
        public IQueryable<ExoPlanetsDto> GetExoPlanets(ODataQueryOptions opts)
        {
            
                return  _planetService.GetExoPlanets(opts);
           
        
    }

    [HttpGet("GetImages")]
      public Dictionary<string,string> GetImages()
        {
            try
            {
                string json = System.IO.File.ReadAllText("colors.json");
                return  JsonConvert.DeserializeObject<Dictionary<string,string>>(json);
              
            }
            catch (Exception e)
            {
                log.Info(e.Message);
                return null;
            }
        
    }
    }
}