using ExoPlanetHunter.Database;
using ExoPlanetHunter.Database.entity;
using ExoPlanetHunter.Service.Dto;
using ExoPlanetHunter.Service.Enum;
using ExoPlanetHunter.Service.Interfaces;
using LiteDB;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ExoPlanetHunter.Service.Services
{
    public class ExoService : IExoService
    {
        private readonly ExoContext _context;
     
        private IHostingEnvironment _env;

        public ExoService(ExoContext context, IHostingEnvironment env)
        {
            _context = context;
         
            _env = env;
        }

        public List<ExoPlanetsDto> GetHabitablePlanets()
        {

            using (var db = new LiteDatabase($@"{_env.ContentRootPath}\nosqlexo.db"))
            {
                var col = db.GetCollection<ExoPlanetsDto>("exoplanet");

                return col.Find(Query.And(Query.EQ("Hab", true), Query.Not("Name", "Tellus"))).Select(p => new ExoPlanetsDto
                {
                    Name = p.Name,
                    Coordinate = p.Coordinate,
                    Star = p.Star
                }).GroupBy(p => p.Coordinate)
            .Select(g => g.First()).ToList(); 
            }
             
        }
        public List<ExoPlanetsDto> GetAllPlanets()
        {
            using (var db = new LiteDatabase($@"{_env.ContentRootPath}\nosqlexo.db"))
            {
                var col = db.GetCollection<ExoPlanetsDto>("exoplanet");

                return col.FindAll().Select(p => new ExoPlanetsDto
                {
                    Hab = p.Hab,
                    Name = p.Name,
                    Coordinate = p.Coordinate,
                    Star = p.Star
                }).GroupBy(p => p.Coordinate)
            .Select(g => new ExoPlanetsDto()
            {
                Hab = g.Any(c => c.Hab == true),
                Coordinate = g.First().Coordinate,
                Star = g.First().Star

            }).ToList();
            }

        }

        public List<ExoSolarSystemDto> GetSolarSystemPerConstellation(ConstellationsEnum id, int? page = null)
        {
            using (var db = new LiteDatabase($@"{_env.ContentRootPath}\nosqlexo.db"))
            {
                var col = db.GetCollection<ExoPlanetsDto>("exoplanet");
              
            if (page == null)
                {
                    return col.Find(p => p.Star.Constellation == (int)id).GroupBy(p => p.Star).Select(g => g.Key).GroupBy(p => p.Name).Select(p => new ExoSolarSystemDto()
                    {
                        Name = p.Key,
                        Color = p.FirstOrDefault()?.Color,
                        Radius = p.FirstOrDefault().Radius
                    }).ToList();
                }

                return col.Find(p => p.Star.Constellation == (int)id).GroupBy(p => p.Star).Select(g => g.Key).GroupBy(p => p.Name).Select(p => new ExoSolarSystemDto()
                {
                    Name = p.Key,
                    Color = p.FirstOrDefault()?.Color,
                    Radius = p.FirstOrDefault().Radius
                }).Skip((int)page * 30).Take(30).ToList();
            }
        }

        public ExoSolarSystemDto GetSolarSystemPerStar(string name)
        {
            using (var db = new LiteDatabase($@"{_env.ContentRootPath}\nosqlexo.db"))
            {
                var col = db.GetCollection<ExoPlanetsDto>("exoplanet");
                var planets =col.Find(p => p.Star.Name == name).ToList();

                var star = planets.First().Star;
                var solarsystem = new ExoSolarSystemDto()
                {
                    Name = star.Name,
                    Color = star.Color,
                    Luminosity = star.Luminosity,
                    HabZoneMax = GetStarDistance(planets, star.HabZoneMax),
                    HabZoneMin = GetStarDistance(planets, star.HabZoneMin),
                    Radius = star.Radius,
                    Coordinate = planets.First().Coordinate,
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
              
         
        }

        public IEnumerable<ExoPlanetsDto> CacheExoPlanets()
        {
           
            using (var db = new LiteDatabase($@"{_env.ContentRootPath}\nosqlexo.db"))
            {
                var col = db.GetCollection<ExoPlanetsDto>("exoplanet");
                if (col.Count() == 0)
                {
                    var planets = _context.Planets.OrderByDescending(p => p.Disc_Year).Include(z => z.Star).Include(z => z.Star.Constellation).Include(z => z.Star.Planets);


                    foreach (var p in planets)
                    {

                        var planet = new ExoPlanetsDto();
                        planet.Id = p.Id;
                        planet.Name = p.Name;
                        planet.Img = new ImgDto() { Uri = GetPlanetColor(p) };
                        planet.Type = p.MassClass;
                        planet.Esi = p.Esi;
                        planet.Hab = p.Habitable;
                        planet.Gravity = p.Gravity;
                        planet.TempZone = (int)p.ZoneClass.ToEnum<TempEnum>();
                        planet.Density = p.Density;
                        planet.Period = p.Period;
                        planet.SurfacePressure = p.SurfPress;
                        planet.RadiusEu = p.Radius ?? 0;
                        planet.EscapeVelocity = p.EscVel;
                        planet.Distance = (decimal)3.26156 * (p.Star.Distance ?? 0);
                        planet.Temp = p.TsMean - (decimal)273.15;
                        planet.TempMax = p.TsMax - (decimal)273.15;
                        planet.TempMin = p.TsMin - (decimal)273.15;
                        planet.Coordinate = new CoordinateDto { Latitude = p.Star.Dec, Longitude = -15 * (p.Star.Ra - 12) };
                        planet.DiscYear = p.Disc_Year;
                        planet.Comp = (int)CompEnum.Nodata;
                        planet.Mass = p.Mass;
                        planet.HabType = (int)p.HabitableClass.ToEnum<HabEnum>();
                        planet.MassType = (int)p.MassClass.ToEnum<MassEnum>();

                        planet.DiscMethod = (int)p.Disc_Method.ToEnum<DiscEnum>();
                        planet.Radius = ((15 * p.Radius > 50) ? 50 : (15 * p.Radius < 10 ? 10 : 15 * p.Radius)) ?? 30;
                        planet.MeanDistance = p.MeanDistance;
                        planet.Eccentricity = p.Eccentricity;

                        planet.Star = new ExoStarDto()
                        {
                            Constellation = p.Star.Constellation == null ? (int)ConstellationsEnum.Nodata : (int)p.Star.Constellation.Name.ToEnum<ConstellationsEnum>(),
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
                            NoPlanets = p.Star.Planets.Count,
                            Type = p.Star.Type,
                            Magnitude = p.Star.ApparMag < 7 ? (int)MagnitudeEnum.visible : (int)MagnitudeEnum.unvisible
                        };
                        col.Insert(planet);
                        col.EnsureIndex(x => x.Name);

                    }
                }
           
                return col.FindAll().ToList();
            }

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

        public IEnumerable<ExoPlanetsDto> GetPaginatedPlanets(int page, bool hab, bool moon, string type, ChartType key, string name)
        {

            using (var db = new LiteDatabase($@"{_env.ContentRootPath}\nosqlexo.db"))
            {
                var col = db.GetCollection<ExoPlanetsDto>("exoplanet");
                if (!string.IsNullOrEmpty(name) && hab)
                {
                    return col.Find(p => p.Name.ToLower().Contains(name.ToLower()) && p.Hab == hab).OrderByDescending(p => p.DiscYear).Skip(page * 30).Take(30);
                }

                if (!string.IsNullOrEmpty(name) && moon)
                {
                    return col.Find(p => p.Name.ToLower().Contains(name.ToLower()) && p.Moons == moon).OrderByDescending(p => p.DiscYear).Skip(page * 30).Take(30);
                }

                if (!string.IsNullOrEmpty(name))
                {
                    return col.Find(p => p.Name.ToLower().Contains(name.ToLower())).OrderByDescending(p => p.DiscYear).Skip(page * 30).Take(30);
                }

                if (hab)
                {
                    return col.Find(p => p.Hab == hab).OrderByDescending(p => p.DiscYear).Skip(page * 30).Take(30);
                }
                if (moon)
                {
                    return col.Find(p => p.Moons == moon).OrderByDescending(p => p.DiscYear).Skip(page * 30).Take(30);
                }

                if (!string.IsNullOrEmpty(type))
                {
                    switch (key)
                    {
                        case ChartType.Mass: return col.Find(p => p.MassType == (int)type.ToEnum<MassEnum>()).OrderByDescending(p => p.DiscYear).Skip(page * 30).Take(30);
                        case ChartType.DiscoveryMetod: return col.Find(p => p.DiscMethod == (int)type.ToEnum<DiscEnum>()).OrderByDescending(p => p.DiscYear).Skip(page * 30).Take(30);
                        case ChartType.Temperature: return col.Find(p => p.TempZone == (int)type.ToEnum<TempEnum>()).OrderByDescending(p => p.DiscYear).Skip(page * 30).Take(30);
                    }
                }

                return col.FindAll().OrderByDescending(p => p.DiscYear).Skip(page * 30).Take(30); 

            }

          
        }
    }
}