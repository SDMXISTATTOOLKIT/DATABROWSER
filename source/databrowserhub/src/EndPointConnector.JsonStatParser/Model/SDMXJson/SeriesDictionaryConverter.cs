using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace EndPointConnector.JsonStatParser.Model.SdmxJson
{
    public class SeriesDictionaryConverter : JsonConverter
    {

        public override bool CanWrite => false;

        private static Dictionary<List<int>, SeriesValue> ReadSeriesValues(JsonReader reader, JsonSerializer serializer)
        {
            var result = new Dictionary<List<int>, SeriesValue>();
            reader.Read();

            while (reader.TokenType != JsonToken.EndObject) {
                if (reader.TokenType != JsonToken.PropertyName) {
                    throw new JsonSerializationException("Unexpected token");
                }

                var keyString = (string) reader.Value;

                if (keyString != null) {
                    var key = new List<int>(Array.ConvertAll(keyString.Split(':'), int.Parse));
                    reader.Read();

                    if (reader.TokenType != JsonToken.StartObject) {
                        throw new JsonSerializationException("Unexpected token. Expected object for series");
                    }

                    var value = serializer.Deserialize<SeriesValue>(reader);
                    result.Add(key, value);
                }

                reader.Read();
            }

            return result;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<List<int>, SeriesValue>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            return reader.TokenType switch
            {
                JsonToken.Null => null,
                JsonToken.StartObject => ReadSeriesValues(reader, serializer),
                JsonToken.None => ReadSeriesValues(reader, serializer),
                JsonToken.StartArray => ReadSeriesValues(reader, serializer),
                JsonToken.StartConstructor => ReadSeriesValues(reader, serializer),
                JsonToken.PropertyName => ReadSeriesValues(reader, serializer),
                JsonToken.Comment => ReadSeriesValues(reader, serializer),
                JsonToken.Raw => ReadSeriesValues(reader, serializer),
                JsonToken.Integer => ReadSeriesValues(reader, serializer),
                JsonToken.Float => ReadSeriesValues(reader, serializer),
                JsonToken.String => ReadSeriesValues(reader, serializer),
                JsonToken.Boolean => ReadSeriesValues(reader, serializer),
                JsonToken.Undefined => ReadSeriesValues(reader, serializer),
                JsonToken.EndObject => ReadSeriesValues(reader, serializer),
                JsonToken.EndArray => ReadSeriesValues(reader, serializer),
                JsonToken.EndConstructor => ReadSeriesValues(reader, serializer),
                JsonToken.Date => ReadSeriesValues(reader, serializer),
                JsonToken.Bytes => ReadSeriesValues(reader, serializer),
                _ => throw new JsonSerializationException("Unexpected token!")
            };
        }


        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

    }
}