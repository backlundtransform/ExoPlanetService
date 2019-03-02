using ExoPlanetHunter.Database;
using ExoPlanetHunter.Database.entity;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;

namespace ExoPlanetHunter.PHL.Integration
{
    public class PhlService : IPhlService
    {
        private string exoplaneturl = $"http://www.hpcf.upr.edu/~abel/phl/phl_hec_all_confirmed.csv.zip";
        private List<Constellation> _constellations = $"And;Ant;Aqr;Apu;Aps;Aql;Ari;Ara;Aur;Boo;Cae;Cam;Cnc;CVn;CMa;CMi;Cap;Car;Cas;Cen;Cep;Cet;Cha;Cir;Col;Com;CrA;CrB;Crv;Crt;Cru;Cyg;Del;Dor;Dra;Equ;Eri;For;Gem;Gru;Her;Hor;Hya;Hyi;Ind;Lac;Leo;LMi;Lep;Lib;Lup;Lyn;Lyr;Men;Mic;Mon;Mus;Nor;Oct;Oph;Ori;Pav;Peg;Per;Phe;Pic;Psc;PsA;Pup;Pyx;Ret;Sge;Sgr;Sco;Scl;Sct;Ser;Sex;Tau;Tel;Tri;TrA;Tuc;UMa;UMi;Vel;Vir;Vol;Vul".Split(new char[] { ';' }).Select(p => new Constellation { Name = p, Stars = new List<Star>() { } }).ToList();

        public void UpdateConstellations(List<Constellation> constellations)
        {
            using (var context = new ExoContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.Constellations.AddRange(constellations);

                context.Stars.Add(OurSolarSystem());

                context.SaveChanges();
            }
        }

        public List<Constellation> DownloadExoData()
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

            return _constellations;
        }

