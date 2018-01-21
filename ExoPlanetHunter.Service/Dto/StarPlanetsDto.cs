using ExoPlanetHunter.Pocos;
using System.Collections.Generic;

namespace ExoPlanetHunter.Service.Dto
{
    public class StarPlanetsDto
    {
        public StarPlanetsDto(List<Planet> planets, int id)
        {
            StarId = id;
            Planets = planets;
        }

        public int StarId { get; set; }

        public List<Planet> Planets { get; set; }
    }
}