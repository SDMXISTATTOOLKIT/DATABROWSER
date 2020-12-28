using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
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
    public class DashboardsMinimalInfoByUserOrPublicQuery : DashboardQueryBase,
        IQuery<List<DashboardMinimalInfoViewModel>>
    {
        public DashboardsMinimalInfoByUserOrPublicQuery(ClaimsPrincipal filterBySpecificUser = null,
            bool filterByPermission = false) :
            base(filterByPermission, filterBySpecificUser)
        {
        }

        public class DashboardsMinimalInfoByUserOrPublicHandler : IRequestHandler<
            DashboardsMinimalInfoByUserOrPublicQuery, List<DashboardMinimalInfoViewModel>>
        {
            private readonly ILogger<DashboardsMinimalInfoByUserOrPublicHandler> _logger;
            private readonly IRepository<Dashboard> _repository;
            private readonly IUserService _userService;

            public DashboardsMinimalInfoByUserOrPublicHandler(
                ILogger<DashboardsMinimalInfoByUserOrPublicHandler> logger,
                IRepository<Dashboard> repository,
                IUserService userService)
            {
                _logger = logger;
                _repository = repository;
                _userService = userService;
            }

            public async Task<List<DashboardMinimalInfoViewModel>> Handle(
                DashboardsMinimalInfoByUserOrPublicQuery request, CancellationToken cancellationToken)
            {
                _logger.LogDebug("START");
                var userId = UtilitySecurity.GetUserId(request.SpecificUser);


                var dashboardList =
                    await _repository.FindAsync(new DashboardByUserIdSpecification(userId, withNodes: false));
                if (dashboardList == null)
                    //error handling
                    return null;

                var resultList = dashboardList?.Select(async i => new DashboardMinimalInfoViewModel
                {
                    DashboardId = i.DashboardId,
                    UserId = i.UserFk,
                    Username = await _userService.GetUsername(i.UserFk),
                    Title = i?.Title?.TransatableItemValues?.ToDictionary(val => val.Language, val => val.Value)
                });

                var results = await Task.WhenAll(resultList);

                return results.ToList();
            }
        }
    }
}