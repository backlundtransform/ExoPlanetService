using ExoPlanetHunter.Database;
using ExoPlanetHunter.Database.entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;

namespace ExoPlanetHunter.PHL.Integration
{
    public class PhlService : IPhlService
    {
        private string exoplaneturl = $"http://www.hpcf.upr.edu/~abel/phl/hec2/database/phl_exoplanet_catalog.csv";
        private List<Constellation> _constellations = $"And;Ant;Aqr;Apu;Aps;Aql;Ari;Ara;Aur;Boo;Cae;Cam;Cnc;CVn;CMa;CMi;Cap;Car;Cas;Cen;Cep;Cet;Cha;Cir;Col;Com;CrA;CrB;Crv;Crt;Cru;Cyg;Del;Dor;Dra;Equ;Eri;For;Gem;Gru;Her;Hor;Hya;Hyi;Ind;Lac;Leo;LMi;Lep;Lib;Lup;Lyn;Lyr;Men;Mic;Mon;Mus;Nor;Oct;Oph;Ori;Pav;Peg;Per;Phe;Pic;Psc;PsA;Pup;Pyx;Ret;Sge;Sgr;Sco;Scl;Sct;Ser;Sex;Tau;Tel;Tri;TrA;Tuc;UMa;UMi;Vel;Vir;Vol;Vul".Split(new char[] { ';' }).Select(p => new Constellation { Name = p, Stars = new List<Star>() { } }).ToList();
        private string[] _headers=null;
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
                var lines = client.DownloadString(exoplaneturl);

                try {

                    using (StringReader reader = new StringReader(lines))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {

                            if (_headers==null)
                            {
                                _headers = line.Split(",").ToArray();
                                continue;
                            }
                            var values = line.Split("\",\"").Select(p=>p.Trim('"')).ToArray();

                            var starname = values[Array.IndexOf(_headers, "S_NAME")];

                            if (!Starlist.Any(o => o.Name == starname))
                            {
                                Starlist.Add(GetStar(starname, values));
                            }
                            var star = Starlist.First(o => o.Name == starname);
                            star.Planets.Add(GetPlanet(values, star));
                        }
                    }

                }
                catch (Exception e) {

                    return _constellations;
                }
            return _constellations;
            }
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
             
                Constellation = _constellations.FirstOrDefault(p => p.Name == values[Array.IndexOf(_headers, "S_CONSTELLATION_ABR")]),
                Type = values[Array.IndexOf(_headers, "S_TYPE")],
                Mass = values[Array.IndexOf(_headers, "S_MASS")].ToNullable<decimal>(),

                   
                Radius = values[Array.IndexOf(_headers, "S_RADIUS")].ToNullable<decimal>(),
                Teff = values[Array.IndexOf(_headers, "S_TEMPERATURE")].ToNullable<decimal>(),
                Luminosity = values[Array.IndexOf(_headers, "S_LUMINOSITY")].ToNullable<decimal>(),
              
                Age = values[Array.IndexOf(_headers, "S_AGE")].ToNullable<decimal>(),
                ApparMag = values[Array.IndexOf(_headers, "S_MAG")].ToNullable<decimal>(),
                Distance = values[Array.IndexOf(_headers, "S_DISTANCE")].ToNullable<decimal>(),

                Ra = values[Array.IndexOf(_headers, "S_RA_H")].ToNullable<decimal>(),
                Dec = values[Array.IndexOf(_headers, "S_DEC")].ToNullable<decimal>(),
              
                HabZoneMin = values[Array.IndexOf(_headers, "S_HZ_OPT_MIN")].ToNullable<decimal>(),
                HabZoneMax = values[Array.IndexOf(_headers, "S_HZ_OPT_MAX")].ToNullable<decimal>(),
      
                Planets = new List<Planet>() { }
            };
            _constellations.FirstOrDefault(p => p.Name == values[Array.IndexOf(_headers, "S_CONSTELLATION_ABR")]).Stars.Add(star);
            return star;
        }

        private Planet GetPlanet(string[] values, Star star)
        {

            var planet = new Planet();
            planet.Name = values[Array.IndexOf(_headers, "P_NAME")];
            planet.ZoneClass = values[Array.IndexOf(_headers, "P_TYPE_TEMP")]; ;
            planet.MassClass = values[Array.IndexOf(_headers, "P_TYPE")]; 
            planet.AtmosphereClass = values[Array.IndexOf(_headers, "P_ATMOSPHERE")]; ;
            planet.Mass = values[Array.IndexOf(_headers, "P_MASS")].ToNullable<decimal>();
            planet.Radius = values[Array.IndexOf(_headers, "P_RADIUS")].ToNullable<decimal>();
            planet.Density = values[Array.IndexOf(_headers, "P_DENSITY")].ToNullable<decimal>();
            planet.Gravity = values[Array.IndexOf(_headers, "P_GRAVITY")].ToNullable<decimal>();
            planet.EscVel = values[Array.IndexOf(_headers, "P_ESCAPE")].ToNullable<decimal>();
            planet.SFluxMin = values[Array.IndexOf(_headers, "P_FLUX_MIN")].ToNullable<decimal>();
            planet.SFluxMean = values[Array.IndexOf(_headers, "P_FLUX")].ToNullable<decimal>();
            planet.SFluxMax = values[Array.IndexOf(_headers, "P_FLUX_MAX")].ToNullable<decimal>();
            planet.TeqMin = values[Array.IndexOf(_headers, "P_TEMP_EQUIL_MIN")].ToNullable<decimal>();
            planet.TeqMean = values[Array.IndexOf(_headers, "P_TEMP_EQUIL")].ToNullable<decimal>();
            planet.TeqMax = values[Array.IndexOf(_headers, "P_TEMP_EQUIL_MAX")].ToNullable<decimal>();

            planet.TsMin= values[Array.IndexOf(_headers, "P_TEMP_EQUIL_MIN")].ToNullable<decimal>();
            planet.TsMean = values[Array.IndexOf(_headers, "P_TEMP_EQUIL")].ToNullable<decimal>();
            planet.TsMax = values[Array.IndexOf(_headers, "P_TEMP_EQUIL_MAX")].ToNullable<decimal>();
            
            planet.Period = values[Array.IndexOf(_headers, "P_PERIOD")].ToNullable<decimal>();
            planet.SemMajorAxis = values[Array.IndexOf(_headers, "P_SEMI_MAJOR_AXIS_EST")].ToNullable<decimal>();
            planet.Eccentricity = values[Array.IndexOf(_headers, "P_ECCENTRICITY")].ToNullable<decimal>();
            planet.MeanDistance = values[Array.IndexOf(_headers, "P_DISTANCE")].ToNullable<decimal>();
            planet.Inclination = values[Array.IndexOf(_headers, "P_INCLINATION")].ToNullable<decimal>();
            planet.Omega = values[Array.IndexOf(_headers, "P_OMEGA")].ToNullable<decimal>();

            planet.Esi = values[Array.IndexOf(_headers, "P_ESI")].ToNullable<decimal>();
            var hab = Convert.ToInt32(values[Array.IndexOf(_headers, "P_HABITABLE")], CultureInfo.InvariantCulture);
            planet.Habitable = hab == 2 || hab == 1;
            
            planet.Disc_Method = values[Array.IndexOf(_headers, "P_DETECTION")].Replace(" ", string.Empty);
            planet.Disc_Year = values[Array.IndexOf(_headers, "P_YEAR")].ConvertYearIntToNullable();

            planet.Star = star;
            return planet;
        }
    }
}