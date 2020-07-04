using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExoPlanetHunter.Service.Dto.Mast
{
   public class MastObjectDataDto
    {
        [JsonProperty("obs_id")]
        public string ObsId { get; set; }

        [JsonProperty("project")]

        public string Project { get; set; }

        [JsonProperty("filters")]
        public string Filters { get; set; }

        [JsonProperty("dataproduct_type")]
        public string DataProductType { get; set; }
    }
}
