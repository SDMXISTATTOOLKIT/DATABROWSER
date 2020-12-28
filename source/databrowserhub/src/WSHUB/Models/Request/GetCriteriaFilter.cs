using System.Collections.Generic;

namespace WSHUB.Models.Request
{
    public class GetCriteriaFilter
    {
        public string DataflowId { get; set; }
        public string DataflowAgency { get; set; }
        public string DataflowVersion { get; set; }
        public Dictionary<string, List<string>> Filters { get; set; }
    }
}