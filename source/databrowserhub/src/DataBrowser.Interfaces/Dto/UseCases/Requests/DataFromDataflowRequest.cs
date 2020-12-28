using System.Collections.Generic;
using DataBrowser.Interfaces.Dto.UseCases.Responses;
using DataBrowser.Interfaces.Mediator;
using EndPointConnector.Models;

namespace DataBrowser.Interfaces.Dto.UseCases.Requests
{
    public class DataFromDataflowRequest : IUseCase<GetDataFromDataflowResponse>
    {
        public string DataflowId { get; set; }
        public List<FilterCriteria> DataCriterias { get; set; }
        public bool TryUseCompatibleKeyCache { get; set; } = true;
    }
}