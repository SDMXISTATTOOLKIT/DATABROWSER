using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace EndPointConnector.JsonStatParser.Model.JsonStat
{
    internal class DimensionIndexToSortedArrayConverter : JsonConverter
    {

        private static Dictionary<string, int> ReadValuesArray(JsonReader reader, JsonSerializer serializer)
        {
            var positionCounter = 0;
            var result = new Dictionary<string, int>();

            reader.Read(); // move to next position

            while (reader.TokenType != JsonToken.EndArray) {
                switch (reader.TokenType) {
                    case JsonToken.String: {
                        var dimensionId = (string) reader.Value;
                        result[dimensionId ?? string.Empty] = positionCounter;
                        positionCounter++;

                        break;
                    }
                    case JsonToken.Date: {
                        var dimensionId = serializer.Deserialize(reader)?.ToString();
                        result[dimensionId ?? string.Empty] = positionCounter;
                        positionCounter++;

                        break;
                    }
                    default:
                        throw new Exception("Cannot unmarshal dimension ID");
                }

                reader.Read(); // move to next position
            }

            return result;
        }

        public override bool CanConvert(Type t)
        {
            return t == typeof(Dictionary<string, int>);
        }


        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType) {
                case JsonToken.Null:
                    return null;
                case JsonToken.String:
                    return serializer.Deserialize<string>(reader);
                //conversione da ['id1', 'id2'...] a  {"id1": 0, "id2": 1, ...}
                case JsonToken.StartArray: {
                    var result = ReadValuesArray(reader, serializer);

                    return result;
                }
                case JsonToken.StartObject: {
                    var val = serializer.Deserialize<Dictionary<string, int>>(reader);

                    if (val == null || val.Count == 0) {
                        return null;
                    }

                    return val;
                }
                default:
                    throw new Exception("Cannot unmarshal type name to string");
            }
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null) {
                serializer.Serialize(writer, null);

                return;
            }

            var value = (Dictionary<string, int>) untypedValue;

            //conversione da {"id1": 0, "id2": 1, ...} a ['id1', 'id2'...]
            var convertedValue = value.OrderBy(entry => entry.Value).Select(entry => entry.Key).ToList();

            serializer.Serialize(writer, convertedValue);
        }

    }
}