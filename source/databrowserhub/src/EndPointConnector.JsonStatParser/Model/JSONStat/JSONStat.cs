using Newtonsoft.Json;

namespace EndPointConnector.JsonStatParser.Model.JsonStat
{
    public class JsonStat
    {

        [JsonProperty("version")] public string Version { get; } = "2.0";

        [JsonProperty("class")] public string Class { get; } = "dataset";

        [JsonProperty("label")] public string Label { get; set; } = "";

    }
}