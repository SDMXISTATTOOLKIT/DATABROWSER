using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DataBrowser.AC.Utility;
using DataBrowser.AC.Utility.Helpers;
using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.DBoard;
using DataBrowser.Domain.Entities.ViewTemplates;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Domain.Specifications.Rules;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Authentication;
using DataBrowser.Interfaces.Mediator;
using DataBrowser.Specifications.ViewTemplates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataBrowser.Command.Dashboards
{
    public class EditDashboardCommand : DashboardCommandBase, ICommand<bool>
    {
        public DashboardDto Dashboard;

        public EditDashboardCommand(DashboardDto dashboard,
            ClaimsPrincipal specificUser = null,
            bool filterByPermissionViewTemplate = false,
            bool checkWritePermission = false,
            bool checkReadPermission = false)
            : base(specificUser, filterByPermissionViewTemplate, checkWritePermission, checkReadPermission)
        {
            Dashboard = dashboard;
        }

        public class EditDashboardHandler : IRequestHandler<EditDashboardCommand, bool>
        {
            private readonly IRepository<Dashboard> _dashboardRepository;
            private readonly IFilterDashboard _filterDashboard;
            private readonly IFilterView _filterView;
            private readonly ILogger<EditDashboardHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IRepository<ViewTemplate> _repositoryViewTemplate;
            private readonly IRequestContext _requestContext;
            private readonly IEnumerable<IRuleSpecification<DashboardDto>> _rules;

            public EditDashboardHandler(ILogger<EditDashboardHandler> logger,
                IRepository<Dashboard> dashboardRepository,
                IRepository<ViewTemplate> repositoryViewTemplate,
                IMapper mapper,
                IFilterDashboard filterDashboard,
                IFilterView filterView,
                IRequestContext requestContext,
                IEnumerable<IRuleSpecification<DashboardDto>> rules
            )
            {
                _logger = logger;
                _dashboardRepository = dashboardRepository;
                _repositoryViewTemplate = repositoryViewTemplate;
                _mapper = mapper;
                _filterDashboard = filterDashboard;
                _requestContext = requestContext;
                _rules = rules;
                _filterView = filterView;
            }

            public async Task<bool> Handle(EditDashboardCommand request, CancellationToken cancellationToken)
            {
                _logger.LogDebug("START");

                if (request.SpecificUser == null) request.SpecificUser = _requestContext.LoggedUser;

                var dashboardEntity = await _dashboardRepository.GetByIdAsync(request.Dashboard.DashboardId);
                if (dashboardEntity == null)
                {
                    _logger.LogDebug($"No dashboard with id {request?.Dashboard?.DashboardId} was found");
                    return false; //Not found in this case
                }

                if (!_filterDashboard.CheckWritePermission(dashboardEntity.ConvertToDashboardDto(_mapper),
                    request.SpecificUser))
                {
                    _logger.LogDebug($"User does not have rights to edit dashboard {request.Dashboard.DashboardId}");
                    throw new UnauthorizedAccessException(
                        $"User does not have rights to edit dashboard {request.Dashboard.DashboardId}");
                }

                var viewTemplateEntities =
                    await _repositoryViewTemplate.FindAsync(new ViewListByIdsSpecification(request.Dashboard.ViewIds));
                var viewsId = new List<int>();
                foreach (var viewTemplateEntity in viewTemplateEntities)
                {
                    var dto = viewTemplateEntity.ConvertToViewTemplateDto(_mapper);
                    var hasViewTemplatePermission = ViewTemplateHelper.HaveViewPermission(true,
                        viewTemplateEntity.NodeFK, request.SpecificUser, dto, _filterView, _logger);

                    if (hasViewTemplatePermission)
                        viewsId.Add(viewTemplateEntity.ViewTemplateId);
                    else
                        _logger.LogDebug(
                            $"User does not have rights to read view {viewTemplateEntity.ViewTemplateId}.");
                }

                _logger.LogDebug("edit to repository");
                request.Dashboard.ViewIds = viewsId;
                await dashboardEntity.EditAsync(request.Dashboard, _rules);
                dashboardEntity.SetView(request.Dashboard.ViewIds);
                _dashboardRepository.Update(dashboardEntity);

                _logger.LogDebug("SaveChangeAsync");
                await _dashboardRepository.UnitOfWork.SaveChangesAsync();

                _logger.LogDebug("END");
                return true;
            }
        }
    }
}