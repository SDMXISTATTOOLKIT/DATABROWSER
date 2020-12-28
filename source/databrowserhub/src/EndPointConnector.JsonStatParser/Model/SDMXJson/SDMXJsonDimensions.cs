using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace EndPointConnector.JsonStatParser.Model.SdmxJson
{
    public class SdmxJsonDimensions
    {

        [JsonProperty("dataset", NullValueHandling = NullValueHandling.Ignore)]
        public List<object> Dataset { get; set; }

        [JsonProperty("series", NullValueHandling = NullValueHandling.Ignore)]
        public List<SdmxJsonDimension> Series { get; set; }

        [JsonProperty("observation", NullValueHandling = NullValueHandling.Ignore)]
        public List<SdmxJsonDimension> Observation { get; set; }

        [JsonProperty("parent", NullValueHandling = NullValueHandling.Ignore)]
        public string Parent { get; set; }

        [JsonProperty("start", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? Start { get; set; }

        [JsonProperty("end", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? End { get; set; }

        public bool HasSeries => Series.Count > 0;

        public SdmxJsonDimension GetDimensionById(string dimensionId)
        {
            if (!HasSeries) {
                return Observation.FirstOrDefault(x => x.Id == dimensionId);
            }

            // value may be in Series or in Observation array (if it's the time period dimension)
            var result = Series.FirstOrDefault(x => x.Id == dimensionId);

            return result ?? Observation.FirstOrDefault(x => x.Id == dimensionId);
        }

        public SdmxJsonDimension GetDimensionByPosition(int position)
        {
            // value may be in Series or in Observation array (if it's the time period dimension)

            if (!HasSeries) {
                return Observation[position];
            }

            try {
                return position < Series.Count ? Series[position] : Observation[position - Series.Count - 1];
            }
            catch (Exception) {
                return Observation[position];
            }
        }

    }
}