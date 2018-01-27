using ExoPlanetHunter.PHL;
using ExoPlanetHunter.Service;
using ExoPlanetHunter.Service.Dto;
using ExoPlanetHunter.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests
{
    public class ConstellationsTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        private readonly string[] _constellations = "And;Ant;Aqr;Apu;Aps;Aql;Ari;Ara;Aur;Boo;Cae;Cam;Cnc;CVn;CMa;CMi;Cap;Car;Cas;Cen;Cep;Cet;Cha;Cir;Col;Com;CrA;CrB;Crv;Crt;Cru;Cyg;Del;Dor;Dra;Equ;Eri;For;Gem;Gru;Her;Hor;Hya;Hyi;Ind;Lac;Leo;LMi;Lep;Lib;Lup;Lyn;Lyr;Men;Mic;Mon;Mus;Nor;Oct;Oph;Ori;Pav;Peg;Per;Phe;Pic;Psc;PsA;Pup;Pyx;Ret;Sge;Sgr;Sco;Scl;Sct;Ser;Sex;Tau;Tel;Tri;TrA;Tuc;UMa;UMi;Vel;Vir;Vol;Vul;".Split(new char[] { ';' });

        public ConstellationsTests()
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


    }
}