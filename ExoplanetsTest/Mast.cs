using ExoPlanetHunter.Service.Dto;
using ExoPlanetHunter.Service.Dto.Mast;
using ExoPlanetHunter.Service.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace ExoplanetsTest
{


    [TestClass]
    public class MastTest
    {
        [TestMethod]
        public async Task GetMast()
        {
            var timeSerie = await new MastService().GetMast(140.468, -18.866, 0.5);
            Assert.IsTrue(timeSerie.Any());

        }
    }
}
