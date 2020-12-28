using System;
using System.Linq;
using EndPointConnector.JsonStatParser.Adapters.Commons;
using EndPointConnector.JsonStatParser.Adapters.Interfaces;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;

namespace EndPointConnector.JsonStatParser.Adapters.SdmxXmlAdapters
{
    public class SdmxXmlDimensionItem : IDimensionItem
    {

        public string Id => _rawValue.Id;

        public LocalizedString Label { get; }

        private readonly ICode _rawValue;

        private readonly Type _rawValueType;

        public SdmxXmlDimensionItem(ICode item)
        {
            _rawValue = item;
            _rawValueType = item.GetType();

            if (item.Names != null && item.Names.Count > 0) {
                Label = new LocalizedString(item.Names.ToDictionary(x => x.Locale, x => x.Value));
            }
            else if (item.Name != null) {
                Label = new LocalizedString(item.Name);
            }
            else {
                Label = new LocalizedString(item.Id);
            }
        }

        public object GetRawValue()
        {
            return _rawValue;
        }

        public Type GetRawValueType()
        {
            return _rawValueType;
        }

    }
}