using System.Collections.Generic;
using System.Linq;
using EndPointConnector.JsonStatParser.Model.JsonStat;

namespace EndPointConnector.JsonStatParser.StructureUtils.Sorting.WeightGenerators.JsonStatCategory
{
    public class JsonStatCategoryWeightGenerator : WeightGenerator
    {

        private readonly JsonStatDimensionCategory _category;

        public JsonStatCategoryWeightGenerator(JsonStatDimensionCategory category, ICollection<string> allowedCodes) :
            base(
                CalculateBannedCodes(category, allowedCodes))
        {
            _category = category;
        }

        public JsonStatCategoryWeightGenerator(JsonStatDimensionCategory category) : base(new HashSet<string>())
        {
            _category = category;
        }

        private static IEnumerable<string> CalculateBannedCodes(JsonStatDimensionCategory category,
            ICollection<string> allowed)
        {
            var banned = category.Index.Keys.Where(code => !allowed.Contains(code));

            return new HashSet<string>(banned);
        }

        public override double? GenerateWeight(string code)
        {
            if (code == null || IsCodeBanned(code) || !_category.Index.ContainsKey(code)) {
                return null;
            }

            return _category.Index[code];
        }

    }
}