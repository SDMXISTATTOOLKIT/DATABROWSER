using System.Collections.Generic;
using EndPointConnector.Models;

namespace EndPointConnector.Interfaces.JsonStat
{
    public interface IFromJsonStatToJsonStatConverterFactory
    {
        public IToJsonStatConverter GetConverter(string json, List<Criteria> notDisplayed, List<FilterCriteria> dataCriterias, string lang);
    }
}