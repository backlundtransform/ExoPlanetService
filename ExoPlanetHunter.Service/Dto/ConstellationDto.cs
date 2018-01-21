using ExoPlanetHunter.Pocos;
using System.Linq;


namespace ExoPlanetHunter.Service.Dto
{
    public class ConstellationDto
    {
        public ConstellationDto(Constellation constellation)
        {
            Id = constellation.Id;
            Name = constellation.Name;
            Stars = constellation.Stars?.ToList()?.Count();
        }
        public int Id { get; set; }
        public string Name { get; set; }

        public int? Stars  { get; set; }
    }
}
