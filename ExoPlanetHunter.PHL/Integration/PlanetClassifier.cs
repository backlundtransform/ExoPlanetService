using ExoPlanetHunter.Database;
using ExoPlanetHunter.Database.entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ExoPlanetHunter.PHL.Integration
{
    public class PlanetClassifier
    {

        private readonly double _tempEarth = 288;

        private readonly string exoplaneturl = $"https://exoplanetarchive.ipac.caltech.edu/";

        public  List<Star> DownloadAndClassifieStars()
        {

            var stars = new List<Star>();
            return stars;
           

        }

        public void AddClassifiedStars(List<Star> stars)
        {
            using (var context = new ExoContext())
            {

                var oldstars = context.Stars.AsQueryable();

                foreach (var star in stars)
                {

                   if(!oldstars.Any(p=>p.Name == star.Name))
                    {

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
        public (decimal habZoneMin, decimal habZoneMax) CalculateGoldiLockZone(double magnitude, double starDistance)
        {
          
            var mv = magnitude - (5 * Math.Log10((double)starDistance / 10));
            var mbol = mv - 0.4;
            var lStar = Math.Pow(10, (mbol - 4.72) / -2.5);
            return ((decimal)Math.Sqrt(lStar / 1.1), (decimal)Math.Sqrt(lStar / 0.21));
   
        }


        public double CalculateEsi(double radius,double temp)
        {
           
            var esi = 1 - Math.Sqrt(0.5 * (Math.Pow((temp - _tempEarth) / (temp + _tempEarth), 2) + Math.Pow((radius - 1) / (radius + 1), 2)));

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
            var r = new double[][] { new[] { 0.0, 0.0, 0.0 }, new[] { 0.0, 0.0, 0.0 }, new[] { 0.0, 0.0, 0.0 } };
            r[0][0] = cosa * cosb * cosc - sina * sinb;
            r[0][1] = -cosa * sinb - sina * cosb * cosc;
            r[0][2] = -cosb * sinc;
            r[1][0] = sina * cosb + cosa * sinb * cosc;
            r[1][1] = cosa * cosb - sina * sinb * cosc;
            r[1][2] = -sinb * sinc;
            r[2][0] = cosa * sinc;
            r[2][1] = -sina * sinc;
            r[2][2] = cosc;
            var x2 = new[] { 0.0, 0.0, 0.0 };
            for (var i = 0; i < 3; i++)
                x2[i] = r[i][0] * x1[0] + r[i][1] * x1[1] + r[i][2] * x1[2];
            var ra2 = Math.Atan2(x2[1], x2[0]);
            if (ra2 < 0.0)
                ra2 += 2.0 * Math.PI;
            var dec2 = Math.Asin(x2[2]);
            return (ra2, dec2);
        }

    }
}
