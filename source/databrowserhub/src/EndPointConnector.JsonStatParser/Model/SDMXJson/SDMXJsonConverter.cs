using System.Globalization;
using EndPointConnector.Interfaces.JsonStat;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EndPointConnector.JsonStatParser.Model.SdmxJson
{
    internal static class SdmxJsonConverter
    {

        public static JsonSerializerSettings GetSettings(IFromSDMXToJsonStatConverterConfig config)
        {
            return new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                DateParseHandling = DateParseHandling.None,
                Converters =
                {
                    ObservationValueConverter.Singleton,
                    AnnotationAndAttributeTypeEnumConverter.GetNewInstance(config),
                    new IsoDateTimeConverter {DateTimeStyles = DateTimeStyles.AssumeUniversal}
                }
            };
        }

    }
}