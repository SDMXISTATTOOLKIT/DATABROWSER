using System.Collections.Generic;
using Newtonsoft.Json;

namespace EndPointConnector.JsonStatParser.Model.SdmxJson
{
    public class SdmxJsonData
    {

        [JsonProperty("dataSets", NullValueHandling = NullValueHandling.Ignore)]
        public List<SdmxJsonDataSet> DataSets { get; set; }

        [JsonProperty("structure", NullValueHandling = NullValueHandling.Ignore)]
        public SdmxJsonStructure Structure { get; set; }

    }
}