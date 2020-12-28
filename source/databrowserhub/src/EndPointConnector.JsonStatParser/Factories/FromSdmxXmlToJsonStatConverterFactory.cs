using System.Collections.Generic;
using System.Xml;
using EndPointConnector.Interfaces.JsonStat;
using EndPointConnector.JsonStatParser.Adapters.SdmxXmlAdapters;
using EndPointConnector.JsonStatParser.Converters;
using Microsoft.Extensions.Logging;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;

namespace EndPointConnector.JsonStatParser.Factories
{
    public class FromSdmxXmlToJsonStatConverterFactory : IFromSdmxXmlToJsonStatConverterFactory
    {

        private readonly ILoggerFactory _loggerFactory;

        public FromSdmxXmlToJsonStatConverterFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public IToJsonStatConverter GetConverter(XmlDocument xmlDocument, IDataflowObject dataflow,
            IDataStructureObject dataStructure,
            ISet<ICodelistObject> codelists, ISet<IConceptSchemeObject> conceptSchemes, string lang,
            IFromSDMXToJsonStatConverterConfig config)
        {
            var datasetStructure = new SdmxXmlDatasetStructureAdapter(dataflow, dataStructure, codelists,
                conceptSchemes, config, lang);

            var datasetObservations =
                new SdmxXmlObservationsAdapter(xmlDocument, dataflow, dataStructure, codelists, conceptSchemes, lang);

            return new ToJsonStatConverter(_loggerFactory, lang, datasetStructure, datasetObservations, config);

            //return new FromSDMXToJsonStatConverter(_loggerFactory, xmlDocument, dataflow,dataStructure,codelists,conceptSchemes, lang);
        }

    }
}