using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace EndPointConnector.Models
{
    public class FilterCriteria
    {
        public string Id { get; set; }
        public List<string> FilterValues { get; set; }
        public FilterType Type { get; set; }
        public int Period { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FilterType { CodeValues, TimeRange, TimePeriod, StringValues }
}