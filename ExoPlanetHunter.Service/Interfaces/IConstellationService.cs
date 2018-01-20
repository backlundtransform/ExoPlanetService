using ExoPlanetHunter.Pocos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExoPlanetHunter.Service.Interfaces
{
    public interface IConstellationService
    {
        IEnumerable<Constellation> GetConstellations();

        Task<Constellation> GetConstellation(int cid);

        IEnumerable<Star> GetStarsByConstellation(int cid);
    }
}