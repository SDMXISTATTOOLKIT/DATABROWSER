using System.Collections.Generic;
using Newtonsoft.Json;

namespace EndPointConnector.JsonStatParser.Model.SdmxJson
{
    public class SdmxJsonDimension
    {

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ObservationIndexConverter))]
        public string Name { get; set; }

        [JsonProperty("names", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Names { get; set; }

        [JsonProperty("order", NullValueHandling = NullValueHandling.Ignore)]
        public int? Order { get; set; }

        [JsonProperty("keyPosition", NullValueHandling = NullValueHandling.Ignore)]
        public long? KeyPosition { get; set; }

        [JsonProperty("roles", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Roles { get; set; }

        [JsonProperty("values", NullValueHandling = NullValueHandling.Ignore)]
        public List<SdmxJsonGenericValueWrapper> Values { get; set; }

        [JsonProperty("annotations", NullValueHandling = NullValueHandling.Ignore)]
        public List<long> Annotations { get; set; }

    }
}