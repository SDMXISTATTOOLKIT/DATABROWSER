using EndPointConnector.Models.Dto;

namespace DataBrowser.Interfaces.Dto.UseCases.Responses
{
    public class GetNodeCatalogTreeResponse
    {
        public NodeCatalogDto NodeCatalogDto { get; set; }

        public string NodeCode { get; set; }
    }
}