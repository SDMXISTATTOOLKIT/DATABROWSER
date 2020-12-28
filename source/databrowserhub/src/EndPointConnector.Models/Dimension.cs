using System.Collections.Generic;

namespace EndPointConnector.Models
{
    public class Dimension
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public Dictionary<string, string> Labels { get; set; }
        public ArtefactRef ConceptRef { get; set; }
        public ArtefactRef Representation { get; set; }
        public DimensionType Type { get; set; }
    }
}