using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace EndPointConnector.JsonStatParser.Model.SdmxJson
{
    public class SdmxJsonHeader
    {

        [JsonProperty("schema", NullValueHandling = NullValueHandling.Ignore)]
        public Uri Schema { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("prepared", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? Prepared { get; set; }

        [JsonProperty("test", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Test { get; set; }

        [JsonProperty("content-languages", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> ContentLanguages { get; set; }

        [JsonProperty("sender", NullValueHandling = NullValueHandling.Ignore)]
        public SdmxJsonSender Sender { get; set; }

        [JsonProperty("links", NullValueHandling = NullValueHandling.Ignore)]
        public List<SdmxJsonLink> Links { get; set; }

    }
}