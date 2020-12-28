using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace EndPointConnector.JsonStatParser.Model.SdmxJson
{
    public class ObservationIndexConverter : JsonConverter
    {

        public override bool CanConvert(Type t)
        {
            return t == typeof(Dictionary<string, string>);
        }

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType) {
                case JsonToken.Null: {
                    return null;
                }
                case JsonToken.String: {
                    return serializer.Deserialize<string>(reader);
                }
                case JsonToken.StartObject: {
                    var names = serializer.Deserialize<Dictionary<string, string>>(reader);

                    if (names == null || names.Count == 0) {
                        return null;
                    }

                    return names.First().Value;
                }
                case JsonToken.None: {
                    return null;
                }
                case JsonToken.StartArray: {
                    return null;
                }
                case JsonToken.StartConstructor: {
                    return null;
                }
                case JsonToken.PropertyName: {
                    return null;
                }
                case JsonToken.Comment: {
                    return null;
                }
                case JsonToken.Raw: {
                    return null;
                }
                case JsonToken.Integer: {
                    return null;
                }
                case JsonToken.Float: {
                    return null;
                }
                case JsonToken.Boolean: {
                    return null;
                }
                case JsonToken.Undefined: {
                    return null;
                }
                case JsonToken.EndObject: {
                    return null;
                }
                case JsonToken.EndArray: {
                    return null;
                }
                case JsonToken.EndConstructor: {
                    return null;
                }
                case JsonToken.Date: {
                    return null;
                }
                case JsonToken.Bytes: {
                    return null;
                }
                default: {
                    throw new Exception("Cannot unmarshal type name to string");
                }
            }
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null) {
                serializer.Serialize(writer, null);

                return;
            }

            var value = (long) untypedValue;
            serializer.Serialize(writer, value.ToString());
        }

    }
}