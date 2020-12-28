using System;
using System.Linq;
using System.Security.Claims;
using DataBrowser.Interfaces.Authentication;
using DataBrowser.Interfaces.Configuration;
using DataBrowser.Interfaces.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace DataBrowser.AuthenticationAuthorization.Filters
{
    public class FilterNode : IFilterNode
    {
        private readonly AuthenticationConfig _authenticationConfig;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FilterNode(IHttpContextAccessor httpContextAccessor,
            IOptionsSnapshot<AuthenticationConfig> authenticationConfig)
        {
            _httpContextAccessor = httpContextAccessor;
            _authenticationConfig = authenticationConfig.Value;
        }

        public bool CheckPermissionNodeManageConfig(int nodeId, ClaimsPrincipal specificUser = null)
        {
            if (specificUser == null) specificUser = _httpContextAccessor?.HttpContext?.User;
            return checkPermissionBySpecificUser(PermissionType.NodePermission.ManageConfig, nodeId, specificUser);
        }

        public bool CheckPermissionNodeManageCache(int nodeId, ClaimsPrincipal specificUser = null)
        {
            if (specificUser == null) specificUser = _httpContextAccessor?.HttpContext?.User;
            return checkPermissionBySpecificUser(PermissionType.NodePermission.ManageCache, nodeId, specificUser);
        }

        public bool CheckPermissionNodeManageTemplate(int nodeId, ClaimsPrincipal specificUser = null)
        {
            if (specificUser == null) specificUser = _httpContextAccessor?.HttpContext?.User;
            return checkPermissionBySpecificUser(PermissionType.NodePermission.ManageTemplate, nodeId, specificUser);
        }

        public bool CheckPermissionNodeManageView(int nodeId, ClaimsPrincipal specificUser = null)
        {
            if (specificUser == null) specificUser = _httpContextAccessor?.HttpContext?.User;
            return checkPermissionBySpecificUser(PermissionType.NodePermission.ManageView, nodeId, specificUser);
        }

        private bool checkPermissionBySpecificUser(PermissionType.NodePermission nodePermission, int nodeId,
            ClaimsPrincipal specificUser)
        {
            if (!_authenticationConfig.IsActive) return true;

            if (specificUser?.Identity == null || !specificUser.Identity.IsAuthenticated) return false;

            var havePermission = specificUser?.IsInRole(UserAndGroup.Roles.Administrator.ToString());

            //CroosNode
            if (!havePermission.HasValue || !havePermission.Value)
            {
                var permTypeAllNode = $"{PermissionType.PermissionCroosNodeType}";
                havePermission = specificUser?.Claims?.Any(i =>
                    i.Type.Equals(permTypeAllNode, StringComparison.OrdinalIgnoreCase) &&
                    (i.Value.Equals(PermissionType.NodePermission.Full.ToString(),
                         StringComparison.OrdinalIgnoreCase) ||
                     i.Value.Equals(nodePermission.ToString(), StringComparison.OrdinalIgnoreCase)));
            }

            if (!havePermission.HasValue || !havePermission.Value)
            {
                var permTypeSingleNode = $"{PermissionType.PermissionCroosNodeType}";
                havePermission = specificUser?.Claims?.Any(i =>
                    i.Type.Equals(permTypeSingleNode, StringComparison.OrdinalIgnoreCase) &&
                    (i.Value.Equals(PermissionType.NodePermission.Full.ToString(),
                         StringComparison.OrdinalIgnoreCase) ||
                     i.Value.Equals(nodePermission.ToString(), StringComparison.OrdinalIgnoreCase)));
            }

            //Single Node
            if (!havePermission.HasValue || !havePermission.Value)
            {
                var permTypeAllNode = $"{PermissionType.PermissionSingleNodeType}";
                havePermission = specificUser?.Claims?.Any(i =>
                    i.Type.Equals(permTypeAllNode, StringComparison.OrdinalIgnoreCase) &&
                    (i.Value.Equals(PermissionType.NodePermission.Full.ToString(),
                         StringComparison.OrdinalIgnoreCase) ||
                     i.Value.Equals(nodePermission.ToString(), StringComparison.OrdinalIgnoreCase)));
            }

            if (!havePermission.HasValue || !havePermission.Value)
            {
                var permTypeSingleNode = $"{PermissionType.PermissionSingleNodeType}{nodeId}";
                havePermission = specificUser?.Claims?.Any(i =>
                    i.Type.Equals(permTypeSingleNode, StringComparison.OrdinalIgnoreCase) &&
                    (i.Value.Equals(PermissionType.NodePermission.Full.ToString(),
                         StringComparison.OrdinalIgnoreCase) ||
                     i.Value.Equals(nodePermission.ToString(), StringComparison.OrdinalIgnoreCase)));
            }

            if (!havePermission.HasValue || !havePermission.Value) return false;

            return true;
        }
    }
}