using System.Collections.Generic;
using Newtonsoft.Json;

namespace EndPointConnector.JsonStatParser.Model.JsonStat.Extensions
{
    public class AttributeMapping
    {

        [JsonProperty("label", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Label { get; set; }

        public AttributeMapping()
        {
            Label = new Dictionary<string, string>();
        }

    }
}