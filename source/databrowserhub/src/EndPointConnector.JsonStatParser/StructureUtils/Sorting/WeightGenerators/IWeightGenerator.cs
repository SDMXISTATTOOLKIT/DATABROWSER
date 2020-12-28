using System.Collections.Generic;
using System.Linq;

namespace EndPointConnector.JsonStatParser.StructureUtils.Sorting.WeightGenerators
{
    public interface IWeightGenerator
    {

        double? GenerateWeight(string code);

        bool IsCodeBanned(string val);

        public Dictionary<string, int> GenerateSortedIndex(string[] values)
        {
            var sortedCodes = values.OrderBy(GenerateWeight);

            return sortedCodes.Select((value, index) => (value, index)).ToDictionary(v => v.value, v => v.index);
        }

    }
}