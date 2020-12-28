using System.Collections.Generic;
using Newtonsoft.Json;

namespace EndPointConnector.JsonStatParser.Model.JsonStat.Extensions
{
    public class Attribute
    {

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("roles", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Roles { get; set; }

        [JsonProperty("relationship", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Relationship { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("values", NullValueHandling = NullValueHandling.Ignore)]
        public List<AttributeItem> Values { get; set; }

        public Attribute()
        {
            Values = new List<AttributeItem>();
        }

    }
}