using ExoPlanetHunter.Service.Dto;
using ExoPlanetHunter.Service.Dto.Mast;
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
        private const string endpoint = "https://mast.stsci.edu";
        [TestMethod]
        public async Task GetMast()
        {

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(endpoint);

                var ra = 140.468;

                var dec = -18.866;

                var radius= 0.5;

                var mastRequest = new MastServiceDto {
                    Service = "Mast.Caom.Cone",
                    Format = "json",
                    PageSize = 2000,
                    RemoveNullColumns = true,
                    TimeOut = 30,
                    RemoveCache = true,
                    Params = new MastParamsDto() { Dec = dec, Ra =ra, Radius = radius }

                };

               var json = JsonConvert.SerializeObject(mastRequest);
              
                var result = await client.GetAsync($"/api/v0/invoke?request={json}");
                var resultContent = await result.Content.ReadAsStringAsync();

                var resultAsObject = JsonConvert.DeserializeObject<MastObjectDto>(resultContent);

                var timeSerie = new List<TransitTimeserie>();
    

                foreach (var table in resultAsObject.Data) {
    
                   if (table.DataProductType == "timeseries")
                   {
                        var mastDataDeliveryRequest = new MastServiceDto
                        {
                            Service = "Mast.DataDelivery",
                            Format = "json",
                            PageSize = 2000,
                            RemoveNullColumns = true,
                            TimeOut = 30,
                            RemoveCache = true,
                            Params = new MastParamsDto() { ObsIds = table.ObsId, Missions = table.Project, Filters = table.Filters, Urls = "None", Targets = "None" }

                        };

                        var dataJson = JsonConvert.SerializeObject(mastDataDeliveryRequest); ;
                        var dataResult = await client.GetAsync($"/api/v0/invoke?request={dataJson}");
                        var dataResultContent = await dataResult.Content.ReadAsStringAsync();
                       
                        var dataResultAsObject = JsonConvert.DeserializeObject<List<MastTimeserieDTO>>(dataResultContent).FirstOrDefault();
                        if (dataResultAsObject!=null&&dataResultAsObject.PlotSeries.FirstOrDefault() != null)
                        {
                            timeSerie.AddRange(dataResultAsObject.PlotSeries.First().Where(p=> !double.IsNaN(p.Last())).Select(p => new TransitTimeserie() { Index = p.First(), Value = p.Last(), Label = table.ObsId }));

                        }
                       
                    }
                }
                Assert.IsTrue(timeSerie.Any());

            }

        }
    }
}
