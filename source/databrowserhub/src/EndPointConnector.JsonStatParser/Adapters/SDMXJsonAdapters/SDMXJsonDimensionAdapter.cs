using System.Collections.Generic;
using EndPointConnector.JsonStatParser.Adapters.Commons;
using EndPointConnector.JsonStatParser.Adapters.Interfaces;
using EndPointConnector.JsonStatParser.Model.SdmxJson;

namespace EndPointConnector.JsonStatParser.Adapters.SdmxJsonAdapters
{
    public class SdmxJsonDimensionAdapter : IDimensionAdapter
    {

        public string Id => Dimension.Id;

        public LocalizedString Label { get; }

        public IDimensionItem[] Items { get; protected set; }

        protected SdmxJsonDimension Dimension;

        private Dictionary<string, IDimensionItem> _itemsCache;

        public SdmxJsonDimensionAdapter(SdmxJsonDimension dimension)
        {
            Dimension = dimension;

            Label = Dimension.Names != null
                ? new LocalizedString(Dimension.Names)
                : new LocalizedString(Dimension.Name);

            InitItems();
        }

        public IDimensionItem GetDimensionItemByCode(string code)
        {
            _itemsCache.TryGetValue(code, out var result);

            return result;
        }

        protected void InitItems()
        {
            _itemsCache = new Dictionary<string, IDimensionItem>();
            Items = new IDimensionItem[Dimension.Values.Count];

            for (var i = 0; i < Items.Length; i++) {
                Items[i] = new SdmxJsonDimensionItem(Dimension.Values[i]);
                _itemsCache[Dimension.Values[i].Id] = Items[i];
            }
        }

    }
}