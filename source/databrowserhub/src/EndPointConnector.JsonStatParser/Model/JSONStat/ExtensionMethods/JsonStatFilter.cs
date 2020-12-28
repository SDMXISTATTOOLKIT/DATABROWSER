using System;
using System.Collections.Generic;
using System.Linq;
using EndPointConnector.JsonStatParser.Model.JsonStat.Extensions;
using EndPointConnector.JsonStatParser.StructureUtils.Sorting.WeightGenerators;
using EndPointConnector.JsonStatParser.StructureUtils.Sorting.WeightGenerators.JsonStatCategory;
using EndPointConnector.JsonStatParser.StructureUtils.Sorting.WeightGenerators.TimePeriod;
using EndPointConnector.Models;
using Microsoft.Extensions.Logging.Abstractions;

namespace EndPointConnector.JsonStatParser.Model.JsonStat.ExtensionMethods
{
    public class JsonStatFilter
    {

        private readonly JsonStatDataset _jsonStatDataset;

        private readonly List<Criteria> _notDisplayed;

        private readonly Dictionary<int, int?[]> _oldPositionReverseCoordinates;

        private readonly Dictionary<int, double?[]> _oldPositionToWeight;

        private Dictionary<string, List<string>> _filters;

        private string _freqDimensionId;

        private Dictionary<int, JsonStatDimension> _newDimensions;

        private JsonStatCustomDatasetExtension _newExtension;

        private Dictionary<int, ObservationValue> _newObservations;

        private List<int> _newSizes;

        private Dictionary<int, string> _newStatus;

        private string _timeDimensionId;

        private Dictionary<string, IWeightGenerator> _weightGenerators;


        public JsonStatFilter(JsonStatDataset jsonStatDataset, List<FilterCriteria> criteria,
            List<Criteria> notDisplayed = null)
        {
            _jsonStatDataset = jsonStatDataset;
            InitTimeDimensionId();
            InitFreqDimensionId();
            InitFilters(criteria);
            _notDisplayed = notDisplayed;
            _oldPositionToWeight = new Dictionary<int, double?[]>();
            _oldPositionReverseCoordinates = new Dictionary<int, int?[]>();
        }

        public void Filter()
        {
            //check if there are some empty criteria.
            if (CountEmptyCriteria() > 0)
            {
                _jsonStatDataset.Clear();

                return;
            }

            InitNewDatasetStructures();

            ReverseOldCoordinates();

            SortNewDimensions();

            CalculateNewDimensionSizes();

            AddNewObservationsAndAttributes();

            AssignNewPropsToOldDataset();

            RemoveEmptyOrHiddenDimensions();

            if (_jsonStatDataset.Size.All(dimensionSize => dimensionSize == 0))
            {
                _jsonStatDataset.Clear();
            }
        }


        private void InitTimeDimensionId()
        {
            if (_jsonStatDataset.Role?.Time?.Count > 0)
            {
                _timeDimensionId = _jsonStatDataset.Role.Time[0];
            }
        }

        private void InitFreqDimensionId()
        {
            _freqDimensionId = "FREQ";
        }

        private void InitFilters(List<FilterCriteria> criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            var timeRange = CalculateTimeRangeCriteria(criteria.FirstOrDefault(i => i.Id.Equals(_timeDimensionId)));

            _filters = criteria.ToDictionary(v => v.Id, v => v.FilterValues);

            if (_filters != null && _filters.ContainsKey(_timeDimensionId))
            {
                _filters[_timeDimensionId] = timeRange;
            }
        }

        private int? CountEmptyCriteria()
        {
            return _filters.Where(entry => entry.Value?.Count == 0)
                .Select(entry => entry.Key).Count();
        }

        private void RemoveEmptyOrHiddenDimensions()
        {
            var hiddenDimensionIds = _notDisplayed?.Where(x =>
                    (x.Values == null || x.Values.Count == 0) && _jsonStatDataset.DimensionSize(x.Id) == 1)
                .Select(x => x.Id)
                .ToHashSet();
            _jsonStatDataset.RemoveEmptyDimensions(hiddenDimensionIds);
        }

        private void AssignNewPropsToOldDataset()
        {
            _jsonStatDataset.Size = _newSizes;
            _jsonStatDataset.Value = _newObservations;
            _jsonStatDataset.Dimension = _jsonStatDataset.Id
                .Select((v, index) => (id: v, dim: _newDimensions[index]))
                .ToDictionary(v => v.id, v => v.dim);
            _jsonStatDataset.Extension = _newExtension;
            _jsonStatDataset.Status = _newStatus;
        }

