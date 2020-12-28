using System;
using System.Collections.Generic;
using System.Linq;
using EndPointConnector.JsonStatParser.Adapters.Commons;
using EndPointConnector.JsonStatParser.Adapters.Commons.Attributes;
using EndPointConnector.JsonStatParser.Adapters.Interfaces;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;

namespace EndPointConnector.JsonStatParser.Adapters.SdmxXmlAdapters
{
    public class SdmxXmlAttributesAdapter : IAttributesAdapter
    {

        public GenericAttribute[] ObservationAttributes { get; private set; }

        public GenericAttribute[] DatasetAttributes { get; private set; }

        public GenericAttribute[] SeriesAttributes { get; private set; }

        public Dictionary<string[], int?[]> ObservationAttributeIndex { get; }

        public Dictionary<int, int?> DatasetAttributeIndex { get; }

        public Dictionary<string[], int?[]> SeriesAttributeIndex { get; }

        protected ISet<ICodelistObject> Codelists;

        private readonly ISet<IConceptSchemeObject> _conceptSchemes;

        private readonly IDataStructureObject _dataStructure;

        private readonly string _defaultLanguage;

        private Dictionary<string, int> _datasetAttributesPositionsByCode;

        private Dictionary<string, int> _observationAttributesPositionsByCode;

        private Dictionary<string, int> _seriesAttributesPositionsByCode;

        public SdmxXmlAttributesAdapter(IDataStructureObject dataStructure, ISet<ICodelistObject> codelists,
            ISet<IConceptSchemeObject> conceptSchemes,
            Dictionary<string[], Dictionary<string, string>> observationAttributesIndex,
            Dictionary<string, string> datasetAttributesIndex,
            Dictionary<string[], Dictionary<string, string>> seriesAttributesIndex, string defaultLanguage)
        {
            _defaultLanguage = defaultLanguage;
            _dataStructure = dataStructure;
            Codelists = codelists;
            _conceptSchemes = conceptSchemes;
            ObservationAttributeIndex = new Dictionary<string[], int?[]>();
            DatasetAttributeIndex = new Dictionary<int, int?>();
            SeriesAttributeIndex = new Dictionary<string[], int?[]>();
            InitAllAttributes();
            InitDatasetAttributesIndex(datasetAttributesIndex);
            InitSeriesAttributesIndex(seriesAttributesIndex);
            InitObservationAttributesIndex(observationAttributesIndex);
        }

        protected static int? GetAttributePosition(GenericAttribute[] attributes, string attributeName,
            string attributeCode)
        {
            var attribute = attributes.FirstOrDefault(x =>
                x.Id.Equals(attributeName, StringComparison.InvariantCultureIgnoreCase));

            if (attribute == null) {
                return null;
            }

            int? position = attribute.GetAttributePosition(attributeCode);

            return position < 0 ? null : position;
        }

        public int FindObservationStatusAttributePosition(string statusAttributeId = "OBS_STATUS")
        {
            var index = Array.FindIndex(ObservationAttributes, w => w.Id == statusAttributeId);

            return index;
        }

        protected void InitSeriesAttributesIndex(Dictionary<string[], Dictionary<string, string>> seriesAttributesIndex)
        {
            var allAttributesCounter = SeriesAttributes.Length;

            foreach (var (seriesIndex, seriesValues) in seriesAttributesIndex) {
                var attributesPositions = new int?[allAttributesCounter];

                foreach (var (attrId, attrItemCode) in seriesValues) {
                    var attributePosition = _seriesAttributesPositionsByCode[attrId];
                    var attrItemPosition = GetAttributePosition(SeriesAttributes, attrId, attrItemCode);

                    attributesPositions[attributePosition] = attrItemPosition;
                }

                if (attributesPositions.Any(x => x != null)) {
                    SeriesAttributeIndex[seriesIndex] = attributesPositions;
                }
            }
        }

        protected void InitDatasetAttributesIndex(Dictionary<string, string> datasetAttributesIndex)
        {
            if (datasetAttributesIndex == null) {
                return;
            }

            foreach (var (attrId, attrCode) in datasetAttributesIndex) {
                var attrPosition = _datasetAttributesPositionsByCode[attrId];
                var attrCodePosition = GetAttributePosition(DatasetAttributes, attrId, attrCode);
                DatasetAttributeIndex[attrPosition] = attrCodePosition;
            }
        }

        protected void InitObservationAttributesIndex(
            Dictionary<string[], Dictionary<string, string>> observationAttributesIndex)
        {
            var allAttributesCounter = ObservationAttributes.Length;

            foreach (var (observationIndex, observationValues) in observationAttributesIndex) {
                var attributesPositions = new int?[allAttributesCounter];

                foreach (var (attributeCode, attrItemCode) in observationValues) {
                    var attributePosition = _observationAttributesPositionsByCode[attributeCode];
                    var attrItemPosition = GetAttributePosition(ObservationAttributes, attributeCode, attrItemCode);

                    attributesPositions[attributePosition] = attrItemPosition;
                }

                if (attributesPositions.Any(x => x != null)) {
                    ObservationAttributeIndex[observationIndex] = attributesPositions;
                }
            }
        }

