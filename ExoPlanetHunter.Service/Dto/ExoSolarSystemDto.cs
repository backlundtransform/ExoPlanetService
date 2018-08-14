using ExoPlanetHunter.Service.Enum;
using System.Collections.Generic;

namespace ExoPlanetHunter.Service.Dto
{
    public class ExoSolarSystemDto
    {
        public string Name { get; set; }

  

        public int? Color { get; set; }


        public decimal Radius { get; set; }

        public decimal? HabZoneMin { get; set; }

        public decimal? HabZoneMax { get; set; }

        public List<ExoSystemPlanetsDto> Planets { get; set; }

        public string Message { get; set; }
    }
}