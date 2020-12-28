using System.Security.Claims;

namespace DataBrowser.Interfaces.Authentication
{
    public interface IFilterNode
    {
        bool CheckPermissionNodeManageConfig(int nodeId, ClaimsPrincipal specificUser = null);
        bool CheckPermissionNodeManageView(int nodeId, ClaimsPrincipal specificUser = null);
        bool CheckPermissionNodeManageTemplate(int nodeId, ClaimsPrincipal specificUser = null);
        bool CheckPermissionNodeManageCache(int nodeId, ClaimsPrincipal specificUser = null);
    }
}