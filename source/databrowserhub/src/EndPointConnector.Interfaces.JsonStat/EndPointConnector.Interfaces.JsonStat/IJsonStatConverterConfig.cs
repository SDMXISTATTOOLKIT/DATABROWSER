using System.Collections.Generic;

namespace EndPointConnector.Interfaces.JsonStat
{
    public interface IJsonStatConverterConfig
    {
        List<string> TerritorialDimensionIds { get; set; }
        List<string> TemporalDimensionIds { get; set; }

        public abstract bool ContainsTerritorialDimensionId(string id);

        public abstract bool ContainsTemporalDimensionId(string id);

        public void AddTemporalDimensionId(string timeIdLabel);

        public void AddTerritorialDimensionId(string terrLabel);

    }
}