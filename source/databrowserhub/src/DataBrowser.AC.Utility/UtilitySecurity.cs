using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using DataBrowser.Interfaces.Authentication;
using DataBrowser.Interfaces.Constants;

namespace DataBrowser.AuthenticationAuthorization
{
    public class UtilitySecurity
    {
        public static int GetUserId(ClaimsPrincipal user)
        {
            var userId = -1;
            if (user?.Identity == null || !user.Identity.IsAuthenticated || user?.Claims == null) return userId;
            var claimUserId = user.Claims.Where(c => c.Type == ClaimValues.UserId).Select(c => c.Value)
                .SingleOrDefault();
            if (claimUserId == null) return userId;

            int.TryParse(claimUserId, out userId);

            return userId;
        }

        public static bool IsAdminUser(ClaimsPrincipal user)
        {
            if (user?.Identity == null || !user.Identity.IsAuthenticated || user?.Claims == null) return false;

            return user.IsInRole(UserAndGroup.Roles.Administrator.ToString());
        }

        public static bool CheckNodePermission(ClaimsPrincipal user, PermissionType.NodePermission nodePermission,
            int nodeId)
        {
            if (user?.Identity == null || !user.Identity.IsAuthenticated || user?.Claims == null) return false;

            if (user.IsInRole(UserAndGroup.Roles.Administrator.ToString()) &&
                nodePermission != PermissionType.NodePermission.ManageView)
                return true;

            var claimUserId = user.Claims.FirstOrDefault(c => c.Type != null &&
                                                              (c.Type == $"{PermissionType.PermissionSingleNodeType}{nodeId}" &&
                                                               c.Value.Equals(nodePermission.ToString(),
                                                                   StringComparison.InvariantCultureIgnoreCase) ||
                                                               c.Type == $"{PermissionType.PermissionCroosNodeType}" &&
                                                               c.Value.Equals(nodePermission.ToString(),
                                                                   StringComparison.InvariantCultureIgnoreCase)));
            if (claimUserId == null) return false;

            return true;
        }

        public static List<int> GetNodesWithManageConfig(ClaimsPrincipal user, IFilterNode filterNode)
        {
            var allNodes = user.Claims.FirstOrDefault(c => c.Type != null &&
                                                           c.Type == $"{PermissionType.PermissionCroosNodeType}" &&
                                                           c.Value.Equals(PermissionType.NodePermission.ManageConfig.ToString(),
                                                               StringComparison.InvariantCultureIgnoreCase));
            if (allNodes != null) return new List<int> {-1}; //Manage all nodes

            var claimsNodesId = user.Claims.Where(c => c.Type != null &&
                                                       c.Type.StartsWith($"{PermissionType.PermissionSingleNodeType}",
                                                           StringComparison.InvariantCultureIgnoreCase) &&
                                                       c.Value.Equals(
                                                           PermissionType.NodePermission.ManageConfig.ToString(),
                                                           StringComparison.InvariantCultureIgnoreCase)
            ).ToList();

            var nodesManage = new List<int>();
            foreach (var claimNode in claimsNodesId)
            {
                var strId = claimNode.Type.Replace(PermissionType.PermissionSingleNodeType, "",
                    StringComparison.InvariantCultureIgnoreCase);
                var nodeId = -1;
                int.TryParse(strId, out nodeId);

                if (nodeId == -1) continue;

                if (filterNode.CheckPermissionNodeManageConfig(nodeId, user)) nodesManage.Add(nodeId);
            }

            return nodesManage;
        }
    }
}