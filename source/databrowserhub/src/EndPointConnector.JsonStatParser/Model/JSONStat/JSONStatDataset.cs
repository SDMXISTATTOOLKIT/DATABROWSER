using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EndPointConnector.JsonStatParser.Model.JsonStat.Extensions;
using EndPointConnector.JsonStatParser.Model.SdmxJson;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EndPointConnector.JsonStatParser.Model.JsonStat
{
    public class JsonStatDataset : JsonStat
    {

        [JsonProperty("id")] public List<string> Id { get; set; } = new List<string>();

        [JsonProperty("size")] public List<int> Size { get; set; } = new List<int>();

        [JsonProperty("role")] public JsonStatRole Role { get; set; } = new JsonStatRole();

        [JsonProperty("status")] public Dictionary<int, string> Status { get; set; } = new Dictionary<int, string>();

        [JsonProperty("value")]
        [JsonConverter(typeof(JsonStatObservationDictionaryConverter))]
        public Dictionary<int, ObservationValue> Value { get; set; } = new Dictionary<int, ObservationValue>();

        [JsonProperty("extension")]
        public JsonStatCustomDatasetExtension Extension { get; set; } = new JsonStatCustomDatasetExtension();

        [JsonProperty("dimension")]
        internal Dictionary<string, JsonStatDimension> Dimension { get; set; } =
            new Dictionary<string, JsonStatDimension>();


        public static JsonStatDataset Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<JsonStatDataset>(json, new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                DateParseHandling = DateParseHandling.None,
                Converters =
                {
                    new IsoDateTimeConverter {DateTimeStyles = DateTimeStyles.AssumeUniversal}
                }
            });
        }


        public static string Serialize(JsonStatDataset instance)
        {
            return JsonConvert.SerializeObject(instance, Formatting.None);
        }


        public void Clear()
        {
            Id = new List<string>();
            Size = new List<int>();
            Role = new JsonStatRole();
            Dimension = new Dictionary<string, JsonStatDimension>();
            Status = new Dictionary<int, string>();
            Value = new Dictionary<int, ObservationValue>();
            Extension = new JsonStatCustomDatasetExtension();
        }


        public void RemoveEmptyDimensions(HashSet<string> hiddenDimensions = null)
        {
            var removingIndex = Size
                .Select((dimSize, index) => (dimSize, index))
                .Where(val => val.dimSize == 0 || hiddenDimensions != null && val.dimSize == 1 &&
                    hiddenDimensions.Contains(Id[val.index])) //empty or banned with size == 1
                .Select(val => val.index)
                .ToList();

            for (var i = 0; i < removingIndex.Count; i++) {
                var dimensionId = Id[removingIndex[i] - i];
                Size.RemoveAt(removingIndex[i] - i);
                Id.RemoveAt(removingIndex[i] - i);
                Dimension.Remove(dimensionId);
                Extension.Attributes.Index.RemoveSeriesDimensionReference(removingIndex[i] - i);
            }
        }


        public int?[] PositionToCoordinates(int observationPosition, Dictionary<int, HashSet<int>> bannedCodesPositions)
        {
            if (Size == null || Size.Count == 0) {
                return null;
            }

            var coordinates = new int?[Size.Count];

            // reverse row-major position algorithm
            var numberOfDimensions = Size.Count;
            var offset = observationPosition;

            for (var i = numberOfDimensions - 1; i >= 0; i--) {
                var dimensionSize = Size[i];
                var val = dimensionSize > 0 ? offset % dimensionSize : 0;

                if (bannedCodesPositions.ContainsKey(i) && bannedCodesPositions[i].Contains(val)) // code is banned
                {
                    return null;
                }

                coordinates[i] = val;
                offset = dimensionSize > 0 ? offset / dimensionSize : 0;
            }

            return coordinates;
        }

        public int DimensionPosition(string dimensionId)
        {
            return Id.FindIndex(id => id == dimensionId);
        }

        public int DimensionSize(string dimensionId)
        {
            var dimensionPosition = DimensionPosition(dimensionId);

            if (dimensionPosition > 0) {
                return Size[dimensionPosition];
            }

            return -1;
        }

        public void AddObservationValue(int position, ObservationValue value)
        {
            Value[position] = value;
        }

    }
}