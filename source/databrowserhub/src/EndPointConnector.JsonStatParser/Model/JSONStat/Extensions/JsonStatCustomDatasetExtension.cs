using Newtonsoft.Json;

namespace EndPointConnector.JsonStatParser.Model.JsonStat.Extensions
{
    public class JsonStatCustomDatasetExtension
    {

        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public AttributeMapping Status { get; set; }

        [JsonProperty("attributes", NullValueHandling = NullValueHandling.Ignore)]
        public DatasetAttributes Attributes { get; set; }

        public JsonStatCustomDatasetExtension()
        {
            Attributes = new DatasetAttributes();
        }

        public void AddStatus(string id, string value)
        {
            Status ??= new AttributeMapping();

            Status.Label[id] = value;
        }

    }
}