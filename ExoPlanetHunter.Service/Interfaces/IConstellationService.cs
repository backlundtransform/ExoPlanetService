using ExoPlanetHunter.Service.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExoPlanetHunter.Service.Interfaces
{
    public interface IConstellationService
    {
        IEnumerable<ConstellationDto> GetConstellations();

        Task<ConstellationDto> GetConstellation(int cid);

        Task<ConstellationStarsDto> GetConstellationWithStars(int cid);

    }
}