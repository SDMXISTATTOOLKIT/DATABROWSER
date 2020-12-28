using Newtonsoft.Json;

namespace EndPointConnector.JsonStatParser.Model.JsonStat
{
    public class JsonStatDimension
    {

        [JsonProperty("label")] public string Label { get; set; }

        [JsonIgnore] public int Count => Category.Count;

        [JsonProperty("category")]
        internal JsonStatDimensionCategory Category { get; set; } = new JsonStatDimensionCategory();

        public JsonStatDimension(string label)
        {
            Label = label;
        }

        public void Clear()
        {
            Category.Clear();
            Label = null;
        }

    }
}