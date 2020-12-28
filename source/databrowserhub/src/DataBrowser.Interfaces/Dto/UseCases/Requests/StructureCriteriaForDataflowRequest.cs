using DataBrowser.Interfaces.Dto.UseCases.Responses;
using DataBrowser.Interfaces.Mediator;

namespace DataBrowser.Interfaces.Dto.UseCases.Requests
{
    public class StructureCriteriaForDataflowRequest : IUseCase<StructureCriteriaForDataflowResponse>
    {
        public string DataflowId { get; set; }
        public int? ViewId { get; set; }
    }
}