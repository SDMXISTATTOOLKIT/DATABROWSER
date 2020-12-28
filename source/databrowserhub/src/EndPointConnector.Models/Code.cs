using System.Collections.Generic;

namespace EndPointConnector.Models
{
    public class Code
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public Dictionary<string, bool> IsDefault { get; set; }
        public Dictionary<string, string> Names { get; set; }
        public bool? IsSelectable { get; set; }
        public bool? IsUnSelectable { get; set; }
    }
}