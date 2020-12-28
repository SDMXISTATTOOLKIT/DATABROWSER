using System.Collections.Generic;
using System.Xml;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;

namespace EndPointConnector.Interfaces.Sdmx.Models
{
    public class XmlDataContainer
    {
        public XmlDocument XmlDocument { get; set; }
        public ISet<ICodelistObject> Codelists { get; set; }
        public ISet<IConceptSchemeObject> ConceptSchemes { get; set; }
        public ISet<IDataStructureObject> DataStructures { get; set; }
    }
}