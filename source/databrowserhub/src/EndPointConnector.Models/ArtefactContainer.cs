using System.Collections.Generic;

namespace EndPointConnector.Models
{
    public class ArtefactContainer
    {
        public List<Codelist> Codelists { get; set; }
        public List<Dataflow> Dataflows { get; set; }
        public List<Dsd> Dsds { get; set; }
        public List<Criteria> Criterias { get; set; }
        public int? ObsCount { get; set; }
        public List<ConceptScheme> ConceptSchemes { get; set; }
    }
}