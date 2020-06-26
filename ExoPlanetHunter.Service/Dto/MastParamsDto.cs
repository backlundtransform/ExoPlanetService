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
    }
}
