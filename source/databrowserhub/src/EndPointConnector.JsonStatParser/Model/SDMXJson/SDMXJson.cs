using System.Collections.Generic;
using EndPointConnector.Interfaces.JsonStat;
using Newtonsoft.Json;

namespace EndPointConnector.JsonStatParser.Model.SdmxJson
{
    public class SdmxJson
    {

        [JsonProperty("header", NullValueHandling = NullValueHandling.Ignore)]
        public SdmxJsonHeader Header { get; set; }

        [JsonProperty("meta", NullValueHandling = NullValueHandling.Ignore)]
        public SdmxJsonHeader Meta
        {
            set => Header = value;
        }

        [JsonProperty("dataSets", NullValueHandling = NullValueHandling.Ignore)]
        public List<SdmxJsonDataSet> DataSets { get; set; }

        [JsonProperty("structure", NullValueHandling = NullValueHandling.Ignore)]
        public SdmxJsonStructure Structure { get; set; }

        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public SdmxJsonData Data
        {
            get => _data;
            set
            {
                _data = value;
                DataSets = value.DataSets;
                Structure = value.Structure;
            }
        }

        private SdmxJsonData _data;


        public static SdmxJson FromJson(string json, IFromSDMXToJsonStatConverterConfig annotationConfig)
        {
            return JsonConvert.DeserializeObject<SdmxJson>(json, SdmxJsonConverter.GetSettings(annotationConfig));
        }

    }
}