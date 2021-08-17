using ExoPlanetHunter.Database.entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ExoplanetHunter.UnitTest.ExoplantetClassifier
{
    [TestClass]
    public class ExoplanetClassifierTest
    {



       private readonly Star _star = new Star()
        {
            Name = "11 Com",
            Type = "G8 III",
            Mass = (decimal)2.7,
            Radius = 19,
            Teff = 4742,
            Luminosity = (decimal)2.243,
            FeH = null,
            Age = (decimal)4.6,
            ApparMag = null,
            Distance = (decimal)303.7,
            NoPlanets = 1,
            NoPlanetsHZ =null,
            HabZoneMin = null,
            HabZoneMax = null,
            Planets = new List<Planet>()
                {
                   new Planet()
                   {
                    Name = "11 Com b",
                    ZoneClass =null,
                    MassClass =null,
                    CompositionClass = null,
                    AtmosphereClass = null,
                    HabitableClass = null,
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

            _star.HabZoneMin = (decimal)Math.Sqrt(Math.Pow(10, (double)_star.Luminosity)/1.1);
            _star.HabZoneMax = (decimal)Math.Sqrt(Math.Pow(10, (double)_star.Luminosity) /0.53);

            Assert.IsTrue(_star.HabZoneMin == (decimal)10.1);
            Assert.IsTrue(_star.HabZoneMax == (decimal)24.8);

        }
       
      
    }
}
