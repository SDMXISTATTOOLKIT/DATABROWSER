using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EndPointConnector.JsonStatParser.Model.SdmxJson
{
    public class RelationshipDictionaryConverter : JsonConverter
    {

        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<string, object>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject) {
                throw new JsonSerializationException("Unexpected token!");
            }

            var ret = new Dictionary<string, object>();
            reader.Read();

            while (reader.TokenType != JsonToken.EndObject) {
                if (reader.TokenType != JsonToken.PropertyName) {
                    throw new JsonSerializationException("Unexpected token");
                }

                var keyString = (string) reader.Value;

                switch (keyString) {
                    case "none": {
                        reader.Skip();

                        break;
                    }
                    case "dimensions": {
                        reader.Read();
                        var val = serializer.Deserialize(reader);

                        if (val != null && val.GetType() == typeof(JArray)) {
                            ret[keyString] = ((JArray) val).ToObject<string[]>();
                        }

                        break;
                    }
                    default: {
                        reader.Read();
                        ret[keyString ?? ""] = serializer.Deserialize(reader);

                        break;
                    }
                }

                reader.Read();
            }

            return ret.Count > 0 ? ret : null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

    }
}