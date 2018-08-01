using ExoPlanetHunter.Service.Dto;
using ExoPlanetHunter.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading.Tasks;
using GeoJSON.Net.Feature;
namespace ExoPlanetHunter.Web.Controllers
{
    
    [Produces("application/json")]
    [Route("api/Maps")]
    public class MapsController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ConstellationsController));
       

         [HttpGet("ConstellationsLines")]
        public FeatureCollection ConstellationsLines()
        {
            
            try
            {
               string json = System.IO.File.ReadAllText("constellations.json");
               var lines  = JsonConvert.DeserializeObject<FeatureCollection>(json);
                return lines;
            }
            catch (Exception e)
            {
                log.Info(e.Message);
                return null;
            }
        }

         [HttpGet("StarMarkers")]
        public FeatureCollection StarMarkers()
        {
            
            try
            {
               string json = System.IO.File.ReadAllText("stars.json");
               var lines  = JsonConvert.DeserializeObject<FeatureCollection>(json);
                return lines;
            }
            catch (Exception e)
            {
                log.Info(e.Message);
                return null;
            }
        }
    }     
 }     

