using System.Collections.Generic;

namespace EndPointConnector.JsonStatParser.StructureUtils.Sorting.WeightGenerators
{
    public class SimpleWeightGenerator : WeightGenerator
    {

        private readonly Dictionary<string, double?> _weightCache = new Dictionary<string, double?>();

        public SimpleWeightGenerator(IEnumerable<string> bannedCodes) : base(bannedCodes)
        { }


        public override double? GenerateWeight(string code)
        {
            if (code == null || IsCodeBanned(code)) {
                return null;
            }

            if (_weightCache.ContainsKey(code) == false) {
                _weightCache[code] = _weightCache.Count;
            }

            return _weightCache[code];
        }

    }
}