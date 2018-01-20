
using ExoPlanetHunter.Database;
using ExoPlanetHunter.Pocos;
using FluentScheduler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ExoPlanetHunter.PHL
{
    public class MyJob : IJob
    {
        private string exoplaneturl =Phl.Configuration.GetSection("Exourl").Value.ToString();

        private List<Constellation> _constellations = Phl.Configuration.GetSection("Constellations").Value.ToString().Split(new char[] { ';' }).Select(p => new Constellation { Name = p }).ToList();

        public void Execute()
        {
            ExecuteAsync().Wait();
        }

        private async Task ExecuteAsync()
        {
            await Task.Run(() => DownloadExoData());
        }

        private void DownloadExoData()
        {
            List<Star> Starlist = new List<Star>();

            using (var client = new WebClient())
            {
                var content = client.DownloadData(exoplaneturl);
                using (var stream = new MemoryStream(content))
                {
                    using (ZipArchive archive = new ZipArchive(stream))
                    {
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            using (var reader = new StreamReader(entry.Open()))
                            {
                                while (!reader.EndOfStream)
                                {
                                    var line = reader.ReadLine();
                                    var values = line.Split(',');
                                    var starname = values[33];
                                    if (starname != "S. Name")
                                    {
                                        if (!Starlist.Any(o => o.Name == starname))
                                        {
                                            Starlist.Add(GetStar(starname, values));
                                        }
                                        var star = Starlist.First(o => o.Name == starname);
                                        star.Planets.Add(GetPlanet(values, star));
                                    }
                                }
                            };
                        }
                    }
                }
            }

            using (var context = new ExoContext())
            {
                var starsToDelete = context.Set<Star>();

                context.Stars.RemoveRange(starsToDelete);
                context.Stars.AddRange(Starlist);

                context.SaveChanges();
            }
        }

        private Star GetStar(string starname, string[] values)
        {
            return new Star()
            {
                Name = starname,

                NameHD = values[34],
                NameHIP = values[35],

                Constellation = _constellations.FirstOrDefault(p => p.Name == values[36]),
                Type = values[37],
                Mass = values[38].ToNullable<decimal>(),
                Radius = values[39].ToNullable<decimal>(),
                Teff = values[40].ToNullable<decimal>(),
                Luminosity = values[41].ToNullable<decimal>(),
                FeH = values[42].ToNullable<decimal>(),
                Age = values[43].ToNullable<decimal>(),
                ApparMag = values[44].ToNullable<decimal>(),
                Distance = values[45].ToNullable<decimal>(),
                Ra = values[46].ToNullable<decimal>(),
                Dec = values[47].ToNullable<decimal>(),
                MagfromPlanet = values[48].ToNullable<decimal>(),

                SizefromPlanet = values[49].ToNullable<decimal>(),

                NoPlanets = values[50].ToNullable<int>(),

                NoPlanetsHZ = values[51].ToNullable<int>(),
                HabZoneMin = values[52].ToNullable<decimal>(),

                HabZoneMax = values[53].ToNullable<decimal>(),

                HabCat = values[62].ConvertToBoolToNullable(),
                Planets = new List<Planet>() { }
            };
        }

        private Planet GetPlanet(string[] values, Star star)
        {
            return new Planet()
            {
                Name = values[0],

                NameKepler = values[1],
                NameKOI = values[2],
                ZoneClass = values[3],

                MassClass = values[4],

                CompositionClass = values[5],
                AtmosphereClass = values[6],
                HabitableClass = values[7],

                MinMass = values[8].ToNullable<decimal>(),
                Mass = values[9].ToNullable<decimal>(),
                MaxMass = values[10].ToNullable<decimal>(),
                Radius = values[11].ToNullable<decimal>(),
                Density = values[12].ToNullable<decimal>(),
                Gravity = values[13].ToNullable<decimal>(),
                EscVel = values[14].ToNullable<decimal>(),
                SFluxMin = values[15].ToNullable<decimal>(),
                SFluxMean = values[16].ToNullable<decimal>(),
                SFluxMax = values[17].ToNullable<decimal>(),
                TeqMin = values[18].ToNullable<decimal>(),
                TeqMean = values[19].ToNullable<decimal>(),
                TeqMax = values[20].ToNullable<decimal>(),
                TsMin = values[21].ToNullable<decimal>(),
                TsMean = values[22].ToNullable<decimal>(),
                TsMax = values[23].ToNullable<decimal>(),
                SurfPress = values[24].ToNullable<decimal>(),
                Mag = values[25].ToNullable<decimal>(),
                ApparSize = values[26].ToNullable<decimal>(),
                Period = values[27].ToNullable<decimal>(),
                SemMajorAxis = values[28].ToNullable<decimal>(),
                Eccentricity = values[29].ToNullable<decimal>(),
                MeanDistance = values[30].ToNullable<decimal>(),
                Inclination = values[31].ToNullable<decimal>(),
                Omega = values[32].ToNullable<decimal>(),

                Hzd = values[54].ToNullable<decimal>(),
                Hzc = values[55].ToNullable<decimal>(),
                Hza = values[56].ToNullable<decimal>(),
                Hzi = values[57].ToNullable<decimal>(),
                Sph = values[58].ToNullable<decimal>(),

                IntEsi = values[59].ToNullable<decimal>(),
                SurfEsi = values[60].ToNullable<decimal>(),
                Esi = values[61].ToNullable<decimal>(),

                Habitable = values[63].ConvertToBoolToNullable(),

                HabMoon = values[64].ConvertToBoolToNullable(),

                Confirmed = values[65].ConvertToBoolToNullable(),

                Disc_Method = values[66],
                Disc_Year = (!string.IsNullOrEmpty(values[67])) ? Convert.ToInt32(values[67].Split(",")[0]) : (int?)null,

                Star = star
            };
        }
    }
}