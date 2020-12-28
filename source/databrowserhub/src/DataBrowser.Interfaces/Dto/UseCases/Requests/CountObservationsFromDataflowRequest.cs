using System.Collections.Generic;
using DataBrowser.Interfaces.Dto.UseCases.Responses;
using DataBrowser.Interfaces.Mediator;
using EndPointConnector.Models;

namespace DataBrowser.Interfaces.Dto.UseCases.Requests
{
    public class CountObservationsFromDataflowRequest : IUseCase<CountObservationsFromDataflowResponse>
    {
        public string DataflowId { get; set; }

        public List<FilterCriteria> DataCriterias { get; set; }
    }
}