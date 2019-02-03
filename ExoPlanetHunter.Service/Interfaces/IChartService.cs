using ExoPlanetHunter.Database.entity;
using ExoPlanetHunter.Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExoPlanetHunter.Service.Interfaces
{
    public interface IChartService
    {
        IQueryable<HertzsprungRussellDto> GetHertzsprungRussell();
      IQueryable<IGrouping<string, Planet>> GetPlanetTypes();
    }
}
