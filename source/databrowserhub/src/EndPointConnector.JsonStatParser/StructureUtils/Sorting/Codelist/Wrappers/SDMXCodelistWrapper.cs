using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EndPointConnector.JsonStatParser.StructureUtils.Annotations;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;

namespace EndPointConnector.JsonStatParser.StructureUtils.Sorting.Codelist.Wrappers
{
    public class SdmxCodelistWrapper : ICodelistWrapper<ICode>
    {

        private const string DefaultLang = "en";

        public IList<IAnnotation> Annotations { get; }

        private readonly ICodelistObject _codelist;

        private readonly string _currentLanguage;

        private readonly List<TreeNode<ICode>> _elementCache;

        private readonly bool _hasOrderAnnotation;

        private readonly Dictionary<string, float> _originalPositions;

        private readonly List<TreeNode<ICode>> _rootElements;

        public SdmxCodelistWrapper(ICodelistObject codelist, IList<IAnnotation> annotations,
            string currentLanguage)
        {
            _codelist = codelist ??
                        throw new Exception(
                            "Cannot create a SDMXCodelistWrapper from  a null ICodelistObject instance.");
            _elementCache = new List<TreeNode<ICode>>();
            _rootElements = new List<TreeNode<ICode>>();
            _currentLanguage = currentLanguage;
            Annotations = annotations;
            _originalPositions = new Dictionary<string, float>();
            _hasOrderAnnotation =
                _codelist.Items.FirstOrDefault(code => GetOrderAnnotationFromCode(code) != null) != null;
            InitOriginalPositionMap();
        }

        protected static IAnnotation GetOrderAnnotationFromCode(ICode code)
        {
            return code.Annotations.FirstOrDefault(x => x.Type.ToLower() == "order");
        }

        public IEnumerator<TreeNode<ICode>> GetEnumerator()
        {
            for (var i = 0; i < _codelist.Items.Count; i++) {
                var currentVal = _codelist.Items[i];

                if (_elementCache.Count != _codelist.Items.Count) {
                    var defaultValue = _hasOrderAnnotation ? -(i + 1) : _originalPositions[currentVal.Id];
                    var weight =
                        GetCodeWeightByOrderAnnotation(currentVal, defaultValue, _currentLanguage, DefaultLang);
                    _elementCache.Insert(i, new TreeNode<ICode>(weight, currentVal.Id, currentVal));
                }

                yield return _elementCache[i];

                //yield return new CodelistElement<ICode>(i, currentVal.Id, currentVal);
            }
        }

        public bool HasTreeStructure()
        {
            return _codelist.Items.FirstOrDefault(item =>
                item.ParentCode != null &&
                _codelist.Items.FirstOrDefault(parentItem => parentItem.Id == item.ParentCode) != null) != null;
        }

        public List<TreeNode<ICode>> ToTreeStructure()
        {
            if (_rootElements.Count > 0) {
                return _rootElements;
            }

            var treeNodesMap = new Dictionary<string, TreeNode<ICode>>();

            foreach (var treeNode in this) // ... in this :o
            {
                if (treeNode.Value.ParentCode == null) {
                    _rootElements.Add(treeNode);
                }

                treeNodesMap[treeNode.Id] = treeNode;
            }

            foreach (var treeNode in this)
                if (treeNode.Value.ParentCode != null) {
                    var parentNode = treeNodesMap[treeNode.Value.ParentCode];
                    parentNode.AddNode(treeNode);
                }

            return _rootElements;
        }

        protected float GetCodeWeightByOrderAnnotation(ICode code, float defaultValue, string lang, string defaultLang)
        {
            var orderAnnotation = GetOrderAnnotationFromCode(code);
            var annotationLocalizedText =
                AnnotationUtils.GetLocalizedAnnotationText(orderAnnotation, lang, defaultLang);

            if (annotationLocalizedText != null && float.TryParse(annotationLocalizedText, NumberStyles.Float,
                CultureInfo.InvariantCulture, out var result)) {
                return result;
            }

            return defaultValue;
        }

        private void InitOriginalPositionMap()
        {
            for (var i = 0; i < _codelist.Items.Count; i++) _originalPositions[_codelist.Items[i].Id] = i;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

    }
}