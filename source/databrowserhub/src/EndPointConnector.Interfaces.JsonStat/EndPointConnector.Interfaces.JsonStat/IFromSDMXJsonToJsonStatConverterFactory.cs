using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;

namespace EndPointConnector.Interfaces.JsonStat
{
    public interface IFromSdmxJsonToJsonStatConverterFactory
    {
        public static readonly string DefaultLanguage = "en";

        public IToJsonStatConverter GetConverter(string json, string lang, IFromSDMXToJsonStatConverterConfig config, IDataStructureObject dataStructure);
    }
}