using System;
using System.Collections.Generic;
using System.Linq;
using EndPointConnector.Interfaces.JsonStat;
using EndPointConnector.JsonStatParser.Adapters.Commons;
using EndPointConnector.JsonStatParser.Adapters.Interfaces;
using EndPointConnector.JsonStatParser.Model.SdmxJson;
using EndPointConnector.JsonStatParser.StructureUtils.Sorting.Codelist.Wrappers;
using EndPointConnector.JsonStatParser.StructureUtils.Sorting.WeightGenerators;
using EndPointConnector.JsonStatParser.StructureUtils.Sorting.WeightGenerators.Codelist;
using EndPointConnector.JsonStatParser.StructureUtils.Sorting.WeightGenerators.TimePeriod;
using Microsoft.Extensions.Logging.Abstractions;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Util.Extensions;

namespace EndPointConnector.JsonStatParser.Adapters.SdmxJsonAdapters
{
    public class SdmxJsonDatasetStructureAdapter : IDatasetStructureAdapter
    {

        public LocalizedString Title { get; }

        public string[] HiddenDimensionIds => new string[0];

        public string[] DimensionIds
        {
            get
            {
                if (_dimensionIdList != null) {
                    return _dimensionIdList;
                }

                if (HasSeries()) {
                    // merging non-time dimensions and time dimension 
                    var mergedDimensionsList = _sdmxJson.Structure.Dimensions.Series.Select(d => d.Id).ToList();
                    mergedDimensionsList.AddRange(_sdmxJson.Structure.Dimensions.Observation.Select(d => d.Id)
                        .ToList());
                    _dimensionIdList = mergedDimensionsList.ToArray();
                }
                else {
                    _dimensionIdList = _sdmxJson.Structure.Dimensions.Observation.Select(d => d.Id).ToArray();
                }

                return _dimensionIdList;
            }
        }


        public string[] GeoDimensionIds => GetGeoDimensionIdsFromAnnotation();

        public string MainGeoDimensionId =>
            GeoDimensionIds != null && GeoDimensionIds.Length > 0 ? GeoDimensionIds[0] : null;

        public string[] TimeDimensionIds
        {
            get
            {
                return _sdmxJson.Structure.Dimensions.Observation.Where(v => v.Id == "TIME_PERIOD").Select(v => v.Id)
                    .ToArray();
            }
        }

        public string MainTimeDimensionId =>
            TimeDimensionIds != null && TimeDimensionIds.Length > 0 ? TimeDimensionIds[0] : null;

        public string[] AlternativeObservationsDimensionIds => new string[0];

        public Dictionary<string, string> CustomRoles => throw new NotImplementedException();

        public Dictionary<string, object> Extras => throw new NotImplementedException();

        private readonly ISDMXParsingConfig _config;

        private readonly SdmxJsonDataSet _dataset;

        private readonly SdmxJson _sdmxJson;

        private string[] _dimensionIdList;

        private Dictionary<string, IDimensionAdapter> _dimensions;

        private string[] _geoDimensionIds;

        private IDataStructureObject _dataStructure;

        public SdmxJsonDatasetStructureAdapter(SdmxJson sdmxJson, ISDMXParsingConfig config, IDataStructureObject dataStructure )
        {
            _sdmxJson = sdmxJson;
            _config = config;
            _dataset = _sdmxJson.DataSets[0];
            _dataStructure = dataStructure;

            Title = _sdmxJson.Structure.Names != null
                ? new LocalizedString(_sdmxJson.Structure.Names)
                : new LocalizedString(_sdmxJson.Structure.Name);
        }


        public IDimensionAdapter GetDimensionById(string dimensionId)
        {
            if (_dimensions == null) {
                InitDimensions();
            }

            return _dimensions.TryGetValue(dimensionId, out var value) ? value : null;
        }

        public bool HasSeries()
        {
            return _dataset.HasSeries;
        }

        public Dictionary<string, int> CalculateDimensionIndex(string dimensionId, string[] codes, string language)
        {
            var weightGenerator = GetWeightGenerator(dimensionId, language);
            var sortedIndex = weightGenerator.GenerateSortedIndex(codes);

            return sortedIndex;
        }


