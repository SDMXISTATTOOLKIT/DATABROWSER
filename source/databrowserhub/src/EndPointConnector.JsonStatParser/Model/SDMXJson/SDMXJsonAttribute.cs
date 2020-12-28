using System.Collections.Generic;
using Newtonsoft.Json;

namespace EndPointConnector.JsonStatParser.Model.SdmxJson
{
    public class SdmxJsonAttribute : ItemWithLocalizedNames
    {

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("roles", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Roles { get; set; }

        [JsonProperty("relationship", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(RelationshipDictionaryConverter))]
        public Dictionary<string, object> Relationship { get; set; }

        [JsonProperty("values", NullValueHandling = NullValueHandling.Ignore)]
        public List<SdmxJsonGenericValueWrapper> Values { get; set; }

        [JsonProperty("annotations", NullValueHandling = NullValueHandling.Ignore)]
        public List<long> Annotations { get; set; }

    }
}