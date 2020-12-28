using System;
using System.Collections.Generic;
using System.Linq;
using EndPointConnector.Interfaces.JsonStat;
using EndPointConnector.JsonStatParser.Adapters.Commons;
using EndPointConnector.JsonStatParser.Adapters.Interfaces;
using EndPointConnector.JsonStatParser.StructureUtils.Sorting.Codelist.Wrappers;
using EndPointConnector.JsonStatParser.StructureUtils.Sorting.WeightGenerators;
using EndPointConnector.JsonStatParser.StructureUtils.Sorting.WeightGenerators.Codelist;
using EndPointConnector.JsonStatParser.StructureUtils.Sorting.WeightGenerators.TimePeriod;
using Microsoft.Extensions.Logging.Abstractions;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Util.Extensions;

namespace EndPointConnector.JsonStatParser.Adapters.SdmxXmlAdapters
{
    public class SdmxXmlDatasetStructureAdapter : IDatasetStructureAdapter
    {

        public LocalizedString Title { get; }

        public string[] HiddenDimensionIds => new string[0];

        public string[] DimensionIds { get; }


        public string[] GeoDimensionIds => GetGeoDimensionIdsFromAnnotation();


        public string MainGeoDimensionId =>
            GeoDimensionIds != null && GeoDimensionIds.Length > 0 ? GeoDimensionIds[0] : null;

        public string[] TimeDimensionIds => new[] { _dataStructure.TimeDimension.Id };

        public string MainTimeDimensionId => _dataStructure.TimeDimension.Id;

        public string[] AlternativeObservationsDimensionIds => new string[0];

        public Dictionary<string, string> CustomRoles => throw new NotImplementedException();

        public Dictionary<string, object> Extras => throw new NotImplementedException();

        private readonly ISet<ICodelistObject> _codelists;

        private readonly ISet<IConceptSchemeObject> _conceptSchemes;

        private readonly ISDMXParsingConfig _config;

        private readonly IDataflowObject _dataflow;

        private readonly IDataStructureObject _dataStructure;

        private readonly string _defaultLanguage;

        private readonly bool _hasSeries;

        private Dictionary<string, IDimensionAdapter> _dimensions;

        private string[] _geoDimensionIds;


        public SdmxXmlDatasetStructureAdapter(IDataflowObject dataflow,
            IDataStructureObject dataStructure,
            ISet<ICodelistObject> codelists, ISet<IConceptSchemeObject> conceptSchemes, ISDMXParsingConfig config,
            string defaultLanguage)
        {
            Title = new LocalizedString(LabelsFromINameable(dataflow));
            _dataflow = dataflow;
            _dataStructure = dataStructure;
            _codelists = codelists;
            _conceptSchemes = conceptSchemes;
            _config = config;
            _hasSeries = false;
            _defaultLanguage = defaultLanguage ?? "en";

            var sortedDimensionsId = _dataStructure.DimensionList.Dimensions.OrderBy(x => x.Position).Select(x => x.Id)
                .ToArray();
            var dimensionsIdList = new List<string>();
            foreach (var idDim in sortedDimensionsId)
            {
                if (!idDim.Equals(_dataStructure.TimeDimension.Id,StringComparison.InvariantCultureIgnoreCase))
                {
                    dimensionsIdList.Add(idDim);
                }
            }
            dimensionsIdList.Add(_dataStructure.TimeDimension.Id);
            DimensionIds = dimensionsIdList.ToArray();
        }


        public IDimensionAdapter GetDimensionById(string dimensionId)
        {
            if (_dimensions == null)
            {
                InitDimensions();
            }

            return _dimensions.TryGetValue(dimensionId, out var value) ? value : null;
        }

        public bool HasSeries()
        {
            return _hasSeries;
        }

        public Dictionary<string, int> CalculateDimensionIndex(string dimensionId, string[] codes, string language)
        {
            var weightGenerator = GetWeightGenerator(dimensionId, language);
            var sortedIndex = weightGenerator.GenerateSortedIndex(codes);

            return sortedIndex;
        }