        protected LocalizedString NameableToLocalizedString(INameableObject nameable)
        {
            if (nameable == null) {
                return null;
            }

            if (nameable.Names == null) {
                return nameable.Name != null
                    ? new LocalizedString(nameable.Name, _defaultLanguage)
                    : new LocalizedString(nameable.Id);
            }

            var concLabels = nameable.Names.ToDictionary(x => x.Locale.ToLower(), x => x.Value);

            return new LocalizedString(concLabels);
        }


        private void InitAllAttributes()
        {
            var datasetAttributesList = new List<GenericAttribute>();
            var observationAttributesList = new List<GenericAttribute>();
            var seriesAttributesList = new List<GenericAttribute>();

            foreach (var attr in _dataStructure.Attributes) {
                var attributeId = attr.Id;
                var label = GetAttributeLabel(attr);
                var items = GetAttributeValues(attr);
                var convertedAttribute = new GenericAttribute(attributeId, label, items, _defaultLanguage);

                switch (attr.AttachmentLevel) {
                    case AttributeAttachmentLevel.DataSet:
                        datasetAttributesList.Add(convertedAttribute);

                        break;
                    case AttributeAttachmentLevel.Observation:
                        observationAttributesList.Add(convertedAttribute);

                        break;
                    case AttributeAttachmentLevel.DimensionGroup: {
                        if (attr.DimensionReferences?.Count == 1) {
                            convertedAttribute.Relationship = new Dictionary<string, object>
                            {
                                ["dimensions"] = new[] {attr.DimensionReferences.First()}
                            };
                        }

                        seriesAttributesList.Add(convertedAttribute);
                        observationAttributesList.Add(convertedAttribute);

                        break;
                    }
                    case AttributeAttachmentLevel.Group: {
                        var relationships = GetAttributeRelationships(attr);

                        if (relationships != null) {
                            convertedAttribute.Relationship = relationships;
                        }

                        seriesAttributesList.Add(convertedAttribute);

                        break;
                    }
                    case AttributeAttachmentLevel.Null:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            DatasetAttributes = datasetAttributesList.ToArray();
            ObservationAttributes = observationAttributesList.ToArray();
            SeriesAttributes = seriesAttributesList.ToArray();

            _datasetAttributesPositionsByCode = DatasetAttributes.Select((val, index) => (val.Id, index))
                .ToDictionary(x => x.Id, x => x.index);
            _observationAttributesPositionsByCode = ObservationAttributes.Select((val, index) => (val.Id, index))
                .ToDictionary(x => x.Id, x => x.index);
            _seriesAttributesPositionsByCode = SeriesAttributes.Select((val, index) => (val.Id, index))
                .ToDictionary(x => x.Id, x => x.index);
        }

        private Dictionary<string, object> GetAttributeRelationships(IAttributeObject attr)
        {
            var groupId = attr.AttachmentGroup;

            var groupDimensions = _dataStructure.Groups?.Where(x => x.Id == groupId).SelectMany(x => x.DimensionRefs)
                .ToArray();

            if (groupDimensions == null) {
                return null;
            }

            var result = new Dictionary<string, object> {["dimensions"] = groupDimensions};

            return result;
        }

        private GenericAttributeItem[] GetAttributeValues(IComponent attr)
        {
            if (attr.Representation?.Representation?.MaintainableReference == null) {
                return new GenericAttributeItem[0];
            }

            var attributeCodelistAgencyId = attr.Representation.Representation.MaintainableReference.AgencyId;
            var attributeCodelistId = attr.Representation.Representation.MaintainableReference.MaintainableId;
            var attributeCodelistVersion = attr.Representation.Representation.MaintainableReference.Version;

            var codelist = Codelists
                .Where(cs => cs.AgencyId == attributeCodelistAgencyId && cs.Version == attributeCodelistVersion)
                .FirstOrDefault(x => x.Id == attributeCodelistId);

            return GetCodelistItems(codelist);
        }

        private GenericAttributeItem[] GetCodelistItems(ICodelistObject codelist)
        {
            return codelist.Items.Select(x => new GenericAttributeItem(x.Id, NameableToLocalizedString(x))).ToArray();
        }

        private LocalizedString GetAttributeLabel(IComponent attr)
        {
            var attributeConceptAgencyId = attr.ConceptRef.MaintainableReference.AgencyId;
            var attributeConceptId = attr.Id;
            var attributeConceptVersion = attr.ConceptRef.MaintainableReference.Version;

            return GetConceptLabel(attributeConceptAgencyId, attributeConceptId, attributeConceptVersion);
        }

        private LocalizedString GetConceptLabel(string attributeConceptAgencyId, string attributeConceptId,
            string attributeConceptVersion)
        {
            var concept = _conceptSchemes
                .Where(cs => cs.AgencyId == attributeConceptAgencyId && cs.Version == attributeConceptVersion)
                .SelectMany(x => x.Items)
                .FirstOrDefault(x => x.Id == attributeConceptId);

            return concept != null ? NameableToLocalizedString(concept) : new LocalizedString(attributeConceptId);
        }

    }
}