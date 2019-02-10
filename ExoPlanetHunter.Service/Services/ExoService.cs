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

namespace ExoPlanetHunter.Service.Services
{
    public class ExoService : IExoService
    {
        private readonly ExoContext _context;
        private IMemoryCache _cache;

        public ExoService(ExoContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _cache = memoryCache;
        }

        public List<ExoPlanetsDto> GetHabitablePlanets()
        {
            var cacheEntry = CacheExoPlanets();
            return cacheEntry.Where(p => p.Hab == true &&

              p.Star.Color != null && p.Radius != 0
            ).Select(p => new ExoPlanetsDto
            {
                Name = p.Name,
                Coordinate = p.Coordinate,
                Star = p.Star
            }).GroupBy(p => p.Coordinate)
            .Select(g => g.First()).ToList();
        }

        public List<ExoSolarSystemDto> GetSolarSystemPerConstellation(ConstellationsEnum id, int? page = null)
        {
            if (page == null)
            {
                return CacheExoPlanets().Where(p => p.Star.Constellation == (int)id).GroupBy(p => p.Star).Select(g => g.Key).GroupBy(p => p.Name).Select(p => new ExoSolarSystemDto()
                {
                    Name = p.Key,
                    Color = p.FirstOrDefault()?.Color,
                    Radius = p.FirstOrDefault().Radius
                }).ToList();
            }

            return CacheExoPlanets().Where(p => p.Star.Constellation == (int)id).GroupBy(p => p.Star).Select(g => g.Key).GroupBy(p => p.Name).Select(p => new ExoSolarSystemDto()
            {
                Name = p.Key,
                Color = p.FirstOrDefault()?.Color,
                Radius = p.FirstOrDefault().Radius
            }).Skip((int)page * 30).Take(30).ToList();
        }

        public ExoSolarSystemDto GetSolarSystemPerStar(string name)
        {
            var cacheEntry = CacheExoPlanets();
            var planets = cacheEntry.Where(p => p.Star.Name == name).ToList();

            var star = planets.First().Star;
            var solarsystem = new ExoSolarSystemDto()
            {
                Name = star.Name,
                Color = star.Color,
                Luminosity = star.Luminosity,
                HabZoneMax = GetStarDistance(planets, star.HabZoneMax),
                HabZoneMin = GetStarDistance(planets, star.HabZoneMin),
                Radius = star.Radius,

                Planets = planets.OrderBy(p => p.MeanDistance).Select(p => new ExoSystemPlanetsDto()
                {
                    Name = p.Name,
                    Img = p.Img,
                    Radius = p.Radius,
                    Eccentricity = p.Eccentricity,
                    StarDistance = GetStarDistance(planets, p.MeanDistance) + p.Radius
                }).ToList()
            };
            return solarsystem;
        }

        public IEnumerable<ExoPlanetsDto> CacheExoPlanets()
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
                    TempZone = (int)p.ZoneClass.ToEnum<TempEnum>(),
                    Density = p.Density,
                    Period = p.Period,
                    SurfacePressure = p.SurfPress,
                    RadiusEu = p.Radius ?? 0,
                    EscapeVelocity = p.EscVel,
                    Distance = (decimal)3.26156 * (p.Star.Distance ?? 0),
                    Temp = p.TsMean - (decimal)273.15,
                    TempMax = p.TsMax - (decimal)273.15,
                    TempMin = p.TsMin - (decimal)273.15,
                    Coordinate = new CoordinateDto { Latitude = p.Star.Dec, Longitude = -15 * (p.Star.Ra - 12) },
                    DiscYear = p.Disc_Year,
                    Comp = (int)p.CompositionClass.ToEnum<CompEnum>(),
                    HabType = (int)p.HabitableClass.ToEnum<MassEnum>(),
                    MassType = (int)p.MassClass.ToEnum<MassEnum>(),
                    Atmosphere = (int)p.AtmosphereClass.ToEnum<AtmosEnum>(),
                    DiscMethod = (int)p.Disc_Method.ToEnum<DiscEnum>(),
                    Radius = ((15 * p.Radius > 50) ? 50 : (15 * p.Radius < 10 ? 10 : 15 * p.Radius)) ?? 30,
                    MeanDistance = p.MeanDistance,
                    Eccentricity = p.Eccentricity,

