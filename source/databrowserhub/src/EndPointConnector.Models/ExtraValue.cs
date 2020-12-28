using System.Collections.Generic;

namespace EndPointConnector.Models
{
    public class ExtraValue
    {
        public string Key { get; set; }
        public string Type { get; set; }
        public Dictionary<string, string> Values { get; set; }
        public bool IsPublic { get; set; }
    }
}