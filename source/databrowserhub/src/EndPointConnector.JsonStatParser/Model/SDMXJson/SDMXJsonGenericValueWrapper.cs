using Newtonsoft.Json;

namespace EndPointConnector.JsonStatParser.Model.SdmxJson
{
    public class SdmxJsonGenericValueWrapper : ItemWithLocalizedNames
    {

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id
        {
            get => ResolveNotNullId();
            set => _id = value;
        }

        [JsonProperty("order", NullValueHandling = NullValueHandling.Ignore)]
        public long? Order { get; set; }

        [JsonProperty("annotations", NullValueHandling = NullValueHandling.Ignore)]
        public int[] Annotations { get; set; }

        [JsonProperty("parent", NullValueHandling = NullValueHandling.Ignore)]
        public string Parent { get; set; }

        [JsonIgnore] private string _id;

        protected string ResolveNotNullId()
        {
            return _id ?? GetLocalizedName("en");
        }

    }
}