        private IEnumerable<string> ExtractNotDisplayedAnnotationValue(string notDiplayedAnnotationName, string dimensionId, string language)
        {
            if (string.IsNullOrEmpty(dimensionId))
            {
                return new List<string>();
            }

            IEnumerable<string> fromDataflow = _sdmxJson.Structure.Annotations
                .Where(a => a.Type == TypeEnum.NotDisplayed)
                .Select(oa => oa.GetLocalizedText(language))
                .Where(val => val != null && val.ToLower().StartsWith(dimensionId.ToLower()));

            if (fromDataflow?.Count() > 0)
            {
                return fromDataflow;
            }

            IEnumerable<string> fromDSD = _dataStructure.Annotations
                            .Where(a => a.Type == notDiplayedAnnotationName)
                            .Select(oa => oa.Title)
                            .Where(val => val != null && val.ToLower().StartsWith(dimensionId.ToLower()));

            return fromDSD;
        }


        public HashSet<string> GetBannedCodesByDimensionById(string dimensionId, string language,
            string notDiplayedAnnotationName, HashSet<string> distinctDimensionValues)
        {
            var dimensionAdapter = GetDimensionById(dimensionId);
            var result = new HashSet<string>();
            /*
            var notDiplayedAnnotation = _sdmxJson.Structure.Annotations
                .Where(a => a.Type == TypeEnum.NotDisplayed)
                .Select(oa => oa.GetLocalizedText(language))
                .Where(val => val != null);
                */
            IEnumerable<string> notDiplayedAnnotation = ExtractNotDisplayedAnnotationValue(notDiplayedAnnotationName, dimensionId, language);

            // from string "DIM_CODE=item_code1,DIM_CODE2,DIM_CODE3=item_code2+item_code3"  -> to [ DIM_CODE=item_code1, DIM_CODE2, DIM_CODE3=item_code2+item_code3 ]
            var splittedAnnotations = notDiplayedAnnotation.SelectMany(tx =>
                    tx.Split(','))
                .ToList();

            splittedAnnotations.ForEach(ann =>
            {
                var annotationTokens = ann.Split('='); //from "DIM_CODE=item_code1"  -> to  [DIM_CODE, item_code1]

                if (!string.Equals(annotationTokens[0], dimensionId, StringComparison.CurrentCultureIgnoreCase)) {
                    return;
                }

                if (annotationTokens.Length == 1 && distinctDimensionValues.Count == 1) {
                    result.AddAll(dimensionAdapter.Items.Select(item => item.Id).ToList());
                }
                else if (annotationTokens.Length > 1) {
                    var allBannedCodesFromAnnotation =
                        annotationTokens[1].Split('+'); // from "item_code2+item_code3" -> [item_code2, item_code3]
                    result.AddAll(allBannedCodesFromAnnotation);
                }

                //the whole codelist is banned
            });

            return result;
        }

        protected void InitDimensions()
        {
            _dimensions = new Dictionary<string, IDimensionAdapter>();

            foreach (var dimensionId in DimensionIds) {
                var sdmxJsonDimension = _sdmxJson.Structure.Dimensions.GetDimensionById(dimensionId);
                var dimension = new SdmxJsonDimensionAdapter(sdmxJsonDimension);
                _dimensions[dimension.Id] = dimension;
            }
        }

        protected IWeightGenerator GetWeightGenerator(string dimensionId, string language)
        {
            var bannedCodes = new HashSet<string>(); //no code ban here

            if (dimensionId == MainTimeDimensionId) {
                return new TimeDimensionWeightGenerator(NullLoggerFactory.Instance, bannedCodes);
            }

            var originalDimension = _sdmxJson.Structure.Dimensions.GetDimensionById(dimensionId);
            var originalAnnotations = _sdmxJson.Structure.Annotations;
            var codelistWrapper = new SdmxJsonCodelistWrapper(originalDimension, originalAnnotations, language);

            return new OrderAnnotationWeightGenerator<SdmxJsonGenericValueWrapper>(codelistWrapper, bannedCodes);
        }

        private string[] GetGeoDimensionIdsFromAnnotation()
        {
            if (_geoDimensionIds != null) {
                return _geoDimensionIds;
            }

            var geoIds = new List<string>();

            if (_sdmxJson.Structure.Annotations != null) {
                foreach (var ann in _sdmxJson.Structure.Annotations.Where(ann => ann.Id != null &&
                    ann.Id.Equals(_config.GeoAnnotationId) && DimensionIds.Contains(ann.Title) &&
                    geoIds.Contains(ann.Id) == false))
                    geoIds.Add(ann.Title);
            }

            _geoDimensionIds = geoIds.ToArray();

            return _geoDimensionIds;
        }

    }
}