using System;
using EndPointConnector.JsonStatParser.Adapters.Commons;
using EndPointConnector.JsonStatParser.Adapters.Interfaces;
using EndPointConnector.JsonStatParser.Model.SdmxJson;

namespace EndPointConnector.JsonStatParser.Adapters.SdmxJsonAdapters
{
    public class SdmxJsonDimensionItem : IDimensionItem
    {

        public string Id => RawValue.Id;

        public LocalizedString Label { get; }

        protected SdmxJsonGenericValueWrapper RawValue;

        protected Type RawValueType;

        public SdmxJsonDimensionItem(SdmxJsonGenericValueWrapper item)
        {
            RawValue = item;
            RawValueType = item.GetType();

            var localizedNames = item.Names;

            if ((localizedNames == null || localizedNames.Count == 0) && item.Name != null) {
                Label = new LocalizedString(item.Name);
            }
            else if ((localizedNames == null || localizedNames.Count == 0) && item.Name == null) {
                Label = new LocalizedString(item.Id);
            }
            else {
                Label = new LocalizedString(localizedNames);
            }
        }

        public object GetRawValue()
        {
            return RawValue;
        }

        public Type GetRawValueType()
        {
            return RawValueType;
        }

    }
}