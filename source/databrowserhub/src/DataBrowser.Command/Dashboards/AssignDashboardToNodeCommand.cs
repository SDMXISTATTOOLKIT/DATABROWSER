using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using DataBrowser.Domain.Entities.DBoard;
using DataBrowser.Domain.Entities.Nodes;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Authentication;
using DataBrowser.Interfaces.Mediator;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataBrowser.Command.Dashboards
{
    public class AssignDashboardToNodeCommand : DashboardCommandBase, ICommand<bool>
    {
        public AssignDashboardToNodeCommand(int dashboardId, int nodeId, ClaimsPrincipal specificUser = null,
            bool checkAssignPermission = false)
            : base(specificUser)
        {
            DashboardId = dashboardId;
            NodeId = nodeId;
            CheckAssignPermission = checkAssignPermission;
        }

        public int DashboardId { get; }
        public int NodeId { get; }
        public bool CheckAssignPermission { get; }

        public class AssignDashboardToNodeHandler : IRequestHandler<AssignDashboardToNodeCommand, bool>
        {
            private readonly IRepository<Dashboard> _dashboardRepository;
            private readonly IFilterNode _filterNode;
            private readonly ILogger<AssignDashboardToNodeHandler> _logger;
            private readonly IRepository<Node> _nodeRepository;
            private readonly IRequestContext _requestContext;

            public AssignDashboardToNodeHandler(ILogger<AssignDashboardToNodeHandler> logger,
                IRepository<Node> nodeRepository,
                IRepository<Dashboard> dashboardRepository,
                IFilterNode filterNode,
                IRequestContext requestContext
            )
            {
                _logger = logger;
                _dashboardRepository = dashboardRepository;
                _nodeRepository = nodeRepository;
                _requestContext = requestContext;
                _filterNode = filterNode;
            }

            public async Task<bool> Handle(AssignDashboardToNodeCommand request, CancellationToken cancellationToken)
            {
                _logger.LogDebug("START");

                if (request.SpecificUser == null) request.SpecificUser = _requestContext.LoggedUser;

                if (request.CheckAssignPermission &&
                    !_filterNode.CheckPermissionNodeManageConfig(request.NodeId, request.SpecificUser))
                {
                    _logger.LogDebug("User does not have rights to assign dashboard to node");
                    throw new UnauthorizedAccessException("User does not have rights to assign dashboard to node");
                }

                var nodeEntity = await _nodeRepository.GetByIdAsync(request.NodeId);
                if (nodeEntity == null)
                {
                    _logger.LogDebug($"Node with id {request?.NodeId} does not exists");
                    return false;
                }

                var dashboardEntity = await _dashboardRepository.GetByIdAsync(request.DashboardId);
                if (dashboardEntity == null)
                {
                    _logger.LogDebug($"Dashboard with id {request?.DashboardId} does not exists");
                    return false;
                }

                dashboardEntity.AssignNode(request.NodeId);
                _logger.LogDebug("SaveChangeAsync");
                await _dashboardRepository.UnitOfWork.SaveChangesAsync();

                return true;
            }
        }
    }
}