                    Star = new ExoStarDto()
                    {
                        Constellation = (int)p.Star.Constellation.Name.ToEnum<ConstellationsEnum>(),
                        Radius = (75 * p.Star.Radius > 100 ? 100 : (75 * p.Star.Radius < 60 ? 60 : 75 * p.Star.Radius)) ?? 75,
                        Color = GetStarColor(p),
                        HabZoneMax = p.Star.HabZoneMax,
                        HabZoneMin = p.Star.HabZoneMin,
                        Name = p.Star.Name,
                        Temp = p.Star.Teff - (decimal)273.15,
                        Age = p.Star.Age,
                        Luminosity = (int)GetStarLuminosity(p),
                        NoHabPlanets = p.Star.NoPlanetsHZ,
                        RadiusSu = p.Star.Radius ?? 0,
                        Mass = p.Star.Mass,
                        NoPlanets = p.Star.NoPlanets,
                        Type = p.Star.Type,
                        Magnitude = p.ApparSize < 7 ? (int)MagnitudeEnum.visible : (int)MagnitudeEnum.unvisible
                    }
                }).ToList();

                var cacheEntryOptions = new MemoryCacheEntryOptions();
                _cache.Set("Exoplanets", cacheExo, cacheEntryOptions);
                _cache.Set("DateUpdated", DateTime.Now, cacheEntryOptions);
            }
            return cacheExo;
        }

        public IQueryable<ExoPlanetsDto> GetExoPlanets(ODataQueryOptions opts)
        {
            var cacheEntry = CacheExoPlanets();
            IQueryable results = opts.ApplyTo(cacheEntry.AsQueryable());
            return (results as IQueryable<ExoPlanetsDto>).Skip(opts.Skip?.Value ?? 0);
        }

        private decimal GetStarDistance(List<ExoPlanetsDto> p, decimal? distance)
        {
            var star = p.First().Star;
            var lastplanet = p.OrderBy(c => c.MeanDistance).Last();
            var habzonemax = star?.HabZoneMax;
            if (habzonemax == null || lastplanet?.MeanDistance > habzonemax)
            {
                return (600 * distance / lastplanet.MeanDistance + 2 * star.Radius) ?? 300;
            }

            return (600 * distance / habzonemax + 2 * star.Radius) ?? 300;
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
            if (type.StartsWith("G") || type.StartsWith("K"))
            {
                return 2;
            }
            if (type.StartsWith("M") || type.StartsWith("L") || type.StartsWith("T"))
            {
                return 3;
            }

            return null;
        }

        public LumEnum GetStarLuminosity(Planet p)
        {
            var type = p.Star?.Type;
            if (type.EndsWith("0"))
            {
                return LumEnum.very_luminous_supergiant;
            }
            if (type.EndsWith("1a"))
            {
                return LumEnum.luminous_supergiant;
            }
            if (type.EndsWith("Ib"))
            {
                return LumEnum.less_luminous_supergiant;
            }
            if (type.EndsWith("II"))
            {
                return LumEnum.luminous_giant;
            }
            if (type.EndsWith("III"))
            {
                return LumEnum.giant;
            }
            if (type.EndsWith("IV"))
            {
                return LumEnum.subgiant;
            }
            if (type.EndsWith("V"))
            {
                return LumEnum.dwarf;
            }
            if (type.EndsWith("VI"))
            {
                return LumEnum.subdwarf;
            }
            if (type.EndsWith("VII"))
            {
                return LumEnum.white_dwarf;
            }
            if (type.Equals("sdB+M"))
            {
                return LumEnum.pulsar;
            }
            if (type.Contains(""))
            {
                return LumEnum.Nodata;
            }
            return LumEnum.Nodata;
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

                    if (p.MassClass.Equals("Terran") || p.MassClass.Equals("Subterran"))
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
                    if (p.MassClass.Equals("Terran") || p.MassClass.Equals("Subterran"))
                    {
                        return "coldstone";
                    }
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
                    if (p.MassClass.Equals("Terran") || p.MassClass.Equals("Subterran"))
                    {
                        return "stone";
                    }
                }
                return "noimg";
            }
        }
    }
}