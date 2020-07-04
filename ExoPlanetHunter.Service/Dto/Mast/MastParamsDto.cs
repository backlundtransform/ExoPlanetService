using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExoPlanetHunter.Service.Dto
{
   public class MastParamsDto
    {
        [JsonProperty("ra")]
        public double Ra { get; set; }

        [JsonProperty("dec")]
        public double Dec { get; set; }

        [JsonProperty("radius")]
        public double Radius { get; set; }


        [JsonProperty("obsids")]
        public string ObsIds { get; set; }

        [JsonProperty("missions")]
        public string Missions { get; set; }

        [JsonProperty("filters")]
        public string Filters { get; set; }


        [JsonProperty("urls")]
        public string Urls { get; set; }

       

        [JsonProperty("targets")]
        public string Targets { get; set; }

    }
}
