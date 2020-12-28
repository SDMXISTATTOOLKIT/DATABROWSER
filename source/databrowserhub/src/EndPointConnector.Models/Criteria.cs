using System.Collections.Generic;

namespace EndPointConnector.Models
{
    public class Criteria
    {
        public string Id { get; set; }
        public Dictionary<string, string> Titles { get; set; }
        public List<Code> Values { get; set; }
        public ArtefactRef DataStructureRef { get; set; }
    }
}