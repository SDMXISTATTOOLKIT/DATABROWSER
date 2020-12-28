using System.Collections.Generic;
using Newtonsoft.Json;

namespace EndPointConnector.JsonStatParser.Model.JsonStat.Extensions
{
    public class RelationshipItem
    {

        [JsonProperty("dimensions", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Dimensions;

        [JsonProperty("primaryMeasure", NullValueHandling = NullValueHandling.Ignore)]
        public string PrimaryMeasure;

    }
}