using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DataBrowser.AC.Utility;
using DataBrowser.Domain.Entities.DBoard;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Interfaces.Authentication;
using DataBrowser.Interfaces.Mediator;
using DataBrowser.Specifications.Dashboards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataBrowser.Command.Dashboards
{
    public class AssignDashboardNodeOrderCommand : DashboardCommandBase, ICommand<bool>
    {
        public int NodeId;

        public List<int> SortedDashboardIds;

        public AssignDashboardNodeOrderCommand(int nodeId, List<int> sortedDashboardIds, ClaimsPrincipal specificUser,
            bool filterByPermissionViewTemplate = false)
            : base(specificUser, filterByPermissionViewTemplate)
        {
            NodeId = nodeId;
            SortedDashboardIds = sortedDashboardIds;
        }

        public class AssignDashboardNodeOrderHandler : IRequestHandler<AssignDashboardNodeOrderCommand, bool>
        {
            private readonly IRepository<Dashboard> _dashboardRepository;
            private readonly IFilterDashboard _filterDashboard;
            private readonly ILogger<AssignDashboardNodeOrderHandler> _logger;
            private readonly IMapper _mapper;

            public AssignDashboardNodeOrderHandler(ILogger<AssignDashboardNodeOrderHandler> logger,
                IRepository<Dashboard> dashboardRepository,
                IMapper mapper,
                IFilterDashboard filterDashboard)
            {
                _logger = logger;
                _mapper = mapper;
                _dashboardRepository = dashboardRepository;
                _filterDashboard = filterDashboard;
            }

            public async Task<bool> Handle(AssignDashboardNodeOrderCommand request, CancellationToken cancellationToken)
            {
                _logger.LogDebug("START");

                var dashboardEntities =
                    await _dashboardRepository.FindAsync(new DashboardByNodeIdSpecification(request.NodeId, false));
                if (dashboardEntities == null)
                {
                    _logger.LogDebug($" Node with id {request?.NodeId} does not exists");
                    return false;
                }

                foreach (var dashboardEntity in dashboardEntities)
                {
                    var havePermission =
                        _filterDashboard.CheckWritePermission(dashboardEntity.ConvertToDashboardDto(_mapper),
                            request.SpecificUser);
                    if (havePermission)
                    {
                        _logger.LogDebug("edit to repository");
                        var newWeight = request.SortedDashboardIds.FindIndex(x => x == dashboardEntity.DashboardId);
                        var matchingEntity = dashboardEntity.Nodes.FirstOrDefault(x => x.NodeId == request.NodeId);

                        if (matchingEntity != null)
                        {
                            matchingEntity.Weight = newWeight >= 0 ? newWeight : short.MaxValue;
                            _dashboardRepository.Update(dashboardEntity);
                        }
                    }
                    else
                    {
                        _logger.LogDebug(
                            $"User does not have rights to edit dashboard with id {dashboardEntity.DashboardId}.");
                    }
                }

                await _dashboardRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogDebug("END");
                return true;
            }
        }
    }
}