        private Star OurSolarSystem()
        {
            var star = new Star()
            {
                Name = "Sun",
                Type = "G2V",
                Mass = 1,
                Radius = 1,
                Teff = 5778,
                Luminosity = 1,
                FeH = (decimal)0.002,
                Age = (decimal)4.6,
                ApparMag = (decimal)-26.7,
                Distance = (decimal)4.84 / 1000000,
                NoPlanets = 8,
                NoPlanetsHZ = 2,
                HabZoneMin = (decimal)0.9,
                HabZoneMax = (decimal)1.5,
                Ra =int.MaxValue,
                Dec = int.MaxValue,

                Planets = new List<Planet>()
                {
                   new Planet()
                   {
                    Name = "Mercury",
                    ZoneClass = "Hot",
                    MassClass ="Mercurian",
                    CompositionClass = "rocky-iron",
                    AtmosphereClass = "no-atmosphere",
                    HabitableClass = "non-habitable",
                    Mass =  (decimal)0.055,
                    Radius = (decimal)0.3829,
                    Density = (decimal)0.984,
                    Gravity = (decimal)0.377,
                    EscVel =   (decimal) 0.38,
                    TsMin = 100,
                    TsMean = 340,
                    TsMax = 700,
                    SurfPress = 0,
                    Eccentricity =(decimal) 0.205,
                    MeanDistance =  (decimal)0.39,
                    Period = (decimal)87.96,
                    Esi = (decimal)0.596,
                    Habitable = false,
                    Disc_Method = "ima",
                    Disc_Year = -265,
                    },
                    new Planet()
                    {
                    Name = "Venus",
                    ZoneClass = "Hot",
                    MassClass ="Terran",
                    CompositionClass = "rocky-water",
                    AtmosphereClass = "metals-rich",
                    HabitableClass = "non-habitable",
                    Mass =  (decimal)0.815 ,
                    Radius = (decimal)0.9499 ,
                    Density = (decimal)0.951,
                    Gravity = (decimal)0.907,
                    EscVel =   (decimal) 0.926,
                    TsMin = 737,
                    TsMean = 737,
                    TsMax = 737,
                    SurfPress = 92,
                    Eccentricity =(decimal) 0.0067,
                    MeanDistance =  (decimal)0.728,
                    Period = (decimal)224.7,
                    Esi = (decimal)0.44,
                    Habitable = false,
                    Disc_Method = "ima",
               
                    },
                    new Planet()
                    {
                    Name = "Tellus",
                    ZoneClass = "Warm",
                    MassClass ="Terran",
                    CompositionClass = "rocky-water",
                    AtmosphereClass = "metals-rich",
                    HabitableClass = "mesoplanet",
                    Mass =  1 ,
                    Radius = 1,
                    Density = 1,
                    Gravity = 1,
                    EscVel = 1,
                    TsMin = 184,
                    TsMean = 288,
                    TsMax =  330,
                    SurfPress = 1,
                    Eccentricity =(decimal)0.0167,
                    MeanDistance =  1,
                    Period = 365,
                    Esi = 1,
                    Sph=1,
                    Habitable = true,
                    },
                    new Planet()
                    {
                    Name = "Mars",
                    ZoneClass = "Cold",
                    MassClass ="Subterran",
                    CompositionClass = "rocky-water",
                    AtmosphereClass = "metals-rich",
                    HabitableClass = "psychroplanet",
                    Mass =  (decimal)0.107 ,
                    Radius =(decimal) 0.533 ,
                    Density = (decimal)0.71,
                    Gravity = (decimal)0.38,
                    EscVel = (decimal)0.45,
                    TsMin = 130,
                    TsMean = 210,
                    TsMax =  308,
                    SurfPress = (decimal)0.006,
                    Eccentricity =(decimal)0.0934,
                    MeanDistance =  (decimal)1.5 ,
                    Period = (decimal)686.971,
                    Esi = (decimal)0.73,
                    Habitable = false,
                    Disc_Method = "ima",
                     
                    },
                    new Planet()
                    {
                    Name = "Jupiter",
                    ZoneClass = "Cold",
                    MassClass ="Jovian",
                    CompositionClass = "gas",
                    AtmosphereClass = "hydrogen-rich",
                    HabitableClass = "non-habitable",
                    Mass =  (decimal)317.8,
                    Radius =(decimal) 11.209 ,
                    Density = (decimal)0.24,
                    Gravity = (decimal)2.36,
                    EscVel = (decimal)5.32,
                    TsMean = 165,
                    Eccentricity =(decimal)0.0489,
                    MeanDistance =  (decimal)5.20 ,
                    Period = (decimal)4332.59,
                    Esi = 0,
                    Habitable = false,
                    Disc_Method = "ima",
                   
                    },
                    new Planet()
                    {
                    Name = "Saturn",
                    ZoneClass = "Cold",
                    MassClass ="Jovian",
                    CompositionClass = "gas",
                    AtmosphereClass = "hydrogen-rich",
                    HabitableClass = "non-habitable",
                    Mass =  (decimal)95.2,
                    Radius =(decimal) 9.45,
                    Density = (decimal)0.125,
                    Gravity = (decimal)0.916,
                    EscVel = (decimal)3.17,
                    TsMean = 165,
                    Eccentricity =(decimal)0.0565,
                    MeanDistance =  (decimal)9.58 ,
                    Period = (decimal)10759.22 ,
                    Esi = 0,
                    Habitable = false,
                    Disc_Method = "ima",
                     
                    },
                    new Planet()
                    {
                    Name = "Uranus",
                    ZoneClass = "Cold",
                    MassClass ="Neptunian",
                    CompositionClass = "gas",
                    AtmosphereClass = "hydrogen-rich",
                    HabitableClass = "non-habitable",
                    Mass =  (decimal)14.5,
                    Radius =(decimal) 4.01,
                    Density = (decimal)0.230,
                    Gravity = (decimal)0.889,
                    EscVel = (decimal)1.90,
                    TsMean = 76,
                    Eccentricity =(decimal)0.046,
                    MeanDistance =  (decimal)19.20,
                    Period = (decimal)30688.5,
                    Esi = 0,
                    Habitable = false,
                    Disc_Method = "ima",
                    Disc_Year =1781
                    },
                    new Planet()
                    {
                    Name = "Neptune",
                    ZoneClass = "Cold",
                    MassClass ="Neptunian",
                    CompositionClass = "gas",
                    AtmosphereClass = "hydrogen-rich",
                    HabitableClass = "non-habitable",
                    Mass =  (decimal)17.1,
                    Radius =(decimal) 3.88,
                    Density = (decimal)0.297,
                    Gravity = (decimal)1.12,
                    EscVel = (decimal)2.10,
                    TsMean = 72,
                    Eccentricity =(decimal)0.009456,
                    MeanDistance =  (decimal)30.05,
                    Period = 60182,
                    Esi = 0,
                    Habitable = false,
                    Disc_Method = "ima",
                    Disc_Year =1846
                    },
                }
            };

            return star;
        }

        private Star GetStar(string starname, string[] values)
        {
            var star = new Star()
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
            _constellations.FirstOrDefault(p => p.Name == values[36]).Stars.Add(star);
            return star;
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
                Disc_Year = values[67].ConvertYearIntToNullable(),

                Star = star
            };
        }
    }
}