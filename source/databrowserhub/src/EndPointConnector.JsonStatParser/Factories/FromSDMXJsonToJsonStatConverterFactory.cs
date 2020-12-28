using EndPointConnector.Interfaces.JsonStat;
using EndPointConnector.JsonStatParser.Adapters.SdmxJsonAdapters;
using EndPointConnector.JsonStatParser.Converters;
using EndPointConnector.JsonStatParser.Model.SdmxJson;
using Microsoft.Extensions.Logging;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;

namespace EndPointConnector.JsonStatParser.Factories
{
    public class FromSdmxJsonToJsonStatConverterFactory : IFromSdmxJsonToJsonStatConverterFactory
    {

        private readonly ILoggerFactory _loggerFactory;

        public FromSdmxJsonToJsonStatConverterFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public IToJsonStatConverter GetConverter(string sdmxJsonText, string lang,
            IFromSDMXToJsonStatConverterConfig config, IDataStructureObject dataStructure)
        {
            var sdmxJson = SdmxJson.FromJson(sdmxJsonText, config);
            var datasetStructure = new SdmxJsonDatasetStructureAdapter(sdmxJson, config, dataStructure);
            var datasetObservations = new SdmxJsonObservationsAdapter(sdmxJson, lang);

            return new ToJsonStatConverter(_loggerFactory, lang, datasetStructure, datasetObservations, config);
        }

    }
}