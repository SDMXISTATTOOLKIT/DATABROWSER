using System.Collections.Generic;
using System.Xml;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;

namespace EndPointConnector.Interfaces.JsonStat
{
    public interface IFromSdmxXmlToJsonStatConverterFactory
    {
        public IToJsonStatConverter GetConverter(XmlDocument xmlDocument, IDataflowObject dataflow,
            IDataStructureObject dataStructure,
            ISet<ICodelistObject> codelists, ISet<IConceptSchemeObject> conceptSchemes, string lang,
            IFromSDMXToJsonStatConverterConfig config);
    }
}