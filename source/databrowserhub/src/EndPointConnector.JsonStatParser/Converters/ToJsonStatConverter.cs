using System;
using System.Collections.Generic;
using System.Linq;
using EndPointConnector.Interfaces.JsonStat;
using EndPointConnector.JsonStatParser.Adapters.Interfaces;
using EndPointConnector.JsonStatParser.Model.JsonStat;
using EndPointConnector.JsonStatParser.StructureUtils.Conversion;
using EndPointConnector.JsonStatParser.StructureUtils.Sorting.WeightGenerators;
using Microsoft.Extensions.Logging;
using Org.Sdmxsource.Util.Extensions;

namespace EndPointConnector.JsonStatParser.Converters
{
    public class ToJsonStatConverter : IToJsonStatConverter
    {

        private const string DefaultLanguage = "en";

        private readonly IFromSDMXToJsonStatConverterConfig _conversionConfig;

        private readonly string _currentLanguage;

        private readonly IObservationsAdapter _datasetObservations;


        private readonly IDatasetStructureAdapter _datasetStructure;

        private readonly ILogger<ToJsonStatConverter> _logger;

        private HashSet<string>[] _allBannedCodes;

        private bool _badStructure;

        private JsonStatDataset _jsonStatDataset;


        public ToJsonStatConverter(ILoggerFactory loggerFactory, string currentLanguage,
            IDatasetStructureAdapter datasetStructure, IObservationsAdapter datasetObservations,
            IFromSDMXToJsonStatConverterConfig config = null)
        {
            _logger = loggerFactory.CreateLogger<ToJsonStatConverter>();
            _currentLanguage = string.IsNullOrEmpty(currentLanguage) ? DefaultLanguage : currentLanguage;
            _conversionConfig = config ?? DefaultJsonStatConverterConfig.GetNew();
            _badStructure = false;
            _datasetStructure = datasetStructure;
            _datasetObservations = datasetObservations;
        }

        public string Convert()
        {
            _logger.LogDebug("START JsonStatConverter");
            InitStructure();

            AddDimensionInfos();

            if (_badStructure) {
                _logger.LogDebug("bad dataset structure was detected. Returning empty result");
                _jsonStatDataset.Clear();

                return SerializeJsonStat();
            }

            //SORT HERE
            SortDimensions();

            AddObservations();

            AddAttributesToJsonStat();

            RemoveEmptyAndHiddenDimensions();

            return SerializeJsonStat();
        }


        protected string SerializeJsonStat()
        {
            try {
                _logger.LogDebug("Serializing JsonStat");
                var serializedJsonStat = JsonStatDataset.Serialize(_jsonStatDataset);

                return serializedJsonStat;
            }
            catch (Exception e) {
                _logger.LogError("An error occurred while serializing JSONStat object", e);

                throw new Exception("JsonStatConverter: An error occurred while serializing JSONStat object", e);
            }
        }

        protected void InitStructure()
        {
            _logger.LogDebug("Building dataset structure");
            _jsonStatDataset = new JsonStatDataset
            {
                Label = _datasetStructure.Title.TryGet(_currentLanguage)
            };
            InitBannedCodes();
            InitGeoDimension();
            InitTimeDimension();
        }

        protected void InitGeoDimension()
        {
            _logger.LogDebug("search for territorial dimensions");
            _jsonStatDataset.Role.Geo = new List<string>();

            if (_datasetStructure.GeoDimensionIds?.Length > 0) {
                var geoIdFromDataStructure = _datasetStructure.DimensionIds
                    .Where(x => _datasetStructure.GeoDimensionIds.Contains(x))
                    .ToList();

                var geoIds = geoIdFromDataStructure.Where(geoId => _jsonStatDataset.Role.Geo.Contains(geoId) == false);
                foreach (var geoId in geoIds) _jsonStatDataset.Role.Geo.Add(geoId);
            }

            if (_conversionConfig.TerritorialDimensionIds == null) {
                return;
            }

            var geoIdFromNodeConfig = _datasetStructure.DimensionIds
                .Where(x => _conversionConfig.TerritorialDimensionIds.Contains(x)).ToList();

            foreach (var geoId in geoIdFromNodeConfig.Where(geoId => _jsonStatDataset.Role.Geo.Contains(geoId) == false)
            ) _jsonStatDataset.Role.Geo.Add(geoId);
        }

