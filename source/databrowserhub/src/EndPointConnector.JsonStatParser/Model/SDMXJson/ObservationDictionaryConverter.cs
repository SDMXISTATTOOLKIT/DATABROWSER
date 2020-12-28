using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace EndPointConnector.JsonStatParser.Model.SdmxJson
{
    public class ObservationDictionaryConverter : JsonConverter
    {

        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<List<int>, List<ObservationValue>>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            switch (reader.TokenType) {
                case JsonToken.StartArray: {
                    reader.Read();

                    if (reader.TokenType == JsonToken.EndArray) {
                        return new Dictionary<List<int>, List<ObservationValue>>();
                    }

                    throw new JsonSerializationException("Non-empty JSON array does not make a valid Dictionary!");
                }
                case JsonToken.Null:
                    return null;
            }

            if (reader.TokenType != JsonToken.StartObject) {
                throw new JsonSerializationException("Unexpected token!");
            }

            var ret = new Dictionary<List<int>, List<ObservationValue>>();
            reader.Read();

            while (reader.TokenType != JsonToken.EndObject) {
                if (reader.TokenType != JsonToken.PropertyName) {
                    throw new JsonSerializationException("Unexpected token");
                }

                var keyString = (string) reader.Value;

                if (keyString != null) {
                    var key = new List<int>(Array.ConvertAll(keyString.Split(':'), int.Parse));
                    reader.Read();

                    if (reader.TokenType != JsonToken.StartArray) {
                        throw new JsonSerializationException("Unexpected token");
                    }

                    var value = serializer.Deserialize<List<ObservationValue>>(reader);

                    ret.Add(key, value);
                }

                reader.Read();
            }

            return ret;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

    }
}