using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExoPlanetHunter.Service.Dto.Mast
{
    public class MastObjectDto
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("data")]
        public IEnumerable<MastObjectDataDto> Data { get; set; }

    }
}
    