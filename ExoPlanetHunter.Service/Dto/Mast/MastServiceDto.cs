using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExoPlanetHunter.Service.Dto
{
   public class MastServiceDto
    {

        [JsonProperty("service")]
        public string Service { get; set; }

        [JsonProperty("format")]
        public string Format { get; set; }

        [JsonProperty("pagesize")]

        public int PageSize { get; set; }

        [JsonProperty("removenullcolumns")]

        public bool RemoveNullColumns { get; set; }

        [JsonProperty("params")]

        public MastParamsDto Params { get; set; }

        [JsonProperty("timeout")]

        public int TimeOut { get; set; }

        [JsonProperty("removecache")]

        public bool RemoveCache { get; set; }
    }
}
