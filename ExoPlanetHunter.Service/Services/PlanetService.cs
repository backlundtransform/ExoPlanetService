﻿using AutoMapper.QueryableExtensions;
using ExoPlanetHunter.Database;
using ExoPlanetHunter.Database.entity;
using ExoPlanetHunter.Service.Dto;
using ExoPlanetHunter.Service.Enum;
using ExoPlanetHunter.Service.Interfaces;
using Microsoft.AspNet.OData.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExoPlanetHunter.Service.Services
{
    public class PlanetService : IPlanetService
    {
        private readonly ExoContext _context;
        private IMemoryCache _cache;

        public PlanetService(ExoContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _cache = memoryCache;
        }

        public IQueryable<PlanetDto> GetPlanets(ODataQueryOptions opts)
        {
            IQueryable results = opts.ApplyTo(_context.Planets.ProjectTo<PlanetDto>().AsQueryable());
            return results as IQueryable<PlanetDto>;
        }

        public List<ExoPlanetsDto> CacheExoPlanets()
        {
            List<ExoPlanetsDto> cacheExo;
            if (!_cache.TryGetValue("Exoplanets", out cacheExo))
            {
                cacheExo = _context.Planets.OrderByDescending(p => p.Disc_Year).Include(z => z.Star).Include(z => z.Star.Planets).Select(p => new ExoPlanetsDto
                {
                    Name = p.Name,
                    Img = new ImgDto() { Uri = GetPlanetColor(p) },
                    Type = p.MassClass,
                    Esi = p.Esi,
                    Sph = p.Sph,
                    Hza = p.Hza,
                    Hab = p.Habitable,
                    Gravity = p.Gravity,
                    Moons = p.HabMoon,

                    Density = p.Density,
                    Period = p.Period,
                    SurfacePressure = p.SurfPress,

                    EscapeVelocity = p.EscVel,
                    Distance = (decimal)3.26156 * (p.Star.Distance ?? 0),
                    Temp = p.TsMean - (decimal)273.15,
                    TempMax = p.TsMax - (decimal)273.15,
                    TempMin = p.TsMin - (decimal)273.15,
                    Coordinate = new CoordinateDto { Latitude = p.Star.Dec, Longitude = 15 * (p.Star.Ra - 12) },
                    DiscYear = p.Disc_Year,
                    Comp = p.CompositionClass.ToEnum<CompEnum>(),
                    MassType = p.MassClass.ToEnum<MassEnum>(),
                    Atmosphere = p.AtmosphereClass.ToEnum<AtmosEnum>(),
                    DiscMethod = p.Disc_Method.ToEnum<DiscEnum>(),
                    Radius = ((15 * p.Radius > 50) ? 50 : (15 * p.Radius < 10 ? 10 : 15 * p.Radius)) ?? 30,
                    MeanDistance = p.MeanDistance,
                    StarDistance = GetStarDistance(p, p.MeanDistance),
                    Star = new ExoStarDto()
                    {
                        Constellation = p.Star.Constellation.Name.ToEnum<ConstellationsEnum>(),
                        Radius = (75 * p.Star.Radius > 100 ? 100 : (75 * p.Star.Radius < 50 ? 50 : 75 * p.Star.Radius)) ?? 75,
                        Color = GetStarColor(p),
                        HabZoneMax = GetStarDistance(p, p.Star.HabZoneMax),
                        HabZoneMin = GetStarDistance(p, p.Star.HabZoneMin),
                        Name = p.Star.Name,
                        Temp = p.Star.Teff - (decimal)273.15,
                        Age = p.Star.Age,
                        Luminosity = p.Star.Luminosity,
                        NoHabPlanets = p.Star.NoPlanetsHZ,
                        Mass = p.Star.Mass,
                        NoPlanets = p.Star.NoPlanets,
                        Type = p.Star.Type,
                        Magnitude =p.ApparSize<7?MagnitudeEnum.visible:MagnitudeEnum.unvisible
                    }
                }).ToList();

                var cacheEntryOptions = new MemoryCacheEntryOptions();
                _cache.Set("Exoplanets", cacheExo, cacheEntryOptions);
            }
            return cacheExo;
        }

        public IQueryable<ExoPlanetsDto> GetExoPlanets(ODataQueryOptions opts)
        {
            var cacheEntry = CacheExoPlanets();
            IQueryable results = opts.ApplyTo(cacheEntry.AsQueryable());
            return (results as IQueryable<ExoPlanetsDto>).Skip(opts.Skip?.Value ?? 0);
        }

        private decimal? GetStarDistance(Planet p, decimal? distance)
        {
            var lastplanet = p.Star?.Planets.OrderByDescending(c => c.MeanDistance).Last();
            var habzonemax = p.Star?.HabZoneMax;
            if (lastplanet?.MeanDistance > habzonemax)
            {
                return 600 * distance / lastplanet.MeanDistance;
            }

            return 600 * distance / habzonemax;
        }

        private int? GetStarColor(Planet p)
        {
            var type = p.Star?.Type;

            if (type == null)
            {
                return null;
            }
            if (type.StartsWith("O") || type.StartsWith("B") || type.StartsWith("A"))
            {
                return 0;
            }

            if (type.StartsWith("F"))
            {
                return 1;
            }
            if (type.StartsWith("G"))
            {
                return 2;
            }
            if (type.StartsWith("K") || type.StartsWith("M"))
            {
                return 3;
            }

            return null;
        }

        private string GetPlanetColor(Planet p)
        {
            if (p.MassClass.Equals("Neptunian"))
            {
                return "neptunian";
            }
            if (p.AtmosphereClass.Equals("no-atmosphere"))
            {
                if (p.MassClass.Equals("Jovian"))
                {
                    return "jovian";
                }
                return "iron";
            }
            else
            {
                if (p.ZoneClass.Equals("Hot"))
                {
                    if (p.MassClass.Equals("Superterran"))
                    {
                        return "hotsuperearth";
                    }

                    if (p.MassClass.Equals("Jovian"))
                    {
                        return "hotjupiter";
                    }

                    if (p.MassClass.Equals("Terran") || p.MassClass.Equals("SubTerran"))
                    {
                        return "hotstone";
                    }
                }

                if (p.ZoneClass.Equals("Cold"))
                {
                    if (p.MassClass.Equals("Superterran"))
                    {
                        return "coldsuperearth";
                    }

                    if (p.MassClass.Equals("Jovian"))
                    {
                        return "jovian";
                    }
                    if (p.MassClass.Equals("Terran") || p.MassClass.Equals("SubTerran"))
                    {
                        return "coldstone";
                    }
                    else
                    {
                        if (p.MassClass.Equals("Superterran"))
                        {
                            return "superearth";
                        }

                        if (p.MassClass.Equals("Jovian"))
                        {
                            return "jovian";
                        }
                        if (p.MassClass.Equals("Terran") || p.MassClass.Equals("SubTerran"))
                        {
                            return "stone";
                        }
                    }
                }
                return "noimg";
            }
        }

        public async Task<PlanetDto> GetPlanet(int id)
        {
            var planet = await _context.Planets.ProjectTo<PlanetDto>().SingleOrDefaultAsync(m => m.Id == id);
            return planet;
        }
    }
}