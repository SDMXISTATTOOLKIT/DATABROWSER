using System.Collections.Generic;
using Newtonsoft.Json;

namespace EndPointConnector.JsonStatParser.Model.JsonStat
{
    public class JsonStatRole
    {

        [JsonProperty("time")] public List<string> Time { get; set; }

        [JsonProperty("geo")] public List<string> Geo { get; set; }

        [JsonProperty("metric")] public List<string> Metric { get; set; }

        public JsonStatRole()
        {
            Time = new List<string>();
            Geo = new List<string>();
            Metric = new List<string>();
        }

    }
}