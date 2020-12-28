using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EndPointConnector.JsonStatParser.Adapters.Interfaces;
using EndPointConnector.JsonStatParser.Model.SdmxJson;

namespace EndPointConnector.JsonStatParser.Adapters.SdmxJsonAdapters
{
    internal class SdmxJsonObservationsAdapter : IObservationsAdapter
    {

        public HashSet<string>[] DistinctDimensionsCodes { get; private set; }

        public Dictionary<string[], IndexedObservation> Values { get; }

        public IAttributesAdapter Attributes { get; }

        private readonly SdmxJsonDataSet _dataset;

        private readonly SdmxJsonStructure _structure;


        public SdmxJsonObservationsAdapter(SdmxJson sdmxJson, string defaultLanguage)
        {
            var sdmxJson1 = sdmxJson;
            _dataset = sdmxJson1.DataSets[0];
            _structure = sdmxJson1.Structure;
            Values = new Dictionary<string[], IndexedObservation>();
            Attributes = new SdmxJsonAttributesAdapter(sdmxJson1.Structure, defaultLanguage);
            InitDatasetAttributes();
            InitObservationCache();
            InitSeriesObservationCache();
        }

        public IEnumerator<IndexedObservation> GetEnumerator()
        {
            return Values.Select(val => val.Value).GetEnumerator();
        }

        protected string[] CalculateCodeIndex(List<int> coords)
        {
            var result = new string[coords.Count];

            for (var i = 0; i < coords.Count; i++) {
                var observationSingleCoordinate = coords[i];
                result[i] = GetDimensionItemCodeByPosition(i, observationSingleCoordinate);
            }

            return result;
        }

        protected string GetDimensionItemCodeByPosition(int dimensionPosition, int coord)
        {
            var dimension = _structure.Dimensions.GetDimensionByPosition(dimensionPosition);

            return dimension.Values[coord].Id;
        }

        private void InitDatasetAttributes()
        {
            for (var i = 0; i < _dataset.Attributes?.Count; i++)
                Attributes.DatasetAttributeIndex[i] = _dataset.Attributes[i];
        }

        private void InitObservationCache()
        {
            if (_dataset.Observations == null) {
                return;
            }

            foreach (var (dimensionCodePositions, observationValues) in _dataset.Observations) {
                var index = CalculateCodeIndex(dimensionCodePositions);
                AddDimensionCodesToCache(index);
                var indexedObservation = new IndexedObservation(index, observationValues[0]);

                Values[index] = indexedObservation;

                //new string[] { "M", "D", "IT", "N", "TURN", "0040", "2015", "2018-S2" };

                // OBSERVATION ATTRIBUTES REFERENCE HERE
                var observationAttributeIndex = observationValues
                    .GetRange(1, observationValues.Count - 1)
                    .Select(x => x.GetAsNullableInt())
                    .ToArray();

                // add attribute index if and only if there's at least one not null value
                if (observationAttributeIndex.Any(x => x != null)) {
                    Attributes.ObservationAttributeIndex[index] = observationAttributeIndex;
                }
            }
        }

        private void InitSeriesObservationCache()
        {
            if (!_dataset.HasSeries) {
                return;
            }

            foreach (var (partialIndex, seriesObservations) in _dataset.Series) {
                var partialCodeIndex =
                    CalculateCodeIndex(partialIndex); // convert [1,2,0,4,8] to ["C1", "GF", "Q12", "CODE1", "AD"]

                // SERIE ATTRIBUTES REFERENCE HERE
                var seriesCodeIndex = new List<string>(partialCodeIndex) {null};
                var seriesCodeIndexArray = seriesCodeIndex.ToArray();

                var seriesAttributes = seriesObservations.Attributes; //copy partial index

                //seriesAttributes.Add(null); //add an empty value for time period dimension
                // add attribute index if and only if there's at least one not null value
                if (seriesAttributes.Any(x => x != null)) {
                    var indexWithoutTimePeriodPosition = new List<string>(partialCodeIndex).ToArray();
                    Attributes.SeriesAttributeIndex[indexWithoutTimePeriodPosition] = seriesAttributes.ToArray();
                }

                foreach (var (dimensioneCodePosition, observationValues) in seriesObservations.Observations) {
                    //calculate time period code (it' always at the last position of the list)
                    string timePeriodCode;

                    try {
                        timePeriodCode =
                            GetDimensionItemCodeByPosition(partialCodeIndex.Length + 1, dimensioneCodePosition);
                    }
                    catch (Exception) {
                        //GetDimensionItemCodeByPosition(partialCodeIndex.Length + 1, seriesObservationsEntry.Key);
                        continue;
                    }

                    // copy partial index and add time period code at last position
                    var index = seriesCodeIndexArray.ToArray();
                    index[^1] = timePeriodCode;

                    //get observation value
                    var observationValue = observationValues[0];

                    AddDimensionCodesToCache(index);
                    var indexedObservation = new IndexedObservation(index, observationValue);
                    Values[index] = indexedObservation;

                    // OBSERVATION ATTRIBUTES REFERENCE HERE
                    var observationAttributeIndex = observationValues
                        .GetRange(1, observationValues.Count - 1)
                        .Select(x => x.GetAsNullableInt())
                        .ToArray();

                    // add attribute index if and only if there's at least one not null value
                    if (observationAttributeIndex.Any(x => x != null)) {
                        Attributes.ObservationAttributeIndex[index] = observationAttributeIndex;
                    }
                }
            }
        }

        private void AddDimensionCodesToCache(IReadOnlyList<string> codes)
        {
            if (DistinctDimensionsCodes == null) {
                DistinctDimensionsCodes = new HashSet<string>[codes.Count];
                for (var i = 0; i < codes.Count; i++) DistinctDimensionsCodes[i] = new HashSet<string>();
            }

            for (var i = 0; i < codes.Count; i++) DistinctDimensionsCodes[i].Add(codes[i]);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

    }
}