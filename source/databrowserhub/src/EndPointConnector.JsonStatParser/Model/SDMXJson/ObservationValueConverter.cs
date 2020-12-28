using System;
using Newtonsoft.Json;

namespace EndPointConnector.JsonStatParser.Model.SdmxJson
{
    public class ObservationValueConverter : JsonConverter
    {

        public static readonly ObservationValueConverter Singleton = new ObservationValueConverter();

        public override bool CanConvert(Type t)
        {
            return t == typeof(ObservationValue) || t == typeof(ObservationValue?);
        }

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType) {
                case JsonToken.Null: {
                    return new ObservationValue();
                }
                case JsonToken.Integer:
                case JsonToken.Float: {
                    var doubleValue = serializer.Deserialize<double>(reader);

                    return new ObservationValue {Double = doubleValue};
                }
                case JsonToken.String:
                case JsonToken.Date: {
                    var stringValue = serializer.Deserialize<string>(reader);

                    return new ObservationValue {String = stringValue};
                }
                case JsonToken.None: {
                    break;
                }
                case JsonToken.StartObject: {
                    break;
                }
                case JsonToken.StartArray: {
                    break;
                }
                case JsonToken.StartConstructor: {
                    break;
                }
                case JsonToken.PropertyName: {
                    break;
                }
                case JsonToken.Comment: {
                    break;
                }
                case JsonToken.Raw: {
                    break;
                }
                case JsonToken.Boolean: {
                    break;
                }
                case JsonToken.Undefined: {
                    break;
                }
                case JsonToken.EndObject: {
                    break;
                }
                case JsonToken.EndArray: {
                    break;
                }
                case JsonToken.EndConstructor: {
                    break;
                }
                case JsonToken.Bytes: {
                    break;
                }
                default: {
                    throw new ArgumentOutOfRangeException();
                }
            }

            throw new Exception("Cannot unmarshal type ObservationValue");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (ObservationValue) untypedValue;

            if (value.IsNull) {
                serializer.Serialize(writer, null);

                return;
            }

            if (value.Double != null) {
                serializer.Serialize(writer, value.Double.Value);

                return;
            }

            if (value.String == null) {
                throw new Exception("Cannot marshal type ObservationValue");
            }

            serializer.Serialize(writer, value.String);
        }

    }
}