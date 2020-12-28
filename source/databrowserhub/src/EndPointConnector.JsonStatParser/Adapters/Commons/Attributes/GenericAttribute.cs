using System.Collections.Generic;
using System.Linq;

namespace EndPointConnector.JsonStatParser.Adapters.Commons.Attributes
{
    public class GenericAttribute
    {

        public GenericAttributeItem[] Values { get; protected set; }

        public bool AutoAdd { get; protected set; }

        public string Id;

        public LocalizedString Label;

        public Dictionary<string, object> Relationship;

        private readonly Dictionary<string, int> _attributeIdToPosition;

        private readonly string _defaultLanguage;


        public GenericAttribute(string id, LocalizedString label, GenericAttributeItem[] values, string defaultLanguage)
        {
            Id = id;
            Label = label;
            _defaultLanguage = defaultLanguage ?? "en";

            if (values == null || values.Length == 0) {
                Values = new GenericAttributeItem[0];
                AutoAdd = true;
            }
            else {
                Values = values;
                AutoAdd = false;
            }

            _attributeIdToPosition =
                Values.Select((val, index) => (val, index)).ToDictionary(x =>
                {
                    GenericAttributeItem val;
                    (val, _) = x;

                    return val?.Id;
                }, x => x.index);
        }

        public int GetAttributePosition(string attributeId)
        {
            while (true) {
                if (attributeId == null) {
                    return -1;
                }

                if (_attributeIdToPosition.TryGetValue(attributeId, out var result)) {
                    return result;
                }

                if (!AutoAdd) {
                    return -1;
                }

                AddNewItem(attributeId);
            }
        }

        protected void AddNewItem(string id)
        {
            if (_attributeIdToPosition.ContainsKey(id)) {
                return;
            }

            var newItem = new GenericAttributeItem(id, new LocalizedString(id, _defaultLanguage));
            Values = Values.Append(newItem).ToArray();
            _attributeIdToPosition[id] = _attributeIdToPosition.Count;
        }

    }
}