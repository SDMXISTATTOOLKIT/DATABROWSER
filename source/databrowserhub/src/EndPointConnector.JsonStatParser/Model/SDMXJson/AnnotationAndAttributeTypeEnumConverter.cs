using System;
using EndPointConnector.Interfaces.JsonStat;
using Newtonsoft.Json;

namespace EndPointConnector.JsonStatParser.Model.SdmxJson
{
    public class AnnotationAndAttributeTypeEnumConverter : JsonConverter
    {

        // singleton
        private readonly IFromSDMXToJsonStatConverterConfig _instanceConfig;

        private AnnotationAndAttributeTypeEnumConverter(IFromSDMXToJsonStatConverterConfig config = null)
        {
            _instanceConfig = config ?? DefaultJsonStatConverterConfig.GetNew();
        }

        public static AnnotationAndAttributeTypeEnumConverter GetNewInstance(IFromSDMXToJsonStatConverterConfig config)
        {
            return new AnnotationAndAttributeTypeEnumConverter(config);
        }

        public override bool CanConvert(Type t)
        {
            return t == typeof(TypeEnum) || t == typeof(TypeEnum?);
        }

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) {
                return null;
            }

            var value = serializer.Deserialize<string>(reader)?.ToLower();

            switch (value) {
                case "associatedcube":
                    return TypeEnum.AssociatedCube;
                case "ddbdataflow":
                    return TypeEnum.DdbDataflow;
            }

            if (_instanceConfig.IsValidOrderAnnotation(value)) {
                return TypeEnum.Order;
            }

            return _instanceConfig.IsValidNotDisplayedAnnotation(value) ? TypeEnum.NotDisplayed : TypeEnum.Unknown;

            //throw new Exception("Cannot unmarshal type TypeEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null) {
                serializer.Serialize(writer, null);

                return;
            }

            var value = (TypeEnum) untypedValue;

            switch (value) {
                case TypeEnum.AssociatedCube:
                    serializer.Serialize(writer, "AssociatedCube");

                    return;
                case TypeEnum.DdbDataflow:
                    serializer.Serialize(writer, "DDBDataflow");

                    return;
                case TypeEnum.Order:
                    serializer.Serialize(writer, GetOrderAnnotationId());

                    return;
                case TypeEnum.Unknown:
                    return;
                case TypeEnum.NotDisplayed:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            throw new Exception("Cannot marshal type TypeEnum");
        }

        private string GetOrderAnnotationId()
        {
            return _instanceConfig.OrderAnnotationId ?? DefaultJsonStatConverterConfig.GetNew().OrderAnnotationId;
        }

    }
}