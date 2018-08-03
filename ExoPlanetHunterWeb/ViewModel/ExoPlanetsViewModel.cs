using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExoPlanetHunter.Service.Dto;
namespace ExoPlanetHunter.Web.ViewModel
{
    public class ExoPlanetsViewModel
    {
        public List<ExoPlanetsDto> PlanetList { get;  set; }
        public Dictionary<string,string> Colors { get; set; }

         public string Message { get;  set; }

       
    }
}
