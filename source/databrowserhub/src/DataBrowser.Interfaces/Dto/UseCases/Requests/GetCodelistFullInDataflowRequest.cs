using System.Collections.Generic;
using DataBrowser.Interfaces.Dto.UseCases.Responses;
using DataBrowser.Interfaces.Mediator;

namespace DataBrowser.Interfaces.Dto.UseCases.Requests
{
    public class GetCodelistFullInDataflowRequest : IUseCase<GetCodelistFullInDataflowResponse>
    {
        public string DataflowId { get; set; }

        public List<string> DimensionIds { get; set; }
    }
}