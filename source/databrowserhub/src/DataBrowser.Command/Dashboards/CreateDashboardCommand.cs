using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DataBrowser.AC.Utility;
using DataBrowser.AC.Utility.Helpers;
using DataBrowser.AuthenticationAuthorization;
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
    public class CreateDashboardCommand : DashboardCommandBase, ICommand<int>
    {
        public DashboardDto Dashboard;

        public CreateDashboardCommand(DashboardDto dashboard,
            ClaimsPrincipal specificUser = null,
            bool filterByPermissionViewTemplate = false,
            bool checkWritePermission = false,
            bool checkReadPermission = false)
            : base(specificUser, filterByPermissionViewTemplate, checkWritePermission, checkReadPermission)
        {
            Dashboard = dashboard;
        }

        public class CreateDashboardHandler : IRequestHandler<CreateDashboardCommand, int>
        {
            private readonly IRepository<Dashboard> _dashboardRepository;
            private readonly IFilterDashboard _filterDashboard;
            private readonly IFilterView _filterView;
            private readonly ILogger<CreateDashboardHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IMediator _mediator;
            private readonly IRepository<ViewTemplate> _repositoryViewTemplate;
            private readonly IRequestContext _requestContext;
            private readonly IEnumerable<IRuleSpecification<DashboardDto>> _rules;

            public CreateDashboardHandler(ILogger<CreateDashboardHandler> logger,
                IRepository<Dashboard> dashboardRepository,
                IRepository<ViewTemplate> repositoryViewTemplate,
                IMapper mapper,
                IMediator mediator,
                IFilterDashboard filterDashboard,
                IFilterView filterView,
                IRequestContext requestContext, 
                IEnumerable<IRuleSpecification<DashboardDto>> rules)
            {
                _logger = logger;
                _dashboardRepository = dashboardRepository;
                _repositoryViewTemplate = repositoryViewTemplate;
                _mapper = mapper;
                _filterDashboard = filterDashboard;
                _mediator = mediator;
                _requestContext = requestContext;
                _rules = rules;
                _filterView = filterView;
            }

            public async Task<int> Handle(CreateDashboardCommand request, CancellationToken cancellationToken)
            {
                _logger.LogDebug("START");

                if (request.SpecificUser == null) request.SpecificUser = _requestContext.LoggedUser;

                request.Dashboard.DashboardId = 0;
                request.Dashboard.UserId = UtilitySecurity.GetUserId(request.SpecificUser);
                if (!_filterDashboard.CheckWritePermission(request.Dashboard, request.SpecificUser))
                {
                    _logger.LogDebug($"User does not have rights to create dashboard {request.Dashboard.DashboardId}");
                    throw new UnauthorizedAccessException(
                        $"User does not have rights to create dashboard {request.Dashboard.DashboardId}");
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

                request.Dashboard.ViewIds = viewsId;
                var validator = await Domain.Entities.DBoard.Dashboard.CreateDashboardAsync(request.Dashboard, _rules);

                if (!validator.IsValid)
                {
                    return -1;
                }

                _dashboardRepository.Add(validator.ValidateObject);
                _logger.LogDebug("SaveChangeAsync");
                await _dashboardRepository.UnitOfWork.SaveChangesAsync();

                _logger.LogDebug("END");
                return validator.ValidateObject.DashboardId;
            }
        }
    }
}