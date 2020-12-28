using System.Security.Claims;
using DataBrowser.Domain.Dtos;
using DataBrowser.Interfaces.Authentication;
using DataBrowser.Interfaces.Configuration;
using DataBrowser.Interfaces.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;

namespace DataBrowser.AuthenticationAuthorization.Filters
{
    public class FilterDashboard : IFilterDashboard
    {
        private readonly AuthenticationConfig _authenticationConfig;
        private readonly IFilterNode _filterNode;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FilterDashboard(IHttpContextAccessor httpContextAccessor,
            IOptionsSnapshot<AuthenticationConfig> authenticationConfig,
            IFilterNode filterNode)
        {
            _httpContextAccessor = httpContextAccessor;
            _authenticationConfig = authenticationConfig.Value;
            _filterNode = filterNode;
        }

        public bool CheckReadPermission(DashboardDto dashboard, ClaimsPrincipal specificUser = null)
        {
            if (dashboard == null) return false;

            if (!_authenticationConfig.IsActive) return true;

            if (specificUser == null) specificUser = _httpContextAccessor?.HttpContext?.User;

            var havePermission = specificUser?.IsInRole(UserAndGroup.Roles.Administrator.ToString());
            if (havePermission.HasValue && havePermission.Value) return true;

            return dashboard.UserId == UtilitySecurity.GetUserId(specificUser) ||
                   dashboard.HubId > 0 ||
                   dashboard.NodeIds != null && dashboard.NodeIds.Any();
        }

        public bool CheckWritePermission(DashboardDto dashboard, ClaimsPrincipal specificUser = null)
        {
            if (dashboard == null) return false;

            if (!_authenticationConfig.IsActive) return true;

            if (specificUser == null) specificUser = _httpContextAccessor?.HttpContext?.User;

            if (specificUser?.Identity == null || !specificUser.Identity.IsAuthenticated) return false;

            var haveAdminPermission = specificUser?.IsInRole(UserAndGroup.Roles.Administrator.ToString());
            if (haveAdminPermission.HasValue && haveAdminPermission.Value) return true;

            var nodesManage = UtilitySecurity.GetNodesWithManageConfig(specificUser, _filterNode);
            if (nodesManage.Count > 0)
                //User with one (or more) ManageConfig permission can create dashboard 
                return true;

            return false;
        }
    }
}