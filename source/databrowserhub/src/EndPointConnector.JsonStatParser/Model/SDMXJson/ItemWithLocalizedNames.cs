using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace EndPointConnector.JsonStatParser.Model.SdmxJson
{
    public class ItemWithLocalizedNames
    {

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ObservationIndexConverter))]
        public string Name { get; set; }

        [JsonProperty("names", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Names { get; set; }


        public Dictionary<string, string> GetAllNames(string unmappedLanguageDefaultId = "*")
        {
            var allNames = Names ?? (Name != null
                ? new Dictionary<string, string> {{unmappedLanguageDefaultId, Name}}
                : new Dictionary<string, string>());

            return allNames;
        }

        public string GetLocalizedName(string language)
        {
            if (Names != null && Names.TryGetValue(language, out var result)) {
                return result;
            }

            if (Name != null) {
                return Name;
            }

            if (Names != null && Names.Count > 0) {
                return Names.ElementAt(0).Value;
            }

            return Name;
        }

    }
}