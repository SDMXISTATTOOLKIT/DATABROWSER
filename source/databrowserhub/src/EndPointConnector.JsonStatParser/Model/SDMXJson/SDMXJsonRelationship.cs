using System.Collections.Generic;
using Newtonsoft.Json;

namespace EndPointConnector.JsonStatParser.Model.SdmxJson
{
    public class SdmxJsonRelationship
    {

        [JsonProperty("primaryMeasure", NullValueHandling = NullValueHandling.Ignore)]
        public string PrimaryMeasure { get; set; }

        [JsonProperty("dimensions", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Dimensions { get; set; }

    }
}