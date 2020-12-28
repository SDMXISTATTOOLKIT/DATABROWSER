using System.Collections.Generic;
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
    public class UnAssignAllDashboardViewCommand : DashboardCommandBase, ICommand<bool>
    {
        public int DashboardId;

        public UnAssignAllDashboardViewCommand(int dashboardId, ClaimsPrincipal specificUser,
            bool filterByPermissionViewTemplate = false)
            : base(specificUser, filterByPermissionViewTemplate)
        {
            DashboardId = dashboardId;
        }

        public class UnAssignAllDashboardViewHandler : IRequestHandler<UnAssignAllDashboardViewCommand, bool>
        {
            private readonly IRepository<Dashboard> _dashboardRepository;
            private readonly IFilterDashboard _filterDashboard;
            private readonly ILogger<UnAssignAllDashboardViewHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IRequestContext _requestContext;

            public UnAssignAllDashboardViewHandler(ILogger<UnAssignAllDashboardViewHandler> logger,
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


            public async Task<bool> Handle(UnAssignAllDashboardViewCommand request, CancellationToken cancellationToken)
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
                    _logger.LogDebug("User does not have rights to delete dashboards.");
                    return false; //Not found in this case
                }

                dashboardEntity.SetView(new List<int>());


                _logger.LogDebug("SaveChangeAsync");
                await _dashboardRepository.UnitOfWork.SaveChangesAsync();

                _logger.LogDebug("END");
                return true;
            }
        }
    }
}