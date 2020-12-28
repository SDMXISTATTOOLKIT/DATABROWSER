using System.Security.Claims;
using DataBrowser.Interfaces.Dto.UseCases.Responses;
using DataBrowser.Interfaces.Mediator;

namespace DataBrowser.Interfaces.Dto.UseCases.Requests
{
    public class HubAndMinimalInfoRequest : IUseCase<GetHubAndMinimalInfoResponse>
    {
        public int HubId { get; set; }
        public bool FilterByPermissionUser { get; set; }
        public ClaimsPrincipal FilterBySpecificUser { get; set; }
    }
}