using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DataBrowser.AC.Utility;
using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.DBoard;
using DataBrowser.Domain.Entities.ViewTemplates;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Interfaces.Authentication;
using DataBrowser.Interfaces.Mediator;
using DataBrowser.Interfaces.ModelViews;
using DataBrowser.Query.Dashboards.ModelView;
using DataBrowser.Specifications.ViewTemplates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataBrowser.Query.Dashboards
{
    public class DashboardByIdQuery : DashboardQueryBase, IQuery<DashboardViewModel>
    {
        public DashboardByIdQuery(int dashboardId,
            bool includeViewsData,
            ClaimsPrincipal filterBySpecificUser = null,
            bool filterByPermission = false) :
            base(filterByPermission, filterBySpecificUser)
        {
            DashboardId = dashboardId;
            IncludeViewsData = includeViewsData;
        }

        public int DashboardId { get; }
        public bool IncludeViewsData { get; }

        public class DashboardByIdHandler : IRequestHandler<DashboardByIdQuery, DashboardViewModel>
        {
            private readonly IFilterDashboard _filterDashboard;
            private readonly ILogger<DashboardByIdHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IRepository<Dashboard> _repository;
            private readonly IUserService _userService;
            private readonly IRepository<ViewTemplate> _viewTemplateRepository;

            public DashboardByIdHandler(ILogger<DashboardByIdHandler> logger,
                IRepository<Dashboard> repository,
                IMapper mapper,
                IFilterDashboard filterDashboard,
                IRepository<ViewTemplate> viewTemplateRepository,
                IUserService userService)
            {
                _logger = logger;
                _repository = repository;
                _mapper = mapper;
                _filterDashboard = filterDashboard;
                _viewTemplateRepository = viewTemplateRepository;
                _userService = userService;
            }

            public async Task<DashboardViewModel> Handle(DashboardByIdQuery request,
                CancellationToken cancellationToken)
            {
                _logger.LogDebug("START");

                var dashboardEntity = await _repository.GetByIdAsync(request.DashboardId);

                if (dashboardEntity == null)
                {
                    //error handling
                }

                if (_filterDashboard != null &&
                    request.FilterByPermission)
                {
                    var havePermission =
                        _filterDashboard.CheckReadPermission(dashboardEntity.ConvertToDashboardDto(_mapper),
                            request.SpecificUser);
                    if (!havePermission) return null; //Not found in this case
                }

                var dashboardViews = new List<ViewTemplateDto>();
                if (request.IncludeViewsData)
                {
                    var viewIds = dashboardEntity?.Views.Select(i => i.ViewTemplateId).ToList();
                    if (viewIds != null && viewIds.Count > 0)
                    {
                        var entities =
                            await _viewTemplateRepository.FindAsync(new ViewTemplateByMultiIdsSpecification(viewIds));
                        dashboardViews.AddRange(entities.Select(i => i.ConvertToViewTemplateDto(_mapper)));
                    }
                }


                var dashModelView = new DashboardViewModel
                {
                    DashboardId = dashboardEntity.DashboardId,
                    DashboardConfig = dashboardEntity.DashboardConfig,
                    Title = dashboardEntity.Title.TransatableItemValues.ToDictionary(val => val.Language,
                        val => val.Value),
                    HubId = dashboardEntity.HubFk,
                    Weight = dashboardEntity.Weight,
                    UserId = dashboardEntity.UserFk,
                    Username = await _userService.GetUsername(dashboardEntity.UserFk),
                    NodeIds = dashboardEntity?.Nodes?.Select(i => i.NodeId).ToList(),
                    ViewIds = dashboardEntity?.Views?.Select(i => i.ViewTemplateId)?.ToList(),
                    Views = dashboardViews?.ToDictionary(x => x.ViewTemplateId,
                        y => ViewTemplateViewModel.ConvertFromDto(y)),
                    FilterLevels = dashboardEntity.FilterLevels
                };

                return dashModelView;
            }
        }
    }
}