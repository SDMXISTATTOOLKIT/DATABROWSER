using System;
using EndPointConnector.JsonStatParser.Adapters.Commons;

namespace EndPointConnector.JsonStatParser.Adapters.Interfaces
{
    public interface IDimensionItem
    {

        //dimension item id
        string Id { get; }

        //dimension item localized labels
        LocalizedString Label { get; }

        //original value wrapped by this class
        object GetRawValue();

        Type GetRawValueType();

    }
}