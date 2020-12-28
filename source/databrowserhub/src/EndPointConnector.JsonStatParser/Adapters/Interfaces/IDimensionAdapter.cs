using EndPointConnector.JsonStatParser.Adapters.Commons;

namespace EndPointConnector.JsonStatParser.Adapters.Interfaces
{
    public interface IDimensionAdapter
    {

        //dimension id
        string Id { get; }

        // dimension labels
        LocalizedString Label { get; }

        // dimension items
        IDimensionItem[] Items { get; }

        IDimensionItem GetDimensionItemByCode(string code);

    }
}