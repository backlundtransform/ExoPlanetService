using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExoPlanetHunter.Service.Dto.Mast
{
    public class MastTimeserieDTO
    {
        [JsonProperty("obsid")]
        public string ObsId{ get; set; }

        [JsonProperty("mission")]
        public string Mission { get; set; }

        [JsonProperty("plot_labels")]
        public IEnumerable<string> PlotLabels { get; set; }

        [JsonProperty("plot_series")]
        public List<List<double[]>> PlotSeries { get; set; }
    }
}
