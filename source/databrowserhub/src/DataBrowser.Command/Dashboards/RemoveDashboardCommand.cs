using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DataBrowser.AC.Exceptions;
using DataBrowser.AC.Responses.Services;
using DataBrowser.AC.Utility;
using DataBrowser.Command.Dashboards.Model;
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
    public class RemoveDashboardCommand : DashboardCommandBase, ICommand<RemoveDashboardResult>
    {
        public int DashboardId;

        public RemoveDashboardCommand(int dashboardId, ClaimsPrincipal specificUser = null,
            bool filterByPermissionViewTemplate = false)
            : base(specificUser, filterByPermissionViewTemplate)
        {
            DashboardId = dashboardId;
        }

        private class RemoveDashboardHandler : IRequestHandler<RemoveDashboardCommand, RemoveDashboardResult>
        {
            private readonly IRepository<Dashboard> _dashboardRepository;
            private readonly IFilterDashboard _filterDashboard;
            private readonly ILogger<RemoveDashboardHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IMediator _mediator;
            private readonly IRepository<Node> _repositoryNode;
            private readonly IRequestContext _requestContext;

            public RemoveDashboardHandler(ILogger<RemoveDashboardHandler> logger,
                IRepository<Dashboard> dashboardRepository,
                IRepository<Node> repositoryNode,
                IMapper mapper,
                IMediator mediator,
                IFilterDashboard filterDashboard,
                IRequestContext requestContext
            )
            {
                _logger = logger;
                _dashboardRepository = dashboardRepository;
                _repositoryNode = repositoryNode;
                _mapper = mapper;
                _filterDashboard = filterDashboard;
                _mediator = mediator;
                _requestContext = requestContext;
            }

            public async Task<RemoveDashboardResult> Handle(RemoveDashboardCommand request,
                CancellationToken cancellationToken)
            {
                _logger.LogDebug("START");

                if (request.SpecificUser == null) request.SpecificUser = _requestContext.LoggedUser;

                var dashboardEntity = await _dashboardRepository.GetByIdAsync(request.DashboardId);
                if (dashboardEntity == null)
                {
                    _logger.LogDebug($"No dashboard with id {request?.DashboardId} was found");
                    return new RemoveDashboardResult {NotFound = true};
                }

                var havePermission =
                    _filterDashboard.CheckWritePermission(dashboardEntity.ConvertToDashboardDto(_mapper),
                        request.SpecificUser);
                if (!havePermission)
                {
                    _logger.LogDebug($"Haven't permission for current dashboard {request.DashboardId}");
                    throw new InsufficentPermissionException(
                        $"Haven't permission for current dashboard {request.DashboardId}");
                }

                var result = new RemoveDashboardResult();

                result.AssignToHub = dashboardEntity.HubFk.HasValue && dashboardEntity.HubFk.Value > 0;

                if (dashboardEntity.Nodes != null)
                {
                    result.Nodes = new List<NodeMinimalInfoDto>();
                    foreach (var itemNode in dashboardEntity.Nodes)
                    {
                        var node = await _repositoryNode.GetByIdAsync(itemNode.NodeId);
                        result.Nodes.Add(node.ConvertToNodeDataView(_mapper));
                    }
                }

                if (result.Nodes != null && result.Nodes.Count > 0 ||
                    result.AssignToHub)
                    return result;

                _logger.LogDebug("remove from repository");
                _dashboardRepository.Delete(dashboardEntity);

                _logger.LogDebug("SaveChangeAsync");
                await _dashboardRepository.UnitOfWork.SaveChangesAsync();

                result.Deleted = true;

                _logger.LogDebug("END");
                return result;
            }
        }
    }
}