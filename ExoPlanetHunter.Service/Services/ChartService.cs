using ExoPlanetHunter.Database;
using ExoPlanetHunter.Database.entity;
using ExoPlanetHunter.Service.Dto;
using ExoPlanetHunter.Service.Enum;
using ExoPlanetHunter.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExoPlanetHunter.Service.Services
{
    public class ChartService : IChartService
    {
        private readonly ExoContext _context;

        public ChartService(ExoContext context)
        {
            _context = context;
        }

        public IQueryable<HertzsprungRussellDto> GetHertzsprungRussell(bool habitableOnly)
        {
            return _context.Stars.AsQueryable()
                .Where(p => p.Mass != null && p.Luminosity != null
            && p.Teff != null && p.Type != null && habitableOnly?p.Planets.Any(o=>o.Habitable==true):true)
            .Select(HertzsprungRussellDto.FromEntities).Where(p => p.Color != null);
        }

        public IQueryable<PlanetDistanceDto> GetPlanetDistance(double? max=null)
        {
            if (max != null)
            {
                return _context.Planets.AsQueryable().Where(p => p.Habitable == true && 3.26156 * (double)p.Star.Distance < max).Select(PlanetDistanceDto.FromEntities);

            }
            return _context.Planets.AsQueryable().Where(p => p.Habitable == true).Select(PlanetDistanceDto.FromEntities);
        }

        public IQueryable<IGrouping<string, Planet>> GetPlanetTypes(ChartType type)
        {
                switch (type) {
                case ChartType.Mass:  return _context.Planets.AsQueryable().Where(p => p.MassClass != "" && p.MassClass.ToEnum<MassEnum>() != MassEnum.Nodata).GroupBy(p => p.MassClass);
                case ChartType.Atmospere: return _context.Planets.AsQueryable().Where(p => p.AtmosphereClass!= "" && p.AtmosphereClass.ToEnum<AtmosEnum>() != AtmosEnum.Nodata).GroupBy(p => p.AtmosphereClass);
                case ChartType.DiscoveryMetod: return _context.Planets.AsQueryable().Where(p => p.Disc_Method != ""&& p.Disc_Method.ToEnum<DiscEnum>() != DiscEnum.Nodata).GroupBy(p => p.Disc_Method);
                case ChartType.DiscoveryYear: return _context.Planets.AsQueryable().Where(p => p.Disc_Year != null).GroupBy(p => p.Disc_Year.ToString());
                case ChartType.Hability: return _context.Planets.AsQueryable().Where(p => p.HabitableClass != "" && p.HabitableClass.ToEnum<HabEnum>() != HabEnum.Nodata).GroupBy(p => p.HabitableClass);
                case ChartType.Temperature: return _context.Planets.AsQueryable().Where(p => p.ZoneClass != "" && p.ZoneClass.ToEnum<TempEnum>() != TempEnum.Nodata).GroupBy(p => p.ZoneClass);
                case ChartType.Composition: return _context.Planets.AsQueryable().Where(p => p.CompositionClass != "" && p.CompositionClass.ToEnum<CompEnum>()!= CompEnum.Nodata).GroupBy(p => p.CompositionClass);
                default: return _context.Planets.GroupBy(p => p.Name);
            }
          
        }
    }
}