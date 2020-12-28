using System.Collections.Generic;

namespace EndPointConnector.Models
{
    public class Codelist : MainObject
    {
        public Dictionary<string, List<string>> DefaultCodeSelected { get; set; }
        public List<Code> Items { get; set; }
    }
}