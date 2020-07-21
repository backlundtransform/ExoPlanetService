using ExoPlanetHunter.Service.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExoPlanetHunter.Service.Interfaces
{
  public interface IMastService
    {
        Task<List<TransitTimeserie>> GetMast(double ra, double dec, double radius);
    }
}
