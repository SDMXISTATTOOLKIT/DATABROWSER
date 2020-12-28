using System.Collections.Generic;
using System.Linq;

namespace EndPointConnector.JsonStatParser.StructureUtils.Sorting.WeightGenerators
{
    public abstract class WeightGenerator : IWeightGenerator
    {

        public HashSet<string> BannedCodes { get; protected set; }

        protected WeightGenerator(IEnumerable<string> bannedCodes)
        {
            BannedCodes = new HashSet<string>(bannedCodes.Where(code => code != null).Select(code => code.ToLower()));
        }

        public abstract double? GenerateWeight(string code);

        public bool IsCodeBanned(string val)
        {
            return string.IsNullOrEmpty(val) || BannedCodes.Contains(val.ToLower());
        }

    }
}