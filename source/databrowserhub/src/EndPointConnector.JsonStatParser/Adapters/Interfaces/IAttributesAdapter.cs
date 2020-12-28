using System.Collections.Generic;
using EndPointConnector.JsonStatParser.Adapters.Commons.Attributes;

namespace EndPointConnector.JsonStatParser.Adapters.Interfaces
{
    public interface IAttributesAdapter
    {

        GenericAttribute[] ObservationAttributes { get; }

        GenericAttribute[] DatasetAttributes { get; }

        GenericAttribute[] SeriesAttributes { get; }

        Dictionary<string[], int?[]> ObservationAttributeIndex { get; }

        Dictionary<int, int?> DatasetAttributeIndex { get; }

        Dictionary<string[], int?[]> SeriesAttributeIndex { get; }

        int FindObservationStatusAttributePosition(string statusAttributeId = "OBS_STATUS");

    }
}