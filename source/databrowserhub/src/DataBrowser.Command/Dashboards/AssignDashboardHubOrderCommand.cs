using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DataBrowser.AC.Utility;
using DataBrowser.Domain.Entities.DBoard;
using DataBrowser.Domain.Entities.Hubs;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Interfaces.Authentication;
using DataBrowser.Interfaces.Mediator;
using DataBrowser.Specifications.Dashboards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataBrowser.Command.Dashboards
{
    public class AssignDashboardHubOrderCommand : DashboardCommandBase, ICommand<bool>
    {
        public int HubId;

        public List<int> SortedDashboardIds;

        public AssignDashboardHubOrderCommand(int hubId, List<int> sortedDashboardIds, ClaimsPrincipal specificUser,
            bool filterByPermissionViewTemplate = false)
            : base(specificUser, filterByPermissionViewTemplate)
        {
            HubId = hubId;
            SortedDashboardIds = sortedDashboardIds;
        }

        public class AssignDashboardHubOrderHandler : IRequestHandler<AssignDashboardHubOrderCommand, bool>
        {
            private readonly IRepository<Dashboard> _dashboardRepository;
            private readonly IFilterDashboard _filterDashboard;
            private readonly IRepository<Hub> _hubRepository;
            private readonly ILogger<AssignDashboardHubOrderHandler> _logger;
            private readonly IMapper _mapper;

            public AssignDashboardHubOrderHandler(ILogger<AssignDashboardHubOrderHandler> logger,
                IRepository<Hub> hubRepository,
                IRepository<Dashboard> dashboardRepository,
                IMapper mapper,
                IFilterDashboard filterDashboard
            )
            {
                _logger = logger;
                _mapper = mapper;
                _hubRepository = hubRepository;
                _dashboardRepository = dashboardRepository;
                _filterDashboard = filterDashboard;
            }

            public async Task<bool> Handle(AssignDashboardHubOrderCommand request, CancellationToken cancellationToken)
            {
                _logger.LogDebug("START AssignDashboardHubOrderCommand");

                var hubList = await _hubRepository.ListAllAsync();
                var hubEntity = hubList.First(x => request.HubId < 0 || x.HubId == request.HubId);
                if (hubEntity == null)
                {
                    _logger.LogDebug($"AssignDashboardHubOrderCommand Hub with id {request?.HubId} does not exists");
                    return false; //hub does not exists
                }

                var dashboardEntities =
                    await _dashboardRepository.FindAsync(new DashboardByHubIdSpecification(hubEntity.HubId));

                foreach (var dashboardEntity in dashboardEntities)
                {
                    var havePermission =
                        _filterDashboard.CheckWritePermission(dashboardEntity.ConvertToDashboardDto(_mapper),
                            request.SpecificUser);
                    if (havePermission)
                    {
                        _logger.LogDebug("edit to repository");
                        dashboardEntity.Weight =
                            request.SortedDashboardIds.FindIndex(v => v == dashboardEntity.DashboardId);
                        _dashboardRepository.Update(dashboardEntity);
                    }
                    else
                    {
                        _logger.LogDebug(
                            $"AssignDashboardHubOrderCommand: User does not have rights to edit dashboard with id {dashboardEntity.DashboardId}.");
                    }
                }

                _logger.LogDebug("SaveChangeAsync");
                await _dashboardRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogDebug("END AssignDashboardHubOrderCommand");
                return true;
            }
        }
    }
}