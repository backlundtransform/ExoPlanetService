
using System.Collections.Generic;

namespace ExoPlanetHunter.Service.Dto
{
    public class StarPlanetsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<PlanetDto> Planets { get; set; }
        public string Message { get; set; }
    }
}