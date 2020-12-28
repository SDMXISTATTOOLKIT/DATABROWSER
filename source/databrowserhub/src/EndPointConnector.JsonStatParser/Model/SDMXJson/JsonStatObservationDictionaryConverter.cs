using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace EndPointConnector.JsonStatParser.Model.SdmxJson
{
    public class JsonStatObservationDictionaryConverter : JsonConverter
    {

        public override bool CanWrite => true;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<int, List<ObservationValue>>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            switch (reader.TokenType) {
                case JsonToken.StartArray: {
                    reader.Read();

                    if (reader.TokenType == JsonToken.EndArray) {
                        return new Dictionary<int, ObservationValue>();
                    }

                    throw new JsonSerializationException("Non-empty JSON array does not make a valid Dictionary!");
                }
                case JsonToken.Null:
                    return null;
            }

            if (reader.TokenType != JsonToken.StartObject) {
                throw new JsonSerializationException("Unexpected token!");
            }

            var ret = new Dictionary<int, ObservationValue>();
            reader.Read();

            while (reader.TokenType != JsonToken.EndObject) {
                if (reader.TokenType != JsonToken.PropertyName) {
                    throw new JsonSerializationException("Unexpected token");
                }

                var keyString = (string) reader.Value;
                var key = int.Parse(keyString ?? string.Empty);
                reader.Read();

                switch (reader.TokenType) {
                    case JsonToken.Float:
                    case JsonToken.Integer:
                    case JsonToken.String: {
                        var value = serializer.Deserialize<ObservationValue>(reader);
                        ret.Add(key, value);

                        break;
                    }
                    case JsonToken.Null:
                        ret.Add(key, new ObservationValue());

                        break;
                    default:
                        throw new JsonSerializationException("Unexpected token");
                }

                reader.Read();
            }

            return ret;
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null) {
                serializer.Serialize(writer, null);

                return;
            }

            var valueMap = (Dictionary<int, ObservationValue>) untypedValue;

            writer.WriteStartObject();

            foreach (var (key, value) in valueMap) {
                writer.WritePropertyName("" + key);

                if (value.IsNull) {
                    writer.WriteNull();
                }

                if (value.Double != null) {
                    writer.WriteValue(value.Double);
                }

                if (value.String != null) {
                    writer.WriteValue(value.String);
                }
            }

            writer.WriteEndObject();
        }

    }
}