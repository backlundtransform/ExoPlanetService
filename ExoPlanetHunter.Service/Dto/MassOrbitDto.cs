using System;
using System.Collections.Generic;
using System.Text;

namespace ExoPlanetHunter.Service.Dto
{
    public class MassOrbitDto
    {
        public double Mass { get; set; }
        public string StarName{ get; set; }
        public string PlanetName { get; set; }
        public string Color { get; set; }
        public double Orbit { get; set; }
    }
}
