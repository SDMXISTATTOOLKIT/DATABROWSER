using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EndPointConnector.JsonStatParser.Model.SdmxJson;

namespace EndPointConnector.JsonStatParser.StructureUtils.Sorting.Codelist.Wrappers
{
    internal class SdmxJsonCodelistWrapper : ICodelistWrapper<SdmxJsonGenericValueWrapper>
    {

        private readonly List<SdmxJsonAnnotation> _annotations;

        private readonly string _currentLanguage;

        private readonly SdmxJsonDimension _dimension;

        private readonly List<TreeNode<SdmxJsonGenericValueWrapper>> _elementCache;

        private readonly List<TreeNode<SdmxJsonGenericValueWrapper>> _rootElements;


        public SdmxJsonCodelistWrapper(SdmxJsonDimension dimension, List<SdmxJsonAnnotation> annotations,
            string currentLanguage)
        {
            _dimension = dimension;
            _annotations = annotations;
            _rootElements = new List<TreeNode<SdmxJsonGenericValueWrapper>>();
            _elementCache = new List<TreeNode<SdmxJsonGenericValueWrapper>>();
            _currentLanguage = currentLanguage;
        }


        public IEnumerator<TreeNode<SdmxJsonGenericValueWrapper>> GetEnumerator()
        {
            for (var i = 0; i < _dimension.Values.Count; i++) {
                var val = _dimension.Values[i];

                var defaultOrder = val.Order ?? -(i + 1);
                double? weight = GetCodeWeightByOrderAnnotation(val, defaultOrder);

                if (_elementCache.Count != _dimension.Values.Count) {
                    var elementId = val.Id ?? val.GetLocalizedName(_currentLanguage);
                    _elementCache.Insert(i, new TreeNode<SdmxJsonGenericValueWrapper>(weight, elementId, val));
                }

                yield return _elementCache[i];
                //var codeListElement = new CodelistElement<SDMXJsonGenericValueWrapper>(weight, val.Id, val);
                //yield return codeListElement;
            }
        }


        public bool HasTreeStructure()
        {
            return _dimension.Values.FirstOrDefault(item =>
                item.Parent != null &&
                _dimension.Values.FirstOrDefault(parentItem => item.Parent == parentItem.Id) != null) != null;
        }

        public List<TreeNode<SdmxJsonGenericValueWrapper>> ToTreeStructure()
        {
            if (_rootElements.Count > 0) {
                return _rootElements;
            }

            var treeNodesMap = this.ToDictionary(c => c.Id);

            foreach (var treeNode in this)
                //has no parent or parent is not in the codelist
                if (treeNode.Value.Parent == null || !treeNodesMap.ContainsKey(treeNode.Value.Parent)) {
                    _rootElements.Add(treeNode);
                }
                else if (treeNode.Value.Parent != null && treeNodesMap.ContainsKey(treeNode.Value.Parent)) {
                    var parentNode = treeNodesMap[treeNode.Value.Parent];
                    parentNode.AddNode(treeNode);
                }

            return _rootElements;
        }


        protected string GetAnnotationText(SdmxJsonAnnotation ann)
        {
            return ann.GetLocalizedText(_currentLanguage);
        }


        private float GetCodeWeightByOrderAnnotation(SdmxJsonGenericValueWrapper val, float defaultValue)
        {
            if (val?.Annotations == null) {
                return defaultValue;
            }

            var orderAnnotation = val.Annotations.Select(idx => _annotations[idx])
                .FirstOrDefault(a => a.Type == TypeEnum.Order);

            if (orderAnnotation != null && float.TryParse(GetAnnotationText(orderAnnotation), NumberStyles.Float,
                CultureInfo.InvariantCulture, out var result)) {
                return result;
            }

            return defaultValue;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

    }
}