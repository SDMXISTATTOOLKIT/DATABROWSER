using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DataBrowser.AC.Utility;
using DataBrowser.AuthenticationAuthorization;
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
    public class AllDashboardCrossNodeQuery : DashboardQueryBase, IQuery<List<DashboardViewModel>>
    {
        public AllDashboardCrossNodeQuery(int hubId,
            bool includeViewsData,
            ClaimsPrincipal filterBySpecificUser,
            bool filterByPermission = false) :
            base(filterByPermission, filterBySpecificUser)
        {
            HubId = hubId;
            IncludeViewsData = includeViewsData;
        }

        public int HubId { get; }
        public bool IncludeViewsData { get; }

        public class DashboardListByHubHandler : IRequestHandler<AllDashboardCrossNodeQuery, List<DashboardViewModel>>
        {
            private readonly IRepository<Dashboard> _dashboardRepository;
            private readonly IFilterDashboard _filterDashboard;
            private readonly ILogger<DashboardListByHubHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IUserService _userService;
            private readonly IRepository<ViewTemplate> _viewTemplateRepository;

            public DashboardListByHubHandler(ILogger<DashboardListByHubHandler> logger,
                IRepository<Dashboard> repository,
                IRepository<ViewTemplate> viewTemplateRepository,
                IMapper mapper,
                IFilterDashboard filterDashboard,
                IUserService userService)
            {
                _logger = logger;
                _dashboardRepository = repository;
                _viewTemplateRepository = viewTemplateRepository;
                _mapper = mapper;
                _filterDashboard = filterDashboard;
                _userService = userService;
            }

            public async Task<List<DashboardViewModel>> Handle(AllDashboardCrossNodeQuery request,
                CancellationToken cancellationToken)
            {
                _logger.LogDebug("START");

                var userId = UtilitySecurity.GetUserId(request.SpecificUser);

                var dashboardList = await _dashboardRepository.ListAllAsync();
                if (dashboardList == null)
                    //error handling
                    return null;

                var dashboardViews = new List<ViewTemplateDto>();
                if (request.IncludeViewsData)
                {
                    var viewIds = dashboardList.SelectMany(i => i.Views.Select(i => i.ViewTemplateId)).ToList();
                    if (viewIds != null && viewIds.Count > 0)
                    {
                        var entities =
                            await _viewTemplateRepository.FindAsync(new ViewTemplateByMultiIdsSpecification(viewIds));
                        dashboardViews.AddRange(entities.Select(i => i.ConvertToViewTemplateDto(_mapper)));
                    }
                }

                var resultList = new List<DashboardViewModel>();
                foreach (var dashboardEntity in dashboardList)
                {
                    var dashModelView = new DashboardViewModel
                    {
                        DashboardId = dashboardEntity.DashboardId,
                        DashboardConfig = dashboardEntity.DashboardConfig,
                        Title = dashboardEntity.Title.TransatableItemValues.ToDictionary(val => val.Language,
                            val => val.Value),
                        HubId = dashboardEntity.HubFk,
                        UserId = dashboardEntity.UserFk,
                        Username = await _userService.GetUsername(dashboardEntity.UserFk),
                        Weight = dashboardEntity.Weight,
                        Views = dashboardViews
                            ?.Where(i => dashboardEntity.Views.Any(k => k.ViewTemplateId == i.ViewTemplateId))
                            ?.ToDictionary(x => x.ViewTemplateId, y => ViewTemplateViewModel.ConvertFromDto(y)),
                        ViewIds = dashboardEntity?.Views.Select(i => i.ViewTemplateId).ToList(),
                        FilterLevels = dashboardEntity.FilterLevels
                    };

                    resultList.Add(dashModelView);
                }

                return resultList;
            }
        }
    }
}