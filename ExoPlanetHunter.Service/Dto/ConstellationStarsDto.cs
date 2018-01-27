
using System.Collections.Generic;

namespace ExoPlanetHunter.Service.Dto
{
    public class ConstellationStarsDto
    {

        public int Id { get; set; }
        public string Name { get; set; }

        public List<StarDto> Stars { get; set; }
        public string Message { get; set; }
    }
}
