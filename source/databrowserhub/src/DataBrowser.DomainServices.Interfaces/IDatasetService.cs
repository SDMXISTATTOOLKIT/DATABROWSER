using DataBrowser.Domain.Dtos;
using EndPointConnector.Models;

namespace DataBrowser.DomainServices.Interfaces
{
    public interface IDatasetService
    {
        Dataset CreateDataset(HubDto hub, NodeDto node, Dataflow dataflow, Dsd dsd, string lang);
        Dataset CreateDataset(Dataflow dataflow, Dsd dsd, string lang);
    }
}