        public HashSet<string> GetBannedCodesByDimensionById(string dimensionId, string language,
            string notDiplayedAnnotationName, HashSet<string> distinctDimensionValues)
        {
            var dimensionAdapter = GetDimensionById(dimensionId);
            var result = new HashSet<string>();
            IEnumerable<string> notDiplayedAnnotation = ExtractNotDisplayedAnnotationValue(notDiplayedAnnotationName, dimensionId);

            // from string "DIM_CODE=item_code1,DIM_CODE2,DIM_CODE3=item_code2+item_code3"  -> to [ DIM_CODE=item_code1, DIM_CODE2, DIM_CODE3=item_code2+item_code3 ]
            var splittedAnnotations = notDiplayedAnnotation.SelectMany(tx => tx.Split(',').ToList()).ToList();

            splittedAnnotations.ForEach(ann =>
            {
                var annotationTokens = ann.Split('='); //from "DIM_CODE=item_code1"  -> to  [DIM_CODE, item_code1]

                if (!string.Equals(annotationTokens[0], dimensionId, StringComparison.CurrentCultureIgnoreCase))
                {
                    return;
                }

                //the whole codelist is banned
                if (annotationTokens.Length == 1 && distinctDimensionValues.Count == 1)
                {
                    result.Add(dimensionAdapter.Items[0].Id);
                }
                else if (annotationTokens.Length > 1)
                //single codes are banned
                {
                    result.AddAll(annotationTokens[1]
                        .Split('+')); // from "item_code2+item_code3" -> [item_code2, item_code3]
                }
            });

            return result;
        }

        private IEnumerable<string> ExtractNotDisplayedAnnotationValue(string notDiplayedAnnotationName, string dimensionId)
        {
            if (string.IsNullOrEmpty(dimensionId))
            {
                return new List<string>();
            }

            IEnumerable<string> fromDataflow = _dataflow.Annotations
                            .Where(a => a.Type == notDiplayedAnnotationName)
                            .Select(oa => oa.Title)
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

        protected Dictionary<string, string> LabelsFromINameable(INameableObject o)
        {
            var allLabels = new Dictionary<string, string>();

            if (o?.Names != null)
            {
                foreach (var n in o.Names)
                    allLabels[n.Locale] = n.Value;
            }

            if (o != null && allLabels.Count == 0 && o.Name != null)
            {
                allLabels[_defaultLanguage] = o.Name;
            }

            return allLabels;
        }

        protected void InitDimensions()
        {
            _dimensions = new Dictionary<string, IDimensionAdapter>();

            foreach (var dimensionId in DimensionIds)
            {
                var sdmxDimension = _dataStructure.DimensionList.Dimensions.FirstOrDefault(d => d.Id == dimensionId);
                var dimension =
                    new SdmxXmlDimensionAdapter(sdmxDimension, _codelists, _conceptSchemes, _defaultLanguage);
                _dimensions[dimension.Id] = dimension;
            }
        }

        protected IWeightGenerator GetWeightGenerator(string dimensionId, string language)
        {
            var bannedCodes = new HashSet<string>(); //no code ban here

            if (dimensionId == MainTimeDimensionId)
            {
                return new TimeDimensionWeightGenerator(NullLoggerFactory.Instance, bannedCodes);
            }

            var dimension = _dataStructure.DimensionList.Dimensions.FirstOrDefault(d => d.Id == dimensionId);
            var codelist =
                _codelists.FirstOrDefault(c =>
                    dimension != null && c.Id == dimension.Representation?.Representation?.MaintainableId);

            return new OrderAnnotationWeightGenerator<ICode>(
                new SdmxCodelistWrapper(codelist, _dataflow.Annotations, language), bannedCodes);
        }

        private string[] GetGeoDimensionIdsFromAnnotation()
        {
            if (_geoDimensionIds != null)
            {
                return _geoDimensionIds;
            }

            var geoIds = new List<string>();

            foreach (var ann in _dataflow.Annotations)
                if (ann.Id != null && ann.Id.Equals(_config.GeoAnnotationId) && DimensionIds.Contains(ann.Title) &&
                    geoIds.Contains(ann.Id) == false)
                {
                    geoIds.Add(ann.Title);
                }

            _geoDimensionIds = geoIds.ToArray();

            return _geoDimensionIds;
        }

    }
}