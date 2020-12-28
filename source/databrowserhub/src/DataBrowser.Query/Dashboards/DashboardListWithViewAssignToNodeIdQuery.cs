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
using DataBrowser.Specifications.Dashboards;
using DataBrowser.Specifications.ViewTemplates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataBrowser.Query.Dashboards
{
    public class DashboardListWithViewAssignToNodeIdQuery : DashboardQueryBase, IQuery<List<DashboardViewModel>>
    {
        public DashboardListWithViewAssignToNodeIdQuery(int nodeId,
            ClaimsPrincipal filterBySpecificUser,
            bool filterByPermission = false) :
            base(filterByPermission, filterBySpecificUser)
        {
            NodeId = nodeId;
        }

        public int NodeId { get; }

        public class
            DashboardListWithViewAssignToNodeIdHandler : IRequestHandler<DashboardListWithViewAssignToNodeIdQuery,
                List<DashboardViewModel>>
        {
            private readonly ILogger<DashboardListWithViewAssignToNodeIdHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IRepository<Dashboard> _repository;
            private readonly IUserService _userService;
            private readonly IRepository<ViewTemplate> _viewTemplateRepository;

            public DashboardListWithViewAssignToNodeIdHandler(
                ILogger<DashboardListWithViewAssignToNodeIdHandler> logger,
                IMapper mapper,
                IRepository<Dashboard> repository,
                IRepository<ViewTemplate> viewTemplateRepository,
                IUserService userService)
            {
                _logger = logger;
                _mapper = mapper;
                _repository = repository;
                _viewTemplateRepository = viewTemplateRepository;
                _userService = userService;
            }

            public async Task<List<DashboardViewModel>> Handle(DashboardListWithViewAssignToNodeIdQuery request,
                CancellationToken cancellationToken)
            {
                _logger.LogDebug("START");

                var userId = UtilitySecurity.GetUserId(request.SpecificUser);


                var dashboardList =
                    await _repository.FindAsync(new DashboardByNodeIdSpecification(request.NodeId, true));
                if (dashboardList == null)
                    //error handling
                    return null;

                var dashboardViews = new List<ViewTemplateDto>();
                var viewIds = dashboardList.SelectMany(i => i.Views.Select(i => i.ViewTemplateId)).Distinct().ToList();
                if (viewIds != null && viewIds.Count > 0)
                {
                    var entities =
                        await _viewTemplateRepository.FindAsync(new ViewTemplateByMultiIdsSpecification(viewIds));
                    var dashView = entities.Where(i => i.NodeFK == request.NodeId)
                        .Select(i => i.ConvertToViewTemplateDto(_mapper));
                    if (dashView != null && dashView.Any()) dashboardViews.AddRange(dashView);
                }

                var resultList = new List<DashboardViewModel>();
                foreach (var dashboardEntity in dashboardList)
                {
                    if (dashboardEntity.Views == null ||
                        !dashboardEntity.Views.Any(i => dashboardViews.Any(k => k.ViewTemplateId == i.ViewTemplateId)))
                        continue;

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
                        ViewIds = dashboardViews.Select(i => i.ViewTemplateId).ToList(),
                        Views = dashboardViews
                            ?.Where(i => dashboardEntity.Views.Any(k => k.ViewTemplateId == i.ViewTemplateId))
                            ?.ToDictionary(x => x.ViewTemplateId, y => ViewTemplateViewModel.ConvertFromDto(y)),
                        FilterLevels = dashboardEntity.FilterLevels
                    };
                    resultList.Add(dashModelView);
                }

                return resultList;
            }
        }
    }
}