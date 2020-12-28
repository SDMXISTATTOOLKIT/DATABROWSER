using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace EndPointConnector.JsonStatParser.Model.JsonStat.Extensions
{
    public class DatasetAttributes
    {

        [JsonProperty("dataSet", NullValueHandling = NullValueHandling.Ignore)]
        public List<Attribute> DataSet { get; set; }

        [JsonProperty("series", NullValueHandling = NullValueHandling.Ignore)]
        public List<Attribute> Series { get; set; }

        [JsonProperty("observation", NullValueHandling = NullValueHandling.Ignore)]
        public List<Attribute> Observation { get; set; }

        [JsonProperty("index", NullValueHandling = NullValueHandling.Ignore)]
        public AttributesIndex Index { get; set; }

        public DatasetAttributes()
        {
            Observation = new List<Attribute>();
            Series = new List<Attribute>();
            DataSet = new List<Attribute>();
            Index = new AttributesIndex();
        }

        internal void AddSeriesAttributeIndex(int?[] seriesCoordinates, List<int?> attributesCoordinates,
            List<string> dimensionIds)
        {
            if (attributesCoordinates == null || attributesCoordinates.Count == 0) {
                return;
            }

            var emptyDimensionIndices = Enumerable.Range(0, seriesCoordinates.Length)
                .Where(i => seriesCoordinates[i] == null).ToList();

            var attributeIndexCoords = new Dictionary<int?[], List<int?>>();

            for (var i = 0; i < attributesCoordinates.Count; i++) {
                var attributePosition = i; //i-simo attributo
                var attributeValuePosition =
                    attributesCoordinates[attributePosition]; //posizione del valore per l'i-simo attributo

                // l'i-simo attributo non e' assegnato a questa serie oppure se l'attributo non contiene valori
                if (!AreSeriesCoordinatesValid(attributePosition, attributeValuePosition)) {
                    //setta a null per tutti gli indici
                    foreach (var entry in attributeIndexCoords) entry.Value[attributePosition] = null;

                    continue;
                }

                //controlla che l'attributo non valga solo per un particolare gruppo di dimensioni della serie
                var fixedCoordinates =
                    GenerateCorrectAttributeIndex(seriesCoordinates, attributePosition, dimensionIds);

                if (fixedCoordinates == null) //caso base: l'attributo vale per tutte le dimensioni della serie
                {
                    if (!attributeIndexCoords.ContainsKey(seriesCoordinates)) {
                        attributeIndexCoords[seriesCoordinates] = Enumerable.Repeat((int?) null, Series.Count).ToList();
                    }

                    attributeIndexCoords[seriesCoordinates][attributePosition] = attributeValuePosition;
                }
                else //caso particolare: l'attributo vale soltanto per un sottoinsieme di dimensioni della serie
                {
                    if (!attributeIndexCoords.ContainsKey(fixedCoordinates)) {
                        attributeIndexCoords[fixedCoordinates] = Enumerable.Repeat((int?) null, Series.Count).ToList();
                    }

                    attributeIndexCoords[fixedCoordinates][attributePosition] = attributeValuePosition;
                }
            }

            foreach (var (key, value) in attributeIndexCoords) {
                var index = key;

                if (emptyDimensionIndices.Count > 0) {
                    var fixedIndex = index.ToList();
                    for (var i = 0; i < emptyDimensionIndices.Count; i++)
                        index.ToList().RemoveAt(emptyDimensionIndices[i] - i);
                    index = fixedIndex.ToArray();
                }

                Index.AddSeriesAttributeIndex(index, value);
            }
        }


        private int?[] GenerateCorrectAttributeIndex(int?[] seriesCoordinates, int attributePosition,
            List<string> dimensionIds)
        {
            var attr = Series[attributePosition];

            //has dimension group
            if (attr.Relationship == null || !attr.Relationship.ContainsKey("dimensions")) {
                return null;
            }

            var relationShipDimension = (string[]) attr.Relationship["dimensions"];
            var dimensionPos = relationShipDimension.Select(dimId => dimensionIds.FindIndex(dim => dim == dimId))
                .ToList();
            var fixedCoordinates = Enumerable.Repeat((int?) null, seriesCoordinates.Length).ToArray();
            dimensionPos.ForEach(dimPos => fixedCoordinates[dimPos] = seriesCoordinates[dimPos]);

            //var translatedDimIds = _jsonStatDataset.CoordinatesToDimensionValueIds(dimensionCoordinatesWithoutTimePeriod);
            return fixedCoordinates;
        }


        private bool AreSeriesCoordinatesValid(int dimensionPosition, int? attributePosition)
        {
            if (Series == null || Series.Count == 0 || attributePosition == null || attributePosition < 0) {
                return false;
            }

            if (dimensionPosition < 0 || dimensionPosition >= Series.Count ||
                Series[dimensionPosition]?.Values?.Count == 0) {
                return false;
            }

            var attributeItems = Series[dimensionPosition].Values;

            if (attributeItems != null && attributeItems.Count <= attributePosition.Value) {
                return false;
            }

            var values = Series[dimensionPosition].Values;

            return values == null || values[attributePosition.Value] != null;
        }

    }
}