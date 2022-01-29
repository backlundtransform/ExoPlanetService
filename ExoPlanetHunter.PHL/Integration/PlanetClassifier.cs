using ExoPlanetHunter.Database;
using ExoPlanetHunter.Database.entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ComponentModel;

namespace ExoPlanetHunter.PHL.Integration
{
    public class PlanetClassifier
    {

        private readonly double _tempEarth = 288;

        private readonly string _exoplanetpath = $"D:/Exoplanetdata/PS_2022.01.29_09.22.08.csv";

        public List<Star> DownloadAndClassifieStars()
        {
            var readText = File.ReadAllLines(_exoplanetpath).Select(x => x.Split(",")).ToArray();

            var headers = readText.First();
            var stars = new List<Star>();

            foreach (var values in readText.Skip(1))
            {
                var starName = values[Array.IndexOf(headers, "hostname")];
                var planetName = values[Array.IndexOf(headers, "pl_name")];
                try
                {
                    var ra = values[Array.IndexOf(headers, "ra")].ToNullable<double>();
                    var dec = values[Array.IndexOf(headers, "dec")].ToNullable<double>();

                    if (!stars.Any(p => p.Name == starName))
                    {

                        var type = values[Array.IndexOf(headers, "st_spectype")];
                        var mass = values[Array.IndexOf(headers, "st_mass")].ToNullable<decimal>();

                        var radius = values[Array.IndexOf(headers, "st_rad")].ToNullable<decimal>();
                        var teff = values[Array.IndexOf(headers, "st_teff")].ToNullable<decimal>();
                        var luminosity = values[Array.IndexOf(headers, "st_lum")].ToNullable<decimal>();

                        var age = values[Array.IndexOf(headers, "st_age")].ToNullable<decimal>();
                        var apparMag = values[Array.IndexOf(headers, "sy_vmag")].ToNullable<decimal>();
                        var distance = values[Array.IndexOf(headers, "sy_dist")].ToNullable<decimal>();

                        var (habZoneMin, habZoneMax) = CalculateGoldiLockZone(apparMag, distance);

                        var star = new Star()
                        {
                            Name = starName,
                            Planets = new List<Planet>(),
                            Type = type,
                            Mass = mass,
                            Radius = radius,
                            Teff = teff,
                            Luminosity = luminosity,
                            Age = age,
                            ApparMag = apparMag,
                            Distance = distance,
                            HabZoneMin = habZoneMin,
                            HabZoneMax = habZoneMax,
                            Constellation = new Constellation()
                            {
                                Name = GetConstellation((double)ra, (double)dec)
                            }
                        };

                        var planet = GetPlanet(values, headers, planetName, habZoneMin, habZoneMax);
                        star.Planets.Add(planet);

                        stars.Add(star);
                    }
                }
                catch (Exception)
                {
                    continue;

                }

                if (stars.Any(p => p.Name == starName))
                {

                    var oldstar = stars.First(p => p.Name == starName);

                    if (!oldstar.Planets.Any(p => p.Name == planetName))
                    {

                        try
                        {

                            var planet = GetPlanet(values, headers, planetName, (decimal)oldstar.HabZoneMin, (decimal)oldstar.HabZoneMax);

                            oldstar.Planets.Add(planet);
                        }
                        catch (Exception)
                        {
                            continue;

                        }

                    }

                }
            }

            return stars;

        }

        public Planet GetPlanet(string[] values, string[] headers, string planetName, decimal habZoneMin, decimal habZoneMax)
        {
            var teq = values[Array.IndexOf(headers, "pl_eqt")].ToNullable<decimal>();
            var teqmin = values[Array.IndexOf(headers, "pl_eqterr2")].ToNullable<decimal>();
            var teqmax = values[Array.IndexOf(headers, "pl_eqterr1")].ToNullable<decimal>();
            var mass = values[Array.IndexOf(headers, "pl_masse")].ToNullable<decimal>();
            var radius = values[Array.IndexOf(headers, "pl_rade")].ToNullable<decimal>();
            var mindist = values[Array.IndexOf(headers, "pl_orbsmaxerr2")].ToNullable<decimal>();
            var maxdist = values[Array.IndexOf(headers, "pl_orbsmaxerr1")].ToNullable<decimal>();
            var meandist = (maxdist + mindist) / 2;
            return new Planet()
            {
                Name = planetName,
                ZoneClass = CalculateZoneClass(teq),
                TeqMax = teqmax,
                TeqMean = teq,
                TeqMin = teqmin,
                TsMean = teq,
                TsMin = teqmin,
                TsMax = teqmax,
                Mass = mass,
                MassClass = CalculateMassClass(mass),
                Radius = values[Array.IndexOf(headers, "pl_rade")].ToNullable<decimal>(),
                Density = values[Array.IndexOf(headers, "pl_dens")].ToNullable<decimal>(),
                Period = values[Array.IndexOf(headers, "pl_orbper")].ToNullable<decimal>(),
                Eccentricity = values[Array.IndexOf(headers, "pl_orbeccen")].ToNullable<decimal>(),
                SemMajorAxis = values[Array.IndexOf(headers, "pl_orbsmax")].ToNullable<decimal>(),
                MeanDistance = meandist,
                Habitable = meandist >= habZoneMin && meandist <= habZoneMax,
                Esi = (decimal)CalculateEsi(radius,teq),
                Disc_Method = values[Array.IndexOf(headers, "discoverymethod")].Replace(" ", string.Empty),
                Disc_Year = values[Array.IndexOf(headers, "disc_year")].ConvertYearIntToNullable(),
            };

        }

