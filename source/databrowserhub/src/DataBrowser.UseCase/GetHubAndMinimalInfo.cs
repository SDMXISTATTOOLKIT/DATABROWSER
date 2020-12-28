using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DataBrowser.AC.Responses.Services;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Dto.UseCases.Requests;
using DataBrowser.Interfaces.Dto.UseCases.Responses;
using DataBrowser.Interfaces.Mediator;
using DataBrowser.Query.Dashboards;
using DataBrowser.Query.Hubs;
using DataBrowser.Query.Nodes;
using DataBrowser.Services.Interfaces;
using EndPointConnector.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DataBrowser.UseCase
{
    public class GetHubAndMinimalInfo : IUseCaseHandler<HubAndMinimalInfoRequest, GetHubAndMinimalInfoResponse>
    {
        private readonly IEndPointConnectorFactory _endPointConnectorFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GetHubAndMinimalInfo> _logger;
        private readonly IMapper _mapper;
        private readonly IMediatorService _mediatorService;
        private readonly IRequestContext _requestContext;

        public GetHubAndMinimalInfo(ILogger<GetHubAndMinimalInfo> logger,
            IRequestContext requestContext,
            IEndPointConnectorFactory endPointConnectorFactory,
            IMapper mapper,
            IMediatorService mediatorService,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _requestContext = requestContext;
            _endPointConnectorFactory = endPointConnectorFactory;
            _mapper = mapper;
            _mediatorService = mediatorService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<GetHubAndMinimalInfoResponse> Handle(HubAndMinimalInfoRequest request,
            CancellationToken cancellationToken)
        {
            _logger.LogDebug("Start Handle");
            if (request == null)
            {
                _logger.LogWarning("Null UseCase");
                return null;
            }

            _logger.LogDebug("Get Hubs");
            var hubs = await _mediatorService.QueryAsync(new HubsListQuery());
            var hubModel = hubs.FirstOrDefault();

            _logger.LogDebug("Get Active Nodes");
            var nodesModel = await _mediatorService.QueryAsync(new ActiveNodeListWithMinimalInfoQuery());

            var dashboard =
                await _mediatorService.QueryAsync(new DashboardListByHubQuery(-1, false,
                    _httpContextAccessor?.HttpContext?.User, true));

            foreach (var item in nodesModel)
            {
                var dashboards = await _mediatorService.QueryAsync(
                    new DashboardsWithMininamlInfoByNodeIdQuery(item.NodeId, _httpContextAccessor?.HttpContext?.User,
                        true));
                item.Dashboards = dashboards?.Select(i => new NodeMinimalInfoDto.NodeDashboard
                    {Id = i.DashboardId, Titles = i.Title}).ToList();
            }

            _logger.LogDebug("End Handle");
            return new GetHubAndMinimalInfoResponse
            {
                Hub = hubModel, Nodes = nodesModel,
                HubDashboards = dashboard?.Select(i => new GetHubAndMinimalInfoResponse.Dashboard
                    {Id = i.DashboardId, Titles = i.Title}).ToList()
            };
        }
    }
}