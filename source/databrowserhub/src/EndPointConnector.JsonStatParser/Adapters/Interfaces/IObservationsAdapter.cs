using System.Collections.Generic;

namespace EndPointConnector.JsonStatParser.Adapters.Interfaces
{
    public interface IObservationsAdapter : IEnumerable<IndexedObservation>
    {

        HashSet<string>[] DistinctDimensionsCodes { get; }

        Dictionary<string[], IndexedObservation> Values { get; }

        IAttributesAdapter Attributes { get; }

    }
}