using AutoMapper;
using DataBrowser.AC.Utility;
using DataBrowser.AC.Utility.Helpers;
using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.DBoard;
using DataBrowser.Domain.Entities.ViewTemplates;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Authentication;
using DataBrowser.Interfaces.Mediator;
using DataBrowser.Specifications.ViewTemplates;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace DataBrowser.Command.Dashboards
{
    public class AssignDashboardViewCommand : DashboardCommandBase, ICommand<bool>
    {
        public DashboardDto Dashboard;

        public AssignDashboardViewCommand(DashboardDto dashboard,
            ClaimsPrincipal specificUser = null,
            bool filterByPermissionViewTemplate = false,
            bool checkWritePermission = false,
            bool checkReadPermission = false)
            : base(specificUser, filterByPermissionViewTemplate, checkWritePermission, checkReadPermission)
        {
            Dashboard = dashboard;
        }

        public class AssignDashboardToViewHandler : IRequestHandler<AssignDashboardViewCommand, bool>
        {
            private readonly IFilterView _filterView;
            private readonly ILogger<AssignDashboardToViewHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IRepository<Dashboard> _repositoryDashboard;
            private readonly IRepository<ViewTemplate> _repositoryViewTemplate;
            private readonly IRequestContext _requestContext;

            public AssignDashboardToViewHandler(ILogger<AssignDashboardToViewHandler> logger,
                IRepository<ViewTemplate> repositoryViewTemplate,
                IRepository<Dashboard> repositoryDashboard,
                IMapper mapper,
                IFilterDashboard filterDashboard,
                IFilterView filterView,
                IRequestContext requestContext
            )
            {
                _logger = logger;
                _mapper = mapper;
                _repositoryViewTemplate = repositoryViewTemplate;
                _filterView = filterView;
                _requestContext = requestContext;
                _repositoryDashboard = repositoryDashboard;
            }

            public async Task<bool> Handle(AssignDashboardViewCommand request, CancellationToken cancellationToken)
            {
                _logger.LogDebug("START");

                if (request.SpecificUser == null) request.SpecificUser = _requestContext.LoggedUser;

                var dashboard = await _repositoryDashboard.GetByIdAsync(request.Dashboard.DashboardId);

                if (dashboard == null)
                {
                    _logger.LogDebug($"Dashboard {request.Dashboard.DashboardId} not found");
                    return false;
                }

                if (request?.Dashboard?.ViewIds?.Count == null || request?.Dashboard?.ViewIds?.Count == 0)
                {
                    _logger.LogDebug("List of view ids is empty. Returning true");
                    return true;
                }

                var viewTemplateEntities =
                    await _repositoryViewTemplate.FindAsync(new ViewListByIdsSpecification(request.Dashboard.ViewIds));

                foreach (var viewTemplateEntity in viewTemplateEntities)
                {
                    var dto = viewTemplateEntity.ConvertToViewTemplateDto(_mapper);
                    var hasViewTemplatePermission = ViewTemplateHelper.HaveViewPermission(true,
                        viewTemplateEntity.NodeFK, request.SpecificUser, dto, _filterView, _logger);

                    if (!hasViewTemplatePermission) return false;

                    dashboard.AssignView(viewTemplateEntity.ViewTemplateId);
                }

                _logger.LogDebug("SaveChangeAsync");
                await _repositoryDashboard.UnitOfWork.SaveChangesAsync();

                return true;
            }
        }
    }
}