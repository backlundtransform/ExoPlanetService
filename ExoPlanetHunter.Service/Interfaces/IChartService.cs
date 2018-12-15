using ExoPlanetHunter.Service.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExoPlanetHunter.Service.Interfaces
{
    public interface IChartService
    {
      List<HertzsprungRussellDto> GetHertzsprungRussell();
    }
}
