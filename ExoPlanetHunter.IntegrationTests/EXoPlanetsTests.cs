
using ExoPlanetHunter.Service.Dto;
using ExoPlanetHunter.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ExoPlanetHunter.IntegrationTests
{
    public class ExoPlanetsTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
 
        private readonly string[] _constellations = "And;Ant;Aqr;Apu;Aps;Aql;Ari;Ara;Aur;Boo;Cae;Cam;Cnc;CVn;CMa;CMi;Cap;Car;Cas;Cen;Cep;Cet;Cha;Cir;Col;Com;CrA;CrB;Crv;Crt;Cru;Cyg;Del;Dor;Dra;Equ;Eri;For;Gem;Gru;Her;Hor;Hya;Hyi;Ind;Lac;Leo;LMi;Lep;Lib;Lup;Lyn;Lyr;Men;Mic;Mon;Mus;Nor;Oct;Oph;Ori;Pav;Peg;Per;Phe;Pic;Psc;PsA;Pup;Pyx;Ret;Sge;Sgr;Sco;Scl;Sct;Ser;Sex;Tau;Tel;Tri;TrA;Tuc;UMa;UMi;Vel;Vir;Vol;Vul;".Split(new char[] { ';' });

        public ExoPlanetsTests()
        {
            _server = new TestServer(new WebHostBuilder()
             .UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task TestConstellationsAsync()
        {
            var response = await _client.GetAsync("/api/Constellations");
            response.EnsureSuccessStatusCode();
        
            var responseString = await response.Content.ReadAsStringAsync();

            var constellations = JsonConvert.DeserializeObject<List<ConstellationDto>>(responseString);

            Assert.Equal(_constellations.Length,
                constellations.Count);

         
        }

        [Fact]
        public async Task TestConstellationAsync()
        {
            var response = await _client.GetAsync("/api/Constellations/1");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            var constellation = JsonConvert.DeserializeObject<ConstellationDto>(responseString);

            Assert.Equal("And",
                constellation.Name);


        }

        [Fact]
        public async Task TestConstellationStarsAsync()
        {
            var response = await _client.GetAsync("/api/Constellations/1/stars");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            var constellationsStars = JsonConvert.DeserializeObject<ConstellationStarsDto>(responseString);
            Assert.False(constellationsStars.Stars.Where(prop => prop.Constellation != "And").Any());

        }


        [Fact]
        public async Task TestStarAsync()
        {
            var response = await _client.GetAsync("/api/stars/1");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            var constellationsStar = JsonConvert.DeserializeObject<StarDto>(responseString);
            Assert.Equal("14 And",
                 constellationsStar.Name);

        }

        [Fact]
        public async Task TestStarPlanetsAsync()
        {
            var response = await _client.GetAsync("/api/stars/1/Planets");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            var constellationsStarPlanets = JsonConvert.DeserializeObject<StarPlanetsDto>(responseString);
            Assert.True(constellationsStarPlanets.Planets.Any());

        }
        [Fact]
        public async Task TestStarsDistancesAsync()
        {
            var response = await _client.GetAsync($"/api/Stars?$filter=Distance gt 100 and Distance lt 200");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            var stars = JsonConvert.DeserializeObject<List<StarDto>>(responseString);

            Assert.False(stars.Where(p => p.Distance > 200).Any());
            Assert.False(stars.Where(p => p.Distance < 100).Any());
            Assert.True(stars.Where(p => p.Distance > 100).Any());

        }
        [Fact]
        public async Task TestHabitablePlanetsAsync()
        {
            var response = await _client.GetAsync($"/api/Planets?$filter= Habitable eq true");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            var planets = JsonConvert.DeserializeObject<List<PlanetDto>>(responseString);

            Assert.True(planets.Any());
            Assert.False(planets.Where(p => p.Habitable==false).Any());
          

        }

    }
}