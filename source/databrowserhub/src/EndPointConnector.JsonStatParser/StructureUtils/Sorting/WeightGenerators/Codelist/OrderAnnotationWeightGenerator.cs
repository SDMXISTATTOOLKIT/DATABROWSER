using System.Collections.Generic;
using System.Linq;
using EndPointConnector.JsonStatParser.StructureUtils.Sorting.Codelist;
using EndPointConnector.JsonStatParser.StructureUtils.Sorting.Codelist.Wrappers;

namespace EndPointConnector.JsonStatParser.StructureUtils.Sorting.WeightGenerators.Codelist
{
    public class OrderAnnotationWeightGenerator<T> : CodelistWeightGenerator<T>
    {

        //TODO: verificare la presenza di pesi duplicati
        private Dictionary<string, double?> _weights;


        public OrderAnnotationWeightGenerator(ICodelistWrapper<T> codelist, HashSet<string> bannedCodes) : base(
            codelist, bannedCodes)
        {
            _weights = new Dictionary<string, double?>();
            InitWeights();
        }


        public override double? GenerateWeight(string code)
        {
            if (IsCodeBanned(code)) {
                return null;
            }

            if (Codelist == null && !_weights.ContainsKey(code)) {
                _weights[code] = _weights.Count;
            }

            return _weights[code];
        }

        protected void InitWeights()
        {
            if (Codelist == null) {
                return;
            }

            _weights = Codelist.ToDictionary(c => c.Id, c => !IsCodeBanned(c.Id) ? c.Weight : null);

            var duplicates = _weights.Where(x => x.Value != null).GroupBy(x => x.Value).Where(x => x.Count() > 1)
                .ToDictionary(k => k.Key, v => v);

            //remove duplicates
            if (duplicates.Count > 0) {
                const double offset = 0.0001;

                var valuesSet = new HashSet<double?>(_weights.Values.ToList());

                foreach (var (duplicateWeight, duplicateEntry) in duplicates) {
                    var duplicateIds = duplicateEntry.Select(x => x.Key).ToList();
                    duplicateIds.Sort(); // sort by Lexicographical order
                    duplicateIds.RemoveAt(0); // remove the first element

                    foreach (var duplicateId in duplicateIds) {
                        var candidateWeight = duplicateWeight + offset;
                        while (valuesSet.Contains(candidateWeight)) candidateWeight += offset;

                        _weights[duplicateId] = candidateWeight;
                        valuesSet.Add(candidateWeight);
                    }
                }
            }

            HandleTreeStructure();
        }

        protected void HandleTreeStructure()
        {
            if (!Codelist.HasTreeStructure()) {
                return;
            }

            var rootNodes = Codelist.ToTreeStructure();

            SortTreeLevels(rootNodes);

            //visita DFS e ri-assegnamento dei pesi
            var weightCounter = 0.0F;
            foreach (var n in rootNodes) DfsWeight(n, ref weightCounter);

            // si bannano ancora i codici
            var allCodes = _weights.Keys.ToList();
            foreach (var w in allCodes.Where(IsCodeBanned)) _weights[w] = null;
        }


        protected void DfsWeight(TreeNode<T> root, ref float weightCounter)
        {
            var code = root.Id;
            _weights[code] = weightCounter++;

            foreach (var child in root.Children) DfsWeight(child, ref weightCounter);
        }


        protected void SortTreeLevels(List<TreeNode<T>> nList)
        {
            if (nList == null) {
                return;
            }

            //nList = nList.OrderBy( n => _weights[n.Id]).ToList();
            nList.Sort((a, b) =>
            {
                var aW = _weights[a.Id];
                var bW = _weights[b.Id];

                if (aW < 0 && bW < 0) {
                    return aW >= bW ? -1 : 1;
                }

                return aW <= bW ? -1 : 1;
            });

            foreach (var node in nList) SortTreeLevels(node.Children);
        }

    }
}