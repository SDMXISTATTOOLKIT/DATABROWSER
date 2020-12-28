using EndPointConnector.JsonStatParser.Adapters.Interfaces;
using EndPointConnector.JsonStatParser.Model.JsonStat;

namespace EndPointConnector.JsonStatParser.Plugins
{
    public interface IConverterPlugin
    {

        void Execute(IDatasetStructureAdapter datasetStructure, IObservationsAdapter datasetObservations,
            JsonStatDataset jsonStatDataset);

    }
}