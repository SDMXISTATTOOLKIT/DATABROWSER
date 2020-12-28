using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DataBrowser.AC.Utility;
using DataBrowser.Domain.Entities.DBoard;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Authentication;
using DataBrowser.Interfaces.Mediator;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataBrowser.Command.Dashboards
{
    public class UnassignDashboardNodeCommand : DashboardCommandBase, ICommand<bool>
    {
        public int DashboardId;

        public int NodeId;

        public UnassignDashboardNodeCommand(int dashboardId, int nodeId, ClaimsPrincipal specificUser = null,
            bool filterByPermissionViewTemplate = false)
            : base(specificUser, filterByPermissionViewTemplate)
        {
            DashboardId = dashboardId;
            NodeId = nodeId;
        }

        public class UnAssignDashboardNodeHandler : IRequestHandler<UnassignDashboardNodeCommand, bool>
        {
            private readonly IRepository<Dashboard> _dashboardRepository;
            private readonly IFilterDashboard _filterDashboard;
            private readonly ILogger<UnAssignDashboardNodeHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IRequestContext _requestContext;

            public UnAssignDashboardNodeHandler(ILogger<UnAssignDashboardNodeHandler> logger,
                IRepository<Dashboard> dashboardRepository,
                IMapper mapper,
                IFilterDashboard filterDashboard,
                IRequestContext requestContext
            )
            {
                _logger = logger;
                _dashboardRepository = dashboardRepository;
                _mapper = mapper;
                _filterDashboard = filterDashboard;
                _requestContext = requestContext;
            }


            public async Task<bool> Handle(UnassignDashboardNodeCommand request, CancellationToken cancellationToken)
            {
                _logger.LogDebug("START");

                if (request.SpecificUser == null) request.SpecificUser = _requestContext.LoggedUser;

                var dashboardEntity = await _dashboardRepository.GetByIdAsync(request.DashboardId);
                if (dashboardEntity == null)
                {
                    _logger.LogDebug($"No dashboard with id {request?.DashboardId} was found");
                    return false; //Not found in this case
                }

                var havePermission =
                    _filterDashboard.CheckWritePermission(dashboardEntity.ConvertToDashboardDto(_mapper),
                        request.SpecificUser);
                if (!havePermission)
                {
                    _logger.LogDebug("User does not have rights to edit dashboards.");
                    return false; //Not found in this case
                }

                dashboardEntity.UnAssignNode(request.NodeId);
                _dashboardRepository.Update(dashboardEntity);

                await _dashboardRepository.UnitOfWork.SaveChangesAsync();

                _logger.LogDebug("END");
                return true;
            }
        }
    }
}