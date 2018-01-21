using ExoPlanetHunter.Pocos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExoPlanetHunter.Service.Dto
{
    public class StarDto
    {

        public StarDto(Star star)
        {
           
            Planets = star.Planets?.ToList().Count();
         
            Star = star;

        }

        public Star Star { get; set; }

        public int? Planets{ get; set; }
    }
}
