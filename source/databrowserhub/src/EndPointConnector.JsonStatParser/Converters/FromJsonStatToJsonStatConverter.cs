using System.Collections.Generic;
using EndPointConnector.Interfaces.JsonStat;
using EndPointConnector.JsonStatParser.Model.JsonStat;
using EndPointConnector.JsonStatParser.Model.JsonStat.ExtensionMethods;
using EndPointConnector.Models;
using Microsoft.Extensions.Logging;

namespace EndPointConnector.JsonStatParser.Converters
{
    internal class FromJsonStatToJsonStatConverter : IToJsonStatConverter
    {

        public string Lang { get; }

        private readonly List<FilterCriteria> _dataCriterias;

        private readonly string _json;

        private readonly ILogger _logger;

        private readonly List<Criteria> _notDisplayed;

        public FromJsonStatToJsonStatConverter(ILoggerFactory loggerFactory, string json, List<Criteria> notDisplayed,
            List<FilterCriteria> dataCriterias, string lang)
        {
            _json = json;
            _dataCriterias = dataCriterias;
            Lang = lang;
            _logger = loggerFactory.CreateLogger<FromJsonStatToJsonStatConverter>();

            _notDisplayed = notDisplayed;
        }

        public string Convert()
        {
            _logger.LogDebug("Filtering JSONStat by criteria - start");
            var jsonStatInstance = JsonStatDataset.Deserialize(_json);

            jsonStatInstance.Filter(_dataCriterias, _notDisplayed);

            _logger.LogDebug("Filtering JSONStat by criteria - end");

            return JsonStatDataset.Serialize(jsonStatInstance);
        }

    }
}