using Newtonsoft.Json;

namespace EndPointConnector.JsonStatParser.Model.JsonStat.Extensions
{
    public class AttributeItem
    {

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("annotations", NullValueHandling = NullValueHandling.Ignore)]
        public int[] Annotations { get; set; }

        [JsonProperty("parent", NullValueHandling = NullValueHandling.Ignore)]
        public string Parent { get; set; }

    }
}