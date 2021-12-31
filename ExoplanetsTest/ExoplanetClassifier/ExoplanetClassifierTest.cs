using ExoPlanetHunter.Database.entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExoplanetHunter.UnitTest.ExoplantetClassifier
{

   
    [TestClass]
    public class ExoplanetClassifierTest
    {
       



        // https://exoplanetarchive.ipac.caltech.edu/

        private readonly Star _star = new Star()
        {
            Name = "11 Com",
            Type = "G8 III",
            Mass = (decimal)2.7,
            Ra = (decimal)185.17928, 
            Dec = (decimal)17.792868,

            Radius = 19,
            Teff = 4742,
            Luminosity = (decimal)2.243,
            FeH = null,
            Age = (decimal)4.6,
            ApparMag = null,
            Distance = (decimal)93.1846,
            NoPlanets = 1,
            NoPlanetsHZ =null,
            HabZoneMin = null,
            HabZoneMax = null,
            Planets = new List<Planet>()
                {
                   new Planet()
                   {
                    Name = "11 Com b",
                    TeqMean =(decimal)799.44963,
                    ZoneClass =null,
                    MassClass =null,
                
                    Mass =  6165,
                    Radius = null,
                    Density = (decimal)0.984,
                    Eccentricity =(decimal) 0.231,
                    MeanDistance =  (decimal)1.29,
                    Period = (decimal)326.03,
                    Esi = null,
                    Habitable = null,
                    Disc_Method = "Radial Velocity",
                    Disc_Year = 2017,

                    }
                }
        };
        [TestMethod]
        public void CalculateGoldiLockZone()
        {
            var magnitude = 4.72307;
            var mv = magnitude - (5 * Math.Log10((double)_star.Distance/10));
            var mbol = mv - 0.4;
            var lStar=Math.Pow(10, (mbol-4.72)/-2.5);
            _star.HabZoneMin = (decimal)Math.Sqrt(lStar / 1.1);
            _star.HabZoneMax = (decimal)Math.Sqrt(lStar / 0.21);
            Assert.IsTrue(_star.HabZoneMin >=(decimal)10.1);
            Assert.IsTrue(_star.HabZoneMax <= (decimal)24.8);

        }


        [TestMethod]
        public void CalculateMassClass()
        {
            var mass = _star.Planets.First().Mass;
            if (mass <= (decimal)0.1)
            {
                _star.Planets.First().MassClass = "Mercurian";

            }

            if (mass <= (decimal)0.5 && mass > (decimal)0.1)
            {
                _star.Planets.First().MassClass = "Subterran";

            }

            if (mass <= 2 && mass > (decimal)0.5)
            {
                _star.Planets.First().MassClass = "Terran";

            }

            if (mass <= 10 && mass > 2)
            {
                _star.Planets.First().MassClass = "Superterran";

            }


            if (mass <= 50 && mass > 10)
            {
                _star.Planets.First().MassClass = "Neptunian";

            }

            if (mass > 50)
            {
                _star.Planets.First().MassClass = "Jovian";

            }



            Assert.IsTrue(_star.Planets.First().MassClass =="Jovian");
        }


        [TestMethod]
        public void CalculateZoneClass()
        {
            var temp= _star.Planets.First().TeqMean;
            if (temp >= (decimal)273.15 && temp <= (decimal)323.15)
            {
                _star.Planets.First().ZoneClass= "Warm";

            }

            if (temp > (decimal)323.15)
            {
                _star.Planets.First().ZoneClass = "Hot";

            }

            if (temp < (decimal)273.15)
            {
                _star.Planets.First().ZoneClass = "Cold";

            }



            Assert.IsTrue(_star.Planets.First().ZoneClass == "Hot");
        }


        [TestMethod]
        public void GetConstellation()
        {
           
            var name = GetConstellation(185.17928, 17.792868);

            Assert.IsTrue(name == "Com");

        }

       private string GetConstellation(double ra, double dec)
        {
            string json = File.ReadAllText(@"constellation.json"); ;

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
                if (dec < (double)table[i][2] || ra/15 < (double)table[i][0] || ra/15 >= (double)table[i][1])
                {

                    continue;
                }
                    
                return (string)table[i][3];
            }
            return "";    
        }


        private (double,double) Precess(double ra1, double dec1, double epoch1, double epoch2)
        {
            var cdr = Math.PI / 180.0;
            var csr = cdr / 3600.0;
            var a = Math.Cos(dec1);
            var x1 = new[] { a * Math.Cos(ra1), a * Math.Sin(ra1), Math.Sin(dec1) };
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
            var r = new double[][]{ new[] { 0.0, 0.0, 0.0 }, new[] { 0.0, 0.0, 0.0 }, new[] { 0.0, 0.0, 0.0 }};
            r[0][0] = cosa * cosb * cosc - sina * sinb;
            r[0][1] = -cosa * sinb - sina * cosb * cosc;
            r[0][2] = -cosb * sinc;
            r[1][0] = sina * cosb + cosa * sinb * cosc;
            r[1][1] = cosa * cosb - sina * sinb * cosc;
            r[1][2] = -sinb * sinc;
            r[2][0] = cosa * sinc;
            r[2][1] = -sina * sinc;
            r[2][2] = cosc;
           var x2 =new[] { 0.0, 0.0, 0.0 };
            for (var i = 0; i < 3; i++)
                x2[i] = r[i][0] * x1[0] + r[i][1] * x1[1] + r[i][2] * x1[2];
          var  ra2 = Math.Atan2(x2[1], x2[0]);
            if (ra2 < 0.0)
                ra2 += 2.0 * Math.PI;
           var dec2 = Math.Asin(x2[2]);
            return (ra2, dec2);
        }

    }
}
