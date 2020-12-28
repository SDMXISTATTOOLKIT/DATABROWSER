using System.Collections.Generic;
using Newtonsoft.Json;

namespace EndPointConnector.JsonStatParser.Model.SdmxJson
{
    public class SeriesValue
    {

        [JsonProperty("attributes", NullValueHandling = NullValueHandling.Ignore)]
        public List<int?> Attributes { get; set; }

        [JsonProperty("annotations", NullValueHandling = NullValueHandling.Ignore)]
        public List<int?> Aannotations { get; set; }

        [JsonProperty("observations", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<int, List<ObservationValue>> Observations { get; set; }

    }
}