namespace EndPointConnector.JsonStatParser.Adapters.Commons.Attributes
{
    public class GenericAttributeItem
    {

        public string Id;

        public LocalizedString Label;

        public string ParentId;


        public GenericAttributeItem(string id, LocalizedString label, string parentId)
        {
            Id = id;
            Label = label;
            ParentId = parentId;
        }

        public GenericAttributeItem(string id, LocalizedString label) : this(id, label, null)
        { }

    }
}