        public void AddClassifiedStars(List<Star> stars)
        {
            using (var context = new ExoContext())
            {

                var oldstars = context.Stars.AsQueryable();

                foreach (var star in stars)
                {

                    if (!oldstars.Any(p => p.Name == star.Name))
                    {

                        var constellation = context.Constellations.AsQueryable().FirstOrDefault(c => c.Name == star.Constellation.Name);

                        if (constellation != null)
                        {

                            star.Constellation = constellation;
                        }

                        context.Stars.Add(star);

                    }

                    if (oldstars.Any(p => p.Name == star.Name))
                    {

                        var oldstar = oldstars.First(p => p.Name == star.Name);

                        foreach (var planet in oldstar.Planets)
                        {
                            if (!oldstar.Planets.Any(p => p.Name == planet.Name))
                            {

                                context.Planets.Add(planet);

                            }
                        }
                    }
                }

                context.SaveChanges();
            }
        }
        public (decimal habZoneMin, decimal habZoneMax) CalculateGoldiLockZone(decimal? magnitude, decimal? starDistance)
        {
            if(magnitude ==null || starDistance == null)
            {

                return (0, 0);
            }

            var mv = (double)magnitude - (5 * Math.Log10((double)starDistance / 10));
            var mbol = mv - 0.4;
            var lStar = Math.Pow(10, (mbol - 4.72) / -2.5);
            return ((decimal)Math.Sqrt(lStar / 1.1), (decimal)Math.Sqrt(lStar / 0.21));

        }

        public double CalculateEsi(decimal? radius, decimal? temp)
        {

            if (radius == null || temp == null)
            {

                return 0;
            }


            var esi = 1 - Math.Sqrt(0.5 * (Math.Pow(((double)temp - _tempEarth) / ((double)temp + _tempEarth), 2) + Math.Pow(((double)radius - 1) / (double)(radius + 1), 2)));

            return esi;

        }

        public string GetConstellation(double ra, double dec)
        {
            string json = File.ReadAllText(@"constellation.json");

            var table = JsonConvert.DeserializeObject<dynamic[][]>(json);
            var convh = Math.PI / 180.0;
            var convd = Math.PI / 180.0;
            ra *= convh;
            dec *= convd;
            var newcoords = Precess(ra, dec, DateTime.Now.Year, 1875.0);
            ra = newcoords.Item1;
            dec = newcoords.Item2;
            ra /= convh;
            dec /= convd;
            for (var i = 0; i < table.Count(); i++)
            {
                if (dec < (double)table[i][2] || ra / 15 < (double)table[i][0] || ra / 15 >= (double)table[i][1])
                {

                    continue;
                }

                return (string)table[i][3];
            }
            return "";
        }

        private (double, double) Precess(double ra1, double dec1, double epoch1, double epoch2)
        {
            var cdr = Math.PI / 180.0;
            var csr = cdr / 3600.0;
            var a = Math.Cos(dec1);
            var x1 = new[] {
        a * Math.Cos(ra1), a * Math.Sin(ra1), Math.Sin(dec1)
      };
            var t = 0.001 * (epoch2 - epoch1);
            var st = 0.001 * (epoch1 - 1900.0);
            a = csr * t * (23042.53 + st * (139.75 + 0.06 * st) + t * (30.23 - 0.27 * st + 18.0 * t));
            var b = csr * t * t * (79.27 + 0.66 * st + 0.32 * t) + a;
            var c = csr * t * (20046.85 - st * (85.33 + 0.37 * st) + t * (-42.67 - 0.37 * st - 41.8 * t));
            var sina = Math.Sin(a);
            var sinb = Math.Sin(b);
            var sinc = Math.Sin(c);
            var cosa = Math.Cos(a);
            var cosb = Math.Cos(b);
            var cosc = Math.Cos(c);
            var r = new double[][] {
        new [] {
          0.0,
          0.0,
          0.0
        }, new [] {
          0.0,
          0.0,
          0.0
        }, new [] {
          0.0,
          0.0,
          0.0
        }
      };
            r[0][0] = cosa * cosb * cosc - sina * sinb;
            r[0][1] = -cosa * sinb - sina * cosb * cosc;
            r[0][2] = -cosb * sinc;
            r[1][0] = sina * cosb + cosa * sinb * cosc;
            r[1][1] = cosa * cosb - sina * sinb * cosc;
            r[1][2] = -sinb * sinc;
            r[2][0] = cosa * sinc;
            r[2][1] = -sina * sinc;
            r[2][2] = cosc;
            var x2 = new[] {
        0.0,
        0.0,
        0.0
      };
            for (var i = 0; i < 3; i++)
                x2[i] = r[i][0] * x1[0] + r[i][1] * x1[1] + r[i][2] * x1[2];
            var ra2 = Math.Atan2(x2[1], x2[0]);
            if (ra2 < 0.0)
                ra2 += 2.0 * Math.PI;
            var dec2 = Math.Asin(x2[2]);
            return (ra2, dec2);
        }

        public string CalculateZoneClass(decimal? temp)
        {

            if (temp >= (decimal)273.15 && temp <= (decimal)323.15)
            {
                return "Warm";

            }

            if (temp > (decimal)323.15)
            {
                return "Hot";

            }

            if (temp < (decimal)273.15)
            {
                return "Cold";

            }
            return "";
        }

        public string CalculateMassClass(decimal? mass)
        {

            if (mass <= (decimal)0.1)
            {
                return "Mercurian";

            }

            if (mass <= (decimal)0.5 && mass > (decimal)0.1)
            {
                return "Subterran";

            }

            if (mass <= 2 && mass > (decimal)0.5)
            {
                return "Terran";

            }

            if (mass <= 10 && mass > 2)
            {
                return "Superterran";

            }

            if (mass <= 50 && mass > 10)
            {
                return "Neptunian";

            }

            if (mass > 50)
            {
                return "Jovian";

            }

            return "";
        }

    }
}