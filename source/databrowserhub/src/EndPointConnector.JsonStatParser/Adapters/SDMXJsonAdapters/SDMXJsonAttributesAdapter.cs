using System;
using System.Collections.Generic;
using System.Linq;
using EndPointConnector.JsonStatParser.Adapters.Commons;
using EndPointConnector.JsonStatParser.Adapters.Commons.Attributes;
using EndPointConnector.JsonStatParser.Adapters.Interfaces;
using EndPointConnector.JsonStatParser.Model.SdmxJson;

namespace EndPointConnector.JsonStatParser.Adapters.SdmxJsonAdapters
{
    public class SdmxJsonAttributesAdapter : IAttributesAdapter
    {

        public GenericAttribute[] ObservationAttributes { get; private set; }

        public GenericAttribute[] DatasetAttributes { get; private set; }

        public GenericAttribute[] SeriesAttributes { get; private set; }

        public Dictionary<string[], int?[]> ObservationAttributeIndex { get; }

        public Dictionary<int, int?> DatasetAttributeIndex { get; }

        public Dictionary<string[], int?[]> SeriesAttributeIndex { get; }

        private readonly string _defaultLanguage;

        private readonly SdmxJsonStructure _sdmxStructure;

        public SdmxJsonAttributesAdapter(SdmxJsonStructure sdmxStructure, string defaultLanguage)
        {
            _sdmxStructure = sdmxStructure;
            ObservationAttributeIndex = new Dictionary<string[], int?[]>();
            DatasetAttributeIndex = new Dictionary<int, int?>();
            SeriesAttributeIndex = new Dictionary<string[], int?[]>();
            _defaultLanguage = defaultLanguage;
            Init();
        }

        private static GenericAttribute SdmxJsonAttributeToGenericAttribute(SdmxJsonAttribute attr,
            string defaultLanguage)
        {
            var items = attr.Values.Select(GenericAttributeItemTo).ToArray();
            var result = new GenericAttribute(attr.Id, new LocalizedString(attr.GetAllNames()), items, defaultLanguage);

            if (attr.Relationship != null) {
                result.Relationship = attr.Relationship;
            }

            return result;
        }

        private static GenericAttributeItem GenericAttributeItemTo(SdmxJsonGenericValueWrapper attrItem)
        {
            var allNames = attrItem.GetAllNames();

            // if no name is available, then we use the item Id
            var localizedLabels = allNames != null && allNames.Count > 0
                ? new LocalizedString(allNames)
                : new LocalizedString(attrItem.Id);

            var item = new GenericAttributeItem(attrItem.Id, localizedLabels);

            return item;
        }

        public int FindObservationStatusAttributePosition(string statusAttributeId = "OBS_STATUS")
        {
            var index = Array.FindIndex(ObservationAttributes, w => w.Id == statusAttributeId);

            return index;
        }

        private void Init()
        {
            DatasetAttributes = _sdmxStructure.Attributes.DataSet
                .Select(v => SdmxJsonAttributeToGenericAttribute(v, _defaultLanguage)).ToArray();
            ObservationAttributes = _sdmxStructure.Attributes.Observation
                .Select(v => SdmxJsonAttributeToGenericAttribute(v, _defaultLanguage)).ToArray();
            SeriesAttributes = _sdmxStructure.Attributes.Series
                .Select(v => SdmxJsonAttributeToGenericAttribute(v, _defaultLanguage)).ToArray();
        }

    }
}