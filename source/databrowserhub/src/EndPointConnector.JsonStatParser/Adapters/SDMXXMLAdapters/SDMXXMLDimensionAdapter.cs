using System.Collections.Generic;
using System.Linq;
using EndPointConnector.JsonStatParser.Adapters.Commons;
using EndPointConnector.JsonStatParser.Adapters.Interfaces;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;

namespace EndPointConnector.JsonStatParser.Adapters.SdmxXmlAdapters
{
    public class SdmxXmlDimensionAdapter : IDimensionAdapter
    {

        public ICodelistObject CodelistReference { get; protected set; }

        public IConceptObject ConceptReference { get; protected set; }

        public string Id => _dimension.Id;

        public LocalizedString Label { get; private set; }

        public IDimensionItem[] Items { get; protected set; }

        private readonly ISet<ICodelistObject> _codelists;

        private readonly ISet<IConceptSchemeObject> _conceptSchemes;

        private readonly string _defaultLanguage;

        private readonly IDimension _dimension;

        private Dictionary<string, IDimensionItem> _itemsCache;

        public SdmxXmlDimensionAdapter(IDimension dimension, ISet<ICodelistObject> codelists,
            ISet<IConceptSchemeObject> conceptSchemes, string defaultLanguage)
        {
            _dimension = dimension;
            _conceptSchemes = conceptSchemes;
            _defaultLanguage = defaultLanguage;
            _codelists = codelists;
            InitDimensionsReferences();
            InitDimensionLabels();
            InitItems();
        }

        public IDimensionItem GetDimensionItemByCode(string code)
        {
            _itemsCache.TryGetValue(code, out var result);

            return result;
        }

        protected void InitDimensionsReferences()
        {
            if (_dimension.ConceptRef != null) {
                var conceptId = _dimension.ConceptRef.FullId;
                var conceptAgency = _dimension.ConceptRef.AgencyId;
                var conceptVersion = _dimension.ConceptRef.Version;

                ConceptReference = _conceptSchemes
                    .Where(cs => cs.AgencyId == conceptAgency && cs.Version == conceptVersion)
                    .SelectMany(x => x.Items)
                    .FirstOrDefault(x => x.Id == conceptId);
            }

            if (_dimension.Representation?.Representation?.MaintainableId != null) {
                CodelistReference = _codelists.FirstOrDefault(cdl =>
                    cdl.Id == _dimension.Representation.Representation.MaintainableId);
            }
        }

        protected void InitDimensionLabels()
        {
            var labelsFromConcept = NameableToDictionary(ConceptReference);
            var labelsFromCodelist = NameableToDictionary(CodelistReference);

            if (labelsFromConcept.Count > 0 || labelsFromCodelist.Count > 0) {
                var merged = labelsFromConcept.Concat(labelsFromCodelist)
                    .ToLookup(x => x.Key, x => x.Value)
                    .ToDictionary(x => x.Key, g => g.First());
                Label = new LocalizedString(merged);
            }
            else {
                Label = new LocalizedString(_dimension.Id, _defaultLanguage);
            }
        }


        protected void InitItems()
        {
            if (_itemsCache != null) {
                return;
            }

            // items can be initialized from a codelist
            _itemsCache = new Dictionary<string, IDimensionItem>();

            if (CodelistReference == null) {
                return;
            }

            Items = new IDimensionItem[CodelistReference.Items.Count];

            for (var i = 0; i < Items.Length; i++) {
                var code = CodelistReference.Items[i];
                Items[i] = new SdmxXmlDimensionItem(code);
                _itemsCache[code.Id] = Items[i];
            }
        }


        private Dictionary<string, string> NameableToDictionary(INameableObject nameable)
        {
            var result = new Dictionary<string, string>();

            if (nameable == null) {
                return result;
            }

            if (nameable.Names != null) {
                result = nameable.Names.ToDictionary(x => x.Locale.ToLower(), x => x.Value);
            }
            else if (nameable.Name != null) {
                result[_defaultLanguage] = nameable.Name;
            }

            return result;
        }

    }
}