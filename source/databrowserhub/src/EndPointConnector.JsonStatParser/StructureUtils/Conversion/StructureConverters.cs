using EndPointConnector.JsonStatParser.Adapters.Commons.Attributes;
using EndPointConnector.JsonStatParser.Model.JsonStat.Extensions;

namespace EndPointConnector.JsonStatParser.StructureUtils.Conversion
{
    internal class StructureConverters
    {

        public static Attribute JStatAttributeFromGenericAttribute(GenericAttribute attr, string lang)
        {
            var result = new Attribute {Id = attr.Id, Name = attr.Label.TryGet(lang)};

            if (attr.Relationship != null) {
                result.Relationship = attr.Relationship;
            }

            foreach (var a in attr.Values) {
                var attrItem = new AttributeItem {Id = a.Id, Name = a.Label.TryGet(lang), Parent = a.ParentId};
                result.Values.Add(attrItem);
            }

            return result;
        }

    }
}