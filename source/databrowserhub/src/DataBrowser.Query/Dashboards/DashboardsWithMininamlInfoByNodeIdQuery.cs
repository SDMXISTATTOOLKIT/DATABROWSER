using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DataBrowser.AuthenticationAuthorization;
using DataBrowser.Domain.Entities.DBoard;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Interfaces.Authentication;
using DataBrowser.Interfaces.Mediator;
using DataBrowser.Query.Dashboards.ModelView;
using DataBrowser.Specifications.Dashboards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataBrowser.Query.Dashboards
{
    public class DashboardsWithMininamlInfoByNodeIdQuery : DashboardQueryBase,
        IQuery<List<DashboardMinimalInfoViewModel>>
    {
        public int NodeId;

        public DashboardsWithMininamlInfoByNodeIdQuery(int nodeId,
            ClaimsPrincipal filterBySpecificUser,
            bool filterByPermission = false) :
            base(filterByPermission, filterBySpecificUser)
        {
            NodeId = nodeId;
        }

        public class DashboardsWithMininamlInfoByNodeIdHandler : IRequestHandler<DashboardsWithMininamlInfoByNodeIdQuery
            , List<DashboardMinimalInfoViewModel>>
        {
            private readonly ILogger<DashboardsWithMininamlInfoByNodeIdHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IRepository<Dashboard> _repository;
            private readonly IUserService _userService;

            public DashboardsWithMininamlInfoByNodeIdHandler(ILogger<DashboardsWithMininamlInfoByNodeIdHandler> logger,
                IMapper mapper,
                IRepository<Dashboard> repository,
                IUserService userService)
            {
                _logger = logger;
                _mapper = mapper;
                _repository = repository;
                _userService = userService;
            }

            public async Task<List<DashboardMinimalInfoViewModel>> Handle(
                DashboardsWithMininamlInfoByNodeIdQuery request, CancellationToken cancellationToken)
            {
                _logger.LogDebug($"START nodeId: {request.NodeId}");

                var userId = UtilitySecurity.GetUserId(request.SpecificUser);


                var dashboardList =
                    await _repository.FindAsync(new DashboardByNodeIdSpecification(request.NodeId, false));
                if (dashboardList == null)
                    //error handling
                    return null;

                var resultList = new List<DashboardMinimalInfoViewModel>();
                foreach (var dashboardEntity in dashboardList)
                {
                    var dashModelView = new DashboardMinimalInfoViewModel
                    {
                        DashboardId = dashboardEntity.DashboardId,
                        UserId = dashboardEntity.UserFk,
                        Username = await _userService.GetUsername(dashboardEntity.UserFk),
                        Title = dashboardEntity.Title.TransatableItemValues.ToDictionary(val => val.Language,
                            val => val.Value)
                    };
                    resultList.Add(dashModelView);
                }

                return resultList;
            }
        }
    }
}