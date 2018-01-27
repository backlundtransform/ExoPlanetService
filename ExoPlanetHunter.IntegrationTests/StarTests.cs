using ExoPlanetHunter.Service.Dto;
using ExoPlanetHunter.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;

using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ExoPlanetHunter.IntegrationTests
{
   public class StarTests
    {
        //private readonly TestServer _server;
        //private readonly HttpClient _client;
        //private readonly string url = "/api/Stars";

        //public StarTests()
        //{
        //    _server = new TestServer(new WebHostBuilder()
        //     .UseStartup<Startup>());
        //    _client = _server.CreateClient();
        //}


        //[Fact]
        //public async Task TestStarsDistancesAsync()
        //{
        //    var response = await _client.GetAsync($"/api/Stars?$filter==distance gt 100 and distance lt 200");
        //    response.EnsureSuccessStatusCode();

        //    var responseString = await response.Content.ReadAsStringAsync();

        //    var stars = JsonConvert.DeserializeObject<List<StarDto>>(responseString);

        //    Assert.False(stars.Where(p=>p.Distance > 200).Any());
        //    Assert.False(stars.Where(p => p.Distance < 100).Any());
        //    Assert.True(stars.Where(p => p.Distance >100).Any());

        //}

    }
}
