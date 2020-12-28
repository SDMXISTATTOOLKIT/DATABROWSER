using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace EndPointConnector.JsonStatParser.Model.SdmxJson
{
    public class SdmxJsonAnnotation
    {

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public TypeEnum? Type { get; set; }

        [JsonProperty("uri", NullValueHandling = NullValueHandling.Ignore)]
        public string Uri { get; set; }

        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        private string Text { get; set; }

        [JsonProperty("texts", NullValueHandling = NullValueHandling.Ignore)]
        private Dictionary<string, string> Texts { get; set; }


        public string GetLocalizedText(string language)
        {
            // return chosen language
            if (Texts != null && Texts.TryGetValue(language, out var result)) {
                return result;
            }

            //return 1st random translated value
            if (Texts != null && Texts.Count > 0) {
                return Texts.ElementAt(0).Value;
            }

            // return default title
            return !string.IsNullOrEmpty(Text) ? Text : Title;
        }

    }
}