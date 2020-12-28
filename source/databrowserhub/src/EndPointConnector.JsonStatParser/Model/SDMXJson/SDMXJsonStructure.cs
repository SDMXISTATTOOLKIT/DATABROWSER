using System.Collections.Generic;
using Newtonsoft.Json;

namespace EndPointConnector.JsonStatParser.Model.SdmxJson
{
    public class SdmxJsonStructure : ItemWithLocalizedNames
    {

        [JsonProperty("dimensions", NullValueHandling = NullValueHandling.Ignore)]
        public SdmxJsonDimensions Dimensions { get; set; }

        [JsonProperty("attributes", NullValueHandling = NullValueHandling.Ignore)]
        public SdmxJsonAttributes Attributes { get; set; }

        [JsonProperty("annotations", NullValueHandling = NullValueHandling.Ignore)]
        public List<SdmxJsonAnnotation> Annotations { get; set; }

    }
}