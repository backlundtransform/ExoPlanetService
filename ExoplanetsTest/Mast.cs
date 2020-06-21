using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://mast.stsci.edu");


                var json = "{\"service\":\"Mast.Caom.Cone\",\"format\":\"extjs\",\"pagesize\":5000,\"timeout\":10,\"removenullcolumns\":false,\"data\":null,\"params\":{\"ra\":140.468,\"dec\":-18.866,\"radius\":0.2,\"verb\":3,\"input\":\"140.468 -18.866\",\"cacheBreaker\":\"3bfffd71-4e1b-4030-b630-b3f640c1c621\"},\"clearcache\":false,\"columnsconfigid\":\"Mast.Caom.Cone\",\"page\":1}";
                var content = new StringContent($"request={json}", Encoding.UTF8, "application/json");
                var result = await client.GetAsync($"/api/v0/invoke?request={json}");
                var resultContent = await result.Content.ReadAsStringAsync();

                var resultAsObject = JsonConvert.DeserializeObject<dynamic>(resultContent);

                var tables= resultAsObject.data.Tables;

                var obsIds = new List<string>();
           

                foreach (var table in tables) {

                    var rows = table.Rows;
                    foreach (var row in rows)
                    {


                        if (row[12].ToString()== "timeseries")
                        {
                            obsIds.Add(row[9].ToString());

                        }

                    }

                
                }

                var dataJson = "{\"service\":\"Mast.DataDelivery\",\"params\":{\"missions\":\"TESS\",\"obsids\":\"tess2019085135100-s0010-0000000038591861-0140-s\",\"filters\":\"TESS\",\"urls\":\"NONE\",\"targets\":\"NONE\",\"cacheBreaker\":\"730d2290-4ee6-4aa6-bdfe-986721f75049\"},\"format\":\"datadelivery\",\"timeout\":10,\"page\":1,\"pagesize\":1000}";
                var dataResult= await client.GetAsync($"/api/v0/invoke?request={dataJson}");
                var dataResultContent = await dataResult.Content.ReadAsStringAsync();

                var dataResultAsObject = JsonConvert.DeserializeObject<dynamic>(dataResultContent);



            }




        }
    }
}
