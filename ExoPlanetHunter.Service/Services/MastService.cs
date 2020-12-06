using ExoPlanetHunter.Service.Dto;
using ExoPlanetHunter.Service.Dto.Mast;
using ExoPlanetHunter.Service.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ExoPlanetHunter.Service.Services
{
   public class MastService : IMastService
    {
        private const string endpoint = "https://mast.stsci.edu";

       public async Task<List<TransitTimeserie>> GetMast(double ra, double dec, double radius)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(endpoint);


                var mastRequest = new MastServiceDto
                {
                    Service = "Mast.Caom.Cone",
                    Format = "json",
                    PageSize = 20,
                    RemoveNullColumns = true,
                    TimeOut = 3000,
                    RemoveCache = true,
                    Params = new MastParamsDto() { Dec = dec, Ra = ra, Radius = radius }

                };

                var json = JsonConvert.SerializeObject(mastRequest);

                var result = await client.GetAsync($"/api/v0/invoke?request={json}");
                var resultContent = await result.Content.ReadAsStringAsync();

                var resultAsObject = JsonConvert.DeserializeObject<MastObjectDto>(resultContent);

                var timeSerie = new List<TransitTimeserie>();


                foreach (var table in resultAsObject.Data)
                {

                    if (table.DataProductType == "timeseries")
                    {
                        var mastDataDeliveryRequest = new MastServiceDto
                        {
                            Service = "Mast.DataDelivery",
                            Format = "json",
                            PageSize = 20,
                            RemoveNullColumns = true,
                            TimeOut = 30000,
                            RemoveCache = true,
                            Params = new MastParamsDto() { ObsIds = table.ObsId, Missions = table.Project, Filters = table.Filters, Urls = "None", Targets = "None" }

                        };

                        var dataJson = JsonConvert.SerializeObject(mastDataDeliveryRequest); ;
                        var dataResult = await client.GetAsync($"/api/v0/invoke?request={dataJson}");
                        var dataResultContent = await dataResult.Content.ReadAsStringAsync();

                        var dataResultAsObject = JsonConvert.DeserializeObject<List<MastTimeserieDTO>>(dataResultContent).FirstOrDefault();
                        if (dataResultAsObject != null && dataResultAsObject.PlotSeries.FirstOrDefault() != null)
                        {
                            timeSerie.AddRange(dataResultAsObject.PlotSeries.First().Where(p => !double.IsNaN(p.Last())).Select(p => new TransitTimeserie() { Index = p.First(), Value = p.Last(), Label = table.ObsId }));

                        }

                    }
                }
                return timeSerie.ToList();
            }
        }
    }
}
