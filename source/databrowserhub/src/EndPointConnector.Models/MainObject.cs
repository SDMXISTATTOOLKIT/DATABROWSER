using System.Collections.Generic;

namespace EndPointConnector.Models
{
    public abstract class MainObject
    {
        public string Id { get; set; }
        public Dictionary<string, string> Names { get; set; }
        public Dictionary<string, string> Descriptions { get; set; }
        public List<ExtraValue> Extras { get; set; }
    }
}