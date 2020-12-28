using System.Collections.Generic;
using DataBrowser.Interfaces.Dto.UseCases.Responses;
using DataBrowser.Interfaces.Mediator;
using EndPointConnector.Models;

namespace DataBrowser.Interfaces.Dto.UseCases.Requests
{
    public class DownloadDataflowFromNodeEndPointRequest : IUseCase<DownloadDataflowFromNodeEndPointResponse>
    {
        public DownloadDataflowFromNodeEndPointRequest(string dataflowId,
            string format,
            bool requireResponseFileCompress,
            List<FilterCriteria> dataCriterias)
        {
            DataflowId = dataflowId;
            DataFormat = format;
            RequireResponseFileCompress = requireResponseFileCompress;
            DataCriterias = dataCriterias;
        }

        public string DataflowId { get; set; }
        public string DataFormat { get; set; }
        public bool RequireResponseFileCompress { get; set; }
        public List<FilterCriteria> DataCriterias { get; set; }
    }
}