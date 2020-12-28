using System.Collections.Generic;

namespace DataBrowser.Interfaces.Dto.UseCases.Responses
{
    public class GetDataFromDataflowResponse
    {
        public string JsonData { get; set; }
        public long? ItemsCount { get; set; }
        public long? ItemsFrom { get; set; }
        public long? ItemsTo { get; set; }
        public long? ItemsMax { get; set; }
        public bool LimitExceeded { get; set; }

        public Dictionary<string, string> Timers { get; set; }
    }
}