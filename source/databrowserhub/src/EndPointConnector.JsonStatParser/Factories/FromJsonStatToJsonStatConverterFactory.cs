using System.Collections.Generic;
using EndPointConnector.Interfaces.JsonStat;
using EndPointConnector.JsonStatParser.Converters;
using EndPointConnector.Models;
using Microsoft.Extensions.Logging;

namespace EndPointConnector.JsonStatParser.Factories
{
    public class FromJsonStatToJsonStatConverterFactory : IFromJsonStatToJsonStatConverterFactory
    {

        private readonly ILoggerFactory _loggerFactory;

        public FromJsonStatToJsonStatConverterFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public IToJsonStatConverter GetConverter(string json, List<Criteria> notDisplayed,
            List<FilterCriteria> dataCriterias, string lang)
        {
            return new FromJsonStatToJsonStatConverter(_loggerFactory, json, notDisplayed, dataCriterias, lang);
        }

    }
}