        protected void InitTimeDimension()
        {
            _logger.LogDebug("search for temporal dimensions");

            if (_conversionConfig.TemporalDimensionIds != null) {
                var timeDimIds = _datasetStructure.DimensionIds
                    .Where(x => _conversionConfig.TemporalDimensionIds.Contains(x)).ToList();

                if (timeDimIds.Count > 0) {
                    _jsonStatDataset.Role.Time = timeDimIds;

                    return;
                }
            }

            if (_datasetStructure.MainTimeDimensionId != null) {
                _jsonStatDataset.Role.Time.Add(_datasetStructure.MainTimeDimensionId);
            }
        }

        protected void AddDimensionInfos()
        {
            _logger.LogDebug("Building dimension codes and values");

            for (var i = 0; i < _datasetStructure.DimensionIds.Length; i++) {
                var dimensionId = _datasetStructure.DimensionIds[i];
                var dimensionAdapter = _datasetStructure.GetDimensionById(dimensionId);
                var newJsonStatDimension = new JsonStatDimension(dimensionAdapter.Label.TryGet(_currentLanguage));
                var bannedDimensionCodes = _allBannedCodes[i];
                var wGenerator = new SimpleWeightGenerator(new HashSet<string>());
                var distinctDimensionCode = _datasetObservations.DistinctDimensionsCodes[i];

                foreach (var code in distinctDimensionCode)
                    if (!bannedDimensionCodes.Contains(code) || distinctDimensionCode.Count == 1) {
                        var dimensionItem = dimensionAdapter.GetDimensionItemByCode(code);
                        var label = dimensionItem != null ? dimensionItem.Label.TryGet(_currentLanguage) : code;
                        newJsonStatDimension.Category.AddLabel(label, code, wGenerator);
                    }

                if (newJsonStatDimension.Count == 0 && distinctDimensionCode.Count > 1) {
                    _badStructure = true;

                    return;
                }

                _jsonStatDataset.Id.Add(dimensionId);
                _jsonStatDataset.Size.Add(newJsonStatDimension.Count);
                _jsonStatDataset.Dimension[dimensionId] = newJsonStatDimension;
            }
        }

        protected void AddObservations()
        {
            _logger.LogDebug("Adding observation values");

            foreach (var (observationDimensionCodes, observationValue) in _datasetObservations) {
                var position = CalculateObservationPositionByDimensionItemIds(observationDimensionCodes);

                if (position < 0) {
                    continue;
                }

                _jsonStatDataset.AddObservationValue(position, observationValue);
            }
        }

        protected void SortDimensions()
        {
            _logger.LogDebug("Sorting dimensions codes");

            foreach (var (dimensionId, dimension) in _jsonStatDataset.Dimension) {
                var sortedIndex = _datasetStructure.CalculateDimensionIndex(dimensionId,
                    dimension.Category.Label.Keys.ToArray(), _currentLanguage);

                if (sortedIndex.Count != dimension.Category.Index.Count) {
                    throw new Exception("Illegal jsonstat index state");
                }

                dimension.Category.Index = sortedIndex;
            }
        }


        protected void AddAttributesToJsonStat()
        {
            _logger.LogDebug("Processing attributes");
            var attributes = _datasetObservations.Attributes;

            if (attributes == null) {
                return;
            }

            AddAllAttributesToJsonStatDataset();

            InitDatasetAttributesIndex();

            InitObservationAttributesIndex();

            InitSeriesAttributesIndex();
        }

        protected bool DimensionCanBeHidden(string dimensionId)
        {
            var dimensionPosition = _jsonStatDataset.DimensionPosition(dimensionId);

            if (dimensionPosition < 0) {
                return true;
            }

            var dimensionSize = _jsonStatDataset.Size[dimensionPosition];

            if (dimensionSize <= 0) {
                return true;
            }

            if (dimensionSize != 1) {
                return false;
            }

            var singleDimensionValue = _jsonStatDataset.Dimension[dimensionId].Category.Index.Keys.First();

            return _allBannedCodes[dimensionPosition].Contains(singleDimensionValue);
        }

        protected void RemoveEmptyAndHiddenDimensions()
        {
            _logger.LogDebug("Removing empty/hidden dimensions from dataset");
            var removingIndex = _jsonStatDataset.Id
                .Select((dimensionId,
                    index) => (dimensionId, index))
                .Where(x => DimensionCanBeHidden(x.dimensionId))
                .Select(x => x.index)
                .ToList();

            for (var i = 0; i < removingIndex.Count; i++) {
                var dimensionId = _jsonStatDataset.Id[removingIndex[i] - i];
                _jsonStatDataset.Size.RemoveAt(removingIndex[i] - i);
                _jsonStatDataset.Id.RemoveAt(removingIndex[i] - i);
                _jsonStatDataset.Dimension.Remove(dimensionId);
                _jsonStatDataset.Extension.Attributes.Index.RemoveSeriesDimensionReference(removingIndex[i]);
            }
        }

        private void InitBannedCodes()
        {
            _logger.LogDebug("search for Hidden dimension codes");
            _allBannedCodes = new HashSet<string>[_datasetStructure.DimensionIds.Length];

            for (var i = 0; i < _datasetStructure.DimensionIds.Length; i++) {
                var dimensionId = _datasetStructure.DimensionIds[i];
                var bannedCodes = _datasetStructure.GetBannedCodesByDimensionById(dimensionId, _currentLanguage,
                    _conversionConfig.NotDisplayedAnnotationId, _datasetObservations.DistinctDimensionsCodes[i]);
                _allBannedCodes[i] = new HashSet<string>();
                var distinctValues = _datasetObservations.DistinctDimensionsCodes[i];
                _allBannedCodes[i].AddAll(bannedCodes.Intersect(distinctValues));
            }
        }


        private int[] CalculateObservationIndexByDimensionItemIds(IReadOnlyList<string> stringIndex)
        {
            var result = new int[stringIndex.Count];

            for (var i = 0; i < stringIndex.Count; i++) {
                var dimensionId = _jsonStatDataset.Id[i];
                var dimensionItemCode = stringIndex[i];

                // if the current code is banned and the dimension size is greater than 1
                if (_allBannedCodes[i].Contains(dimensionItemCode) &&
                    _datasetObservations.DistinctDimensionsCodes[i].Count > 1) {
                    return null;
                }

                var codePosition = _jsonStatDataset.Dimension[dimensionId].Category.Index[dimensionItemCode];
                result[i] = codePosition;
            }

            return result;
        }


        private int?[] CalculateSeriesIndexByDimensionItemIds(IReadOnlyList<string> stringIndex)
        {
            var result = new int?[stringIndex.Count];

            for (var i = 0; i < stringIndex.Count; i++) {
                var dimensionId = _jsonStatDataset.Id[i];
                var dimensionItemCode = stringIndex[i];

                if (dimensionItemCode == null) {
                    result[i] = null;
                }
                else {
                    // if the current code is banned and the dimension size is greater than 1
                    if (_allBannedCodes[i].Contains(dimensionItemCode) &&
                        _datasetObservations.DistinctDimensionsCodes[i].Count > 1) {
                        return null;
                    }

                    var codePosition = _jsonStatDataset.Dimension[dimensionId].Category.Index[dimensionItemCode];
                    result[i] = codePosition;
                }
            }

            return result;
        }

        private int CalculateObservationPositionByDimensionItemIds(IReadOnlyList<string> dimensionCodes)
        {
            var index = CalculateObservationIndexByDimensionItemIds(dimensionCodes);
            var observationPosition = JsonStatUtils.RowMajorOrder(_jsonStatDataset.Size, index);

            return observationPosition;
        }

        private void InitSeriesAttributesIndex()
        {
            _logger.LogDebug("Building series attributes index");

            // init series attributes index
            foreach (var (serieDimensionItemIds, serieAttributeValue) in _datasetObservations.Attributes
                .SeriesAttributeIndex) {
                var positions = CalculateSeriesIndexByDimensionItemIds(serieDimensionItemIds);

                if (positions != null) {
                    _jsonStatDataset.Extension.Attributes.AddSeriesAttributeIndex(positions,
                        serieAttributeValue.ToList(),
                        _jsonStatDataset.Id);
                }
            }
        }

        private void InitObservationAttributesIndex()
        {
            _logger.LogDebug("Building observation attributes index");
            // observation status values initialization
            var observationStatusAttributePosition =
                _datasetObservations.Attributes.FindObservationStatusAttributePosition();
            AddAllObservationStatusAttributes(observationStatusAttributePosition);

            // init observation attributes index
            foreach (var (dimensionItemIds, attributePositions) in _datasetObservations.Attributes
                .ObservationAttributeIndex) {
                var observartionPosition = CalculateObservationPositionByDimensionItemIds(dimensionItemIds);

                if (observartionPosition < 0) {
                    continue;
                }

                _jsonStatDataset.Extension.Attributes.Index.AddObservationAttributeIndex(observartionPosition,
                    attributePositions);

                // if there is an observation value and is not null, the we add it to the dataset "status" field
                if (observationStatusAttributePosition < 0) {
                    continue;
                }

                var observationStatusAttributeValuePosition =
                    attributePositions[observationStatusAttributePosition];

                if (observationStatusAttributeValuePosition == null) {
                    continue;
                }

                var statusAttributesItem = _datasetObservations.Attributes
                    .ObservationAttributes[observationStatusAttributePosition]
                    .Values[observationStatusAttributeValuePosition.Value];
                var statusAttributesItemId = statusAttributesItem.Id;
                _jsonStatDataset.Status[observartionPosition] = statusAttributesItemId;
            }
        }

        private void AddAllObservationStatusAttributes(int observationStatusAttributePosition)
        {
            _logger.LogDebug("Searching for status attributes");

            if (observationStatusAttributePosition < 0) {
                return;
            }

            var statusAttribute =
                _datasetObservations.Attributes.ObservationAttributes[observationStatusAttributePosition];
            foreach (var statusAttrValue in statusAttribute.Values)
                _jsonStatDataset.Extension.AddStatus(statusAttrValue.Id,
                    statusAttrValue.Label.TryGet(_currentLanguage));
        }

        private void InitDatasetAttributesIndex()
        {
            _logger.LogDebug("Building dataset attributes index");

            // init dataset attributes index
            if (_datasetObservations.Attributes.DatasetAttributeIndex.Count <= 0) {
                return;
            }

            var datasetAttrIndex = new int?[_datasetObservations.Attributes.DatasetAttributes.Length];

            for (var i = 0; i < _datasetObservations.Attributes.DatasetAttributes.Length; i++)
                if (_datasetObservations.Attributes.DatasetAttributeIndex.ContainsKey(i)) {
                    datasetAttrIndex[i] = _datasetObservations.Attributes.DatasetAttributeIndex[i];
                }
                else {
                    datasetAttrIndex[i] = null;
                }

            _jsonStatDataset.Extension.Attributes.Index.SetDataSetAttributeIndex(datasetAttrIndex.ToList());
        }

        private void AddAllAttributesToJsonStatDataset()
        {
            _logger.LogDebug("Adding all attributes to the dataset");
            // init attribute list
            _jsonStatDataset.Extension.Attributes.Observation = _datasetObservations.Attributes.ObservationAttributes
                .Select(attr => StructureConverters.JStatAttributeFromGenericAttribute(attr, _currentLanguage))
                .ToList();
            _jsonStatDataset.Extension.Attributes.Series = _datasetObservations.Attributes.SeriesAttributes
                .Select(attr => StructureConverters.JStatAttributeFromGenericAttribute(attr, _currentLanguage))
                .ToList();
            _jsonStatDataset.Extension.Attributes.DataSet = _datasetObservations.Attributes.DatasetAttributes
                .Select(attr => StructureConverters.JStatAttributeFromGenericAttribute(attr, _currentLanguage))
                .ToList();
        }

    }
}