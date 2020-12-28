using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DataBrowser.AuthenticationAuthorization;
using DataBrowser.Domain.Entities.DBoard;
using DataBrowser.Domain.Entities.Hubs;
using DataBrowser.Domain.Entities.Nodes;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Authentication;
using DataBrowser.Interfaces.Mediator;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataBrowser.Command.Dashboards
{
    public class AssignDashboardToHubCommand : DashboardCommandBase, ICommand<bool>
    {
        public AssignDashboardToHubCommand(int dashboardId, int hubId, ClaimsPrincipal specificUser = null,
            bool checkAssignPermission = false)
            : base(specificUser)
        {
            DashboardId = dashboardId;
            HubId = hubId;
            CheckAssignPermission = checkAssignPermission;
        }

        public int DashboardId { get; }
        public int HubId { get; }
        public bool CheckAssignPermission { get; }

        public class AssignDashboardToHubHandler : IRequestHandler<AssignDashboardToHubCommand, bool>
        {
            private readonly IRepository<Dashboard> _dashboardRepository;
            private readonly IFilterDashboard _filterDashboard;
            private readonly IRepository<Hub> _hubRepository;
            private readonly ILogger<AssignDashboardToHubHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IRepository<Node> _nodeRepository;
            private readonly IRequestContext _requestContext;

            public AssignDashboardToHubHandler(ILogger<AssignDashboardToHubHandler> logger,
                IRepository<Hub> hubRepository,
                IRepository<Node> nodeRepository,
                IRepository<Dashboard> dashboardRepository,
                IMapper mapper,
                IFilterDashboard filterDashboard,
                IRequestContext requestContext
            )
            {
                _logger = logger;
                _mapper = mapper;
                _hubRepository = hubRepository;
                _dashboardRepository = dashboardRepository;
                _nodeRepository = nodeRepository;
                _filterDashboard = filterDashboard;
                _requestContext = requestContext;
            }

            public async Task<bool> Handle(AssignDashboardToHubCommand request, CancellationToken cancellationToken)
            {
                _logger.LogDebug("START");

                if (request.SpecificUser == null) request.SpecificUser = _requestContext.LoggedUser;

                var dashboardEntity = await _dashboardRepository.GetByIdAsync(request.DashboardId);
                if (dashboardEntity == null)
                {
                    _logger.LogDebug($"Dashboard with id {request?.DashboardId} does not exists");
                    return false; //dashboard does not exists
                }

                if (request.CheckAssignPermission && !UtilitySecurity.IsAdminUser(request.SpecificUser))
                {
                    _logger.LogDebug("User does not have rights to assign dashboard to hub");
                    throw new UnauthorizedAccessException("User does not have rights to assign dashboard to hub");
                }

                var hubList = await _hubRepository.ListAllAsync();
                var hubEntity = hubList.First(x => request.HubId < 0 || x.HubId == request.HubId);
                if (hubEntity == null)
                {
                    _logger.LogDebug($"Hub with id {request?.HubId} does not exists");
                    return false; //hub does not exists
                }


                _logger.LogDebug("edit to repository");
                dashboardEntity.SetHubAssociation(hubEntity.HubId);
                _dashboardRepository.Update(dashboardEntity);

                _logger.LogDebug("SaveChangeAsync");
                await _dashboardRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

                return true;
            }
        }
    }
}