        private void AddNewObservationsAndAttributes()
        {
            foreach (var (oldPosition, newWeights) in _oldPositionToWeight)
            {
                var observationValue = _jsonStatDataset.Value[oldPosition];
                var newCoordinates = CalculateNewObservationCoordinates(newWeights);
                var newObservationPosition = JsonStatUtils.RowMajorOrderWithnullables(_newSizes, newCoordinates);

                AddNewObservationAndStatus(newObservationPosition, observationValue, oldPosition);

                AddNewObservationAttributes(oldPosition, newObservationPosition);

                AddNewSeriesAttributes(oldPosition, newCoordinates);
            }
        }

        private void CalculateNewDimensionSizes()
        {
            _newSizes = _jsonStatDataset.Id.Select((v, index) => _newDimensions[index].Count).ToList();
        }

        private void AddNewSeriesAttributes(int oldPosition, int?[] newCoordinates)
        {
            // add series attributes
            var oldReverseCoordinates = _oldPositionReverseCoordinates[oldPosition];
            var oldReverseCoordinatesWithoutTimePeriod =
                oldReverseCoordinates.Take(oldReverseCoordinates.Length - 1).ToArray();
            var newCoordinatesWithoutTimePeriod = newCoordinates.Take(newCoordinates.Length - 1).ToArray();
            var seriesAttributesIndex =
                _jsonStatDataset.Extension.Attributes.Index.GetSeriesAttributeIndex(
                    oldReverseCoordinatesWithoutTimePeriod);

            if (seriesAttributesIndex == null)
            {
                return;
            }

            foreach (var seriesIndex in seriesAttributesIndex)
                _newExtension.Attributes.Index.AddSeriesAttributeIndex(newCoordinatesWithoutTimePeriod,
                    seriesIndex.Attributes);
        }

        private void AddNewObservationAttributes(int oldPosition, int? newObservationPosition)
        {
            // add observation attributes
            var observationAttributesIndex =
                _jsonStatDataset.Extension.Attributes.Index.GetObservationAttributeIndex(oldPosition);

            if (observationAttributesIndex == null)
            {
                return;
            }

            if (newObservationPosition != null)
            {
                _newExtension.Attributes.Index.AddObservationAttributeIndex(newObservationPosition.Value,
                    observationAttributesIndex);
            }
        }

        private void AddNewObservationAndStatus(int? newObservationPosition, ObservationValue observationValue,
            int oldPosition)
        {
            if (newObservationPosition == null)
            {
                return;
            }

            _newObservations[newObservationPosition.Value] = observationValue;

            //add status information
            if (_jsonStatDataset.Status.TryGetValue(oldPosition, out var statusId) && statusId != null)
            {
                _newStatus[newObservationPosition.Value] = statusId;
            }
        }

        private int?[] CalculateNewObservationCoordinates(IReadOnlyList<double?> newWeights)
        {
            var newCoordinates = new int?[newWeights.Count];

            for (var i = 0; i < newWeights.Count; i++)
            {
                var weight = newWeights[i];
                var realPosition = _newDimensions[i].Category.PositionByWeight(weight);
                newCoordinates[i] = realPosition;
            }

            return newCoordinates;
        }

        private void SortNewDimensions()
        {
            foreach (var dim in _newDimensions) dim.Value.Category.SortByWeight();
        }

        private void ReverseOldCoordinates()
        {
            var bannedCodes = CalculateBannedCodesPositions();

            foreach (var observationsPositon in _jsonStatDataset.Value.Keys)
            {
                var reverseCoordinates = _jsonStatDataset.PositionToCoordinates(observationsPositon, bannedCodes);

                if (reverseCoordinates == null) // reverseCoordinates == null -> banned code
                {
                    continue;
                }

                var newWeights = new double?[reverseCoordinates.Length];

                for (var i = 0; i < reverseCoordinates.Length; i++)
                {
                    var dimensionId = _jsonStatDataset.Id[i];
                    var dimensionCode = _jsonStatDataset.Dimension[dimensionId].Category
                        .CodeByPosition(reverseCoordinates[i].Value);
                    var dimensionLabel = _jsonStatDataset.Dimension[dimensionId].Category.Label[dimensionCode];
                    var newWeight = _newDimensions[i].Category
                        .AddLabel(dimensionLabel, dimensionCode, _weightGenerators[dimensionId]);
                    newWeights[i] = newWeight;
                }

                _oldPositionToWeight[observationsPositon] = newWeights;
                _oldPositionReverseCoordinates[observationsPositon] = reverseCoordinates;
            }
        }

        private void InitNewDatasetStructures()
        {
            // create new empty dimensions
            _newDimensions = _jsonStatDataset.Id.Select((val, index) => (val, index)).ToDictionary(e => e.index,
                e => new JsonStatDimension(_jsonStatDataset.Dimension[e.val ?? string.Empty].Label));

            //new observations container
            _newObservations = new Dictionary<int, ObservationValue>();

            //new status container
            _newStatus = new Dictionary<int, string>();

            //new extension field
            _newExtension = new JsonStatCustomDatasetExtension
            {
                Status = _jsonStatDataset.Extension.Status,
                Attributes =
                {
                    DataSet = _jsonStatDataset.Extension.Attributes.DataSet,
                    Observation = _jsonStatDataset.Extension.Attributes.Observation,
                    Series = _jsonStatDataset.Extension.Attributes.Series
                }
            };
            _newExtension.Attributes.Index.SetDataSetAttributeIndex(_jsonStatDataset.Extension.Attributes.Index
                .GetDatasetAttributeIndex());

            // create weight generator instances
            _weightGenerators = GetWeightGenerators();
        }


        private List<string> CalculateTimeRangeCriteria(FilterCriteria timePeriodCriteria)
        {
            if (timePeriodCriteria == null)
            {
                return null;
            }

            var freqValues = _jsonStatDataset?.Dimension
                ?.FirstOrDefault(i => i.Key.Equals(_freqDimensionId, StringComparison.InvariantCultureIgnoreCase)).Value
                ?.Category?.Index?.Keys?.Distinct();

            var avaiablesFreq = freqValues as string[] ?? (freqValues ?? Array.Empty<string>()).ToArray();

            if (timePeriodCriteria.Type == FilterType.TimeRange)
            {
                timePeriodCriteria.From ??= DateTime.Now;
                timePeriodCriteria.To ??= timePeriodCriteria.From;

                var filtersDate = new List<string>();

                
                filtersDate = getDateRanges(avaiablesFreq, timePeriodCriteria.From.Value, timePeriodCriteria.To.Value, timePeriodCriteria);

                return filtersDate;
            }


            var timeValues = _jsonStatDataset.Dimension
                .FirstOrDefault(i => i.Key.Equals(_timeDimensionId, StringComparison.InvariantCultureIgnoreCase)).Value
                ?.Category?.Index?.Keys.Distinct();

            timeValues = (timeValues ?? Array.Empty<string>()).Select(i => i.Length == 4 ? i + "-01-01" : i);
            var dateTimeValues = timeValues.Select(DateTime.Parse);
            var maxDate = dateTimeValues.Max();
            var minDate = DateTime.MinValue;

            if (avaiablesFreq.Any(i => i.Equals("D", StringComparison.InvariantCultureIgnoreCase)))
            {
                minDate = maxDate.AddDays(-timePeriodCriteria.Period + 1);
            }
            else if (avaiablesFreq.Any(i => i.Equals("M", StringComparison.InvariantCultureIgnoreCase)))
            {
                minDate = maxDate.AddMonths(-timePeriodCriteria.Period + 1);
            }
            else if (avaiablesFreq.Any(i => i.Equals("Q", StringComparison.InvariantCultureIgnoreCase)))
            {
                minDate = maxDate.AddMonths(-(timePeriodCriteria.Period * 4) + 1);
            }
            else if (avaiablesFreq.Any(i => i.Equals("S", StringComparison.InvariantCultureIgnoreCase)))
            {
                minDate = maxDate.AddMonths(-(timePeriodCriteria.Period * 6) + 1);
            }
            else if (avaiablesFreq.Any(i => i.Equals("A", StringComparison.InvariantCultureIgnoreCase)))
            {
                minDate = maxDate.AddYears(-timePeriodCriteria.Period + 1);
            }

            var filtersDatePeriod = new List<string>();

            filtersDatePeriod = getDateRanges(avaiablesFreq, minDate, maxDate, timePeriodCriteria);

            return filtersDatePeriod;
            //var timePeriodDimension = _jsonStatDataset.Dimension[_timeDimensionId];
            //var result = new List<string>();
            //var timeWeightGenerator =
            //    new TimeDimensionWeightGenerator(NullLoggerFactory.Instance, new HashSet<string>());
            //timeWeightGenerator.SetYearsPositionToTheEnd(false);
            //var minW = timePeriodCriteria.Select(time => timeWeightGenerator.GenerateWeight(time)).Min();
            //var maxW = timePeriodCriteria.Select(time => timeWeightGenerator.GenerateWeight(time)).Max();
            //foreach (var timeDimension in timePeriodDimension.Category.Index)
            //{
            //    var timeCode = timeDimension.Key;
            //    var timeWeight = timeWeightGenerator.GenerateWeight(timeCode);
            //    if (timeWeight <= maxW && timeWeight >= minW) result.Add(timeCode);
            //}

            //return result;
        }

        private List<string> getDateRanges(string[] enumerable, DateTime minDate, DateTime maxDate, FilterCriteria timePeriodCriteria)
        {
            var filtersDatePeriod = new List<string>();


            var minDateFreqD = minDate;
            var minDateFreqM = minDate;
            var minDateFreqQ = minDate;
            var minDateFreqS = minDate;
            var minDateFreqA = minDate;

            if (enumerable.Any(i => i.Equals("D", StringComparison.InvariantCultureIgnoreCase)))
            {
                while (minDateFreqD <= maxDate)
                {
                    filtersDatePeriod.Add(minDateFreqD.ToString("yyyy-MM-dd"));
                    minDateFreqD = minDateFreqD.AddDays(1);
                }
            }

            if (enumerable.Any(i => i.Equals("M", StringComparison.InvariantCultureIgnoreCase)))
            {
                while (minDateFreqM <= maxDate)
                {
                    filtersDatePeriod.Add(minDateFreqM.ToString("yyyy-MM"));
                    minDateFreqM = minDateFreqM.AddMonths(1);
                }
            }

            if (enumerable.Any(i => i.Equals("Q", StringComparison.InvariantCultureIgnoreCase)))
            {
                while (minDateFreqQ <= maxDate)
                {
                    if (minDateFreqQ.Month == 1)
                    {
                        filtersDatePeriod.Add(minDateFreqQ.ToString("yyyy" + "-Q1"));
                    }
                    else if (minDateFreqQ.Month == 4)
                    {
                        filtersDatePeriod.Add(minDateFreqQ.ToString("yyyy" + "-Q2"));
                    }
                    else if (minDateFreqQ.Month == 7)
                    {
                        filtersDatePeriod.Add(minDateFreqQ.ToString("yyyy" + "-Q3"));
                    }
                    else
                    {
                        filtersDatePeriod.Add(minDateFreqQ.ToString("yyyy" + "-Q4"));
                    }
                    minDateFreqQ = minDateFreqQ.AddMonths(3);
                }
            }

            if (enumerable.Any(i => i.Equals("S", StringComparison.InvariantCultureIgnoreCase)))
            {
                while (minDateFreqS <= maxDate)
                {
                    if (minDateFreqS.Month == 1)
                    {
                        filtersDatePeriod.Add(minDateFreqS.ToString("yyyy" + "-S1"));
                    }
                    else
                    {
                        filtersDatePeriod.Add(minDateFreqS.ToString("yyyy" + "-S2"));
                    }
                    minDateFreqS = minDateFreqS.AddMonths(6);
                }
            }

            if (enumerable.Any(i => i.Equals("A", StringComparison.InvariantCultureIgnoreCase)))
            {
                while (minDateFreqA <= maxDate)
                {
                    filtersDatePeriod.Add(minDateFreqA.ToString("yyyy"));
                    minDateFreqA = minDateFreqA.AddYears(1);
                }
            }

            return filtersDatePeriod;
        }

        private Dictionary<string, IWeightGenerator> GetWeightGenerators()
        {
            // create weight generator instances
            var weightGenerators = new Dictionary<string, IWeightGenerator>();
            var dimensionIds =
                _jsonStatDataset.Id.Where(dimensionId => !_jsonStatDataset.Role.Time.Contains(dimensionId));

            foreach (var dimensionId in dimensionIds)
            {
                var dimensionCategory = _jsonStatDataset.Dimension[dimensionId].Category;
                IWeightGenerator weightGenerator = _filters.TryGetValue(dimensionId, out var criteria)
                    ? new JsonStatCategoryWeightGenerator(dimensionCategory, criteria)
                    : new JsonStatCategoryWeightGenerator(dimensionCategory);

                weightGenerators[dimensionId] = weightGenerator;
            }

            // add the time period weight generator
            weightGenerators[_jsonStatDataset.Role.Time[0]] =
                new TimeDimensionWeightGenerator(NullLoggerFactory.Instance, new HashSet<string>());

            return weightGenerators;
        }


        private Dictionary<int, HashSet<int>> CalculateBannedCodesPositions()
        {
            var result = _jsonStatDataset.Id.Select((dimensionId, index) => (dimensionId, index))
                .Where(v => _filters.ContainsKey(v.dimensionId ?? string.Empty) &&
                            _jsonStatDataset.Dimension[v.dimensionId ?? throw new InvalidOperationException()].Category
                                .Index.Count > 1)
                .ToDictionary(
                    k => k.index,
                    v => _jsonStatDataset.Dimension[v.dimensionId ?? throw new InvalidOperationException()].Category
                        .Index
                        .Where(entry => !_filters[v.dimensionId].Contains(entry.Key)).Select(entry => entry.Value)
                        .ToHashSet()
                );
            //_filters[v.dimensionId].Select(dimCode => _jsonStatDataset.Dimension[v.dimensionId].Category.Index[dimCode]).ToHashSet()

            return result;
        }

    }
}