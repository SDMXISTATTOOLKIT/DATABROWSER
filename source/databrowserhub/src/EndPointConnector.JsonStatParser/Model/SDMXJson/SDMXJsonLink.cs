using Newtonsoft.Json;

namespace EndPointConnector.JsonStatParser.Model.SdmxJson
{
    public class SdmxJsonLink
    {

        [JsonProperty("urn", NullValueHandling = NullValueHandling.Ignore)]
        public string Urn { get; set; }

        [JsonProperty("rel", NullValueHandling = NullValueHandling.Ignore)]
        public string Rel { get; set; }

    }
}