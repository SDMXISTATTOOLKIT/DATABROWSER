using System.Collections.Generic;
using Newtonsoft.Json;

namespace EndPointConnector.JsonStatParser.Model.SdmxJson
{
    public class SdmxJsonAttributes
    {

        [JsonProperty("dataSet", NullValueHandling = NullValueHandling.Ignore)]
        public List<SdmxJsonAttribute> DataSet { get; set; }

        [JsonProperty("series", NullValueHandling = NullValueHandling.Ignore)]
        public List<SdmxJsonAttribute> Series { get; set; }

        [JsonProperty("observation", NullValueHandling = NullValueHandling.Ignore)]
        public List<SdmxJsonAttribute> Observation { get; set; }

    }
}