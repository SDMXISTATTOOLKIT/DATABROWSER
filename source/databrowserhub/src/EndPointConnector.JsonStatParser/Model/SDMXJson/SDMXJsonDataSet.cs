using System.Collections.Generic;
using Newtonsoft.Json;

namespace EndPointConnector.JsonStatParser.Model.SdmxJson
{
    public class SdmxJsonDataSet
    {

        [JsonProperty("action", NullValueHandling = NullValueHandling.Ignore)]
        public string Action { get; set; }

        [JsonProperty("links", NullValueHandling = NullValueHandling.Ignore)]
        public List<SdmxJsonLink> Links { get; set; }

        [JsonProperty("annotations", NullValueHandling = NullValueHandling.Ignore)]
        public List<long> Annotations { get; set; }

        [JsonProperty("observations", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ObservationDictionaryConverter))]
        public Dictionary<List<int>, List<ObservationValue>> Observations { get; set; }

        [JsonProperty("series", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SeriesDictionaryConverter))]
        public Dictionary<List<int>, SeriesValue> Series { get; set; }

        [JsonProperty("attributes", NullValueHandling = NullValueHandling.Ignore)]
        public List<int?> Attributes { get; set; }

        public bool HasSeries => Series?.Count > 0;

    }
}