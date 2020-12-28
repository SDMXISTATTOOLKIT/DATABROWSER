using System;
using System.Collections.Generic;
using System.Linq;
using EndPointConnector.JsonStatParser.StructureUtils.Sorting.WeightGenerators;
using Newtonsoft.Json;

namespace EndPointConnector.JsonStatParser.Model.JsonStat
{
    public class JsonStatDimensionCategory
    {

        [JsonProperty("index")]
        [JsonConverter(typeof(DimensionIndexToSortedArrayConverter))]
        [field: JsonIgnore]
        public Dictionary<string, int> Index { get; set; } = new Dictionary<string, int>();

        [JsonProperty("label")]
        [field: JsonIgnore]
        public Dictionary<string, string> Label { get; set; } = new Dictionary<string, string>();

        [JsonIgnore] public int Count => Index.Count;

        [JsonIgnore] public string[] InverseIndex;

        [JsonIgnore]
        private readonly Dictionary<int, double?> _originalPositionToWeight = new Dictionary<int, double?>();

        [JsonIgnore] private readonly List<(string c, double? w)> _sortedCodes = new List<(string c, double? w)>();

        [JsonIgnore] private readonly Dictionary<double?, int> _weightToPosition = new Dictionary<double?, int>();


        public void Clear()
        {
            Index.Clear();
            Label.Clear();
            _weightToPosition.Clear();
            _originalPositionToWeight.Clear();
            _sortedCodes.Clear();
            InverseIndex = null;
        }


        public double? AddLabel(string label, string code, IWeightGenerator weightGenerator,
            int? originalPosition = null)
        {
            if (Index.ContainsKey(code) || string.IsNullOrEmpty(code)) {
                return weightGenerator.GenerateWeight(code);
            }

            var codeWeight = weightGenerator.GenerateWeight(code);

            if (codeWeight == null) {
                return null;
            }

            if (_weightToPosition.ContainsKey(codeWeight)) {
                throw new Exception(
                    $"Duplicate weight {codeWeight} for codes '{code}' and '{_sortedCodes[_weightToPosition[codeWeight]]}'");
            }

            originalPosition ??= _originalPositionToWeight.Count;
            _originalPositionToWeight[originalPosition.Value] = codeWeight;

            var labelPosition = _sortedCodes.Count;
            Index[code] = labelPosition;
            Label[code] = label;
            _weightToPosition[codeWeight] = labelPosition;
            _sortedCodes.Add((c: code, w: codeWeight));

            return codeWeight;
        }


        public void SortByWeight()
        {
            //sort by weight
            _sortedCodes.Sort((elem1, elem2) =>
            {
                var aW = elem1.w;
                var bW = elem2.w;

                if (aW < 0 && bW < 0) {
                    return aW >= bW ? -1 : 1;
                }

                return aW <= bW ? -1 : 1;
            });

            //fix all reference
            for (var i = 0; i < _sortedCodes.Count; i++) {
                var code = _sortedCodes[i].c;
                var weight = _sortedCodes[i].w;
                var sortedPosition = i;
                Index[code] = sortedPosition;
                _weightToPosition[weight ?? -1] = sortedPosition;
            }
        }


        public int PositionByWeight(double? weight)
        {
            return _weightToPosition[weight ?? -1];
        }


        public string CodeByPosition(int position)
        {
            if (position < 0 || position >= Index.Count) {
                return null;
            }

            InverseIndex ??= Index.OrderBy(entry => entry.Value).Select(entry => entry.Key).ToArray();

            return InverseIndex[position];
        }

    }
}