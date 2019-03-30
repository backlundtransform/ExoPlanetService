using ExoPlanetHunter.Database.entity;
using ExoPlanetHunter.Service.Dto;
using ExoPlanetHunter.Service.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExoPlanetHunter.Service.Interfaces
{
    public interface IChartService
    {
        IQueryable<HertzsprungRussellDto> GetHertzsprungRussell(bool habitableOnly);
        IQueryable<IGrouping<string, Planet>> GetPlanetTypes(ChartType type);
        IQueryable<PlanetDistanceDto> GetPlanetDistance();
    }
}
