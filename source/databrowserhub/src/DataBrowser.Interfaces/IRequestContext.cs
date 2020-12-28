using System.Collections.Generic;
using System.Security.Claims;

namespace DataBrowser.Interfaces
{
    public interface IRequestContext
    {
        List<string> ApplicationLangs { get; }
        List<string> ApplicationLangsFromCurrentContext { get; }
        string CacheControl { get; }
        string CacheControlFromCurrentContext { get; }
        int DashboardId { get; }
        int DashboardIdFromCurrentContext { get; }
        bool IgnoreCache { get; }
        bool IgnoreCacheFromCurrentContext { get; }
        bool IsCacheRefresh { get; }
        bool IsCacheRefreshFromCurrentContext { get; }
        ClaimsPrincipal LoggedUser { get; }
        ClaimsPrincipal LoggedUserFromCurrentContext { get; }
        int LoggedUserId { get; }
        int LoggedUserIdFromCurrentContext { get; }
        string NodeCode { get; }
        string NodeCodeFromCurrentContext { get; }
        int NodeId { get; }
        int NodeIdFromCurrentContext { get; }
        int TemplateId { get; }
        int TemplateIdFromCurrentContext { get; }
        string UserGuid { get; }
        string UserGuidFromCurrentContext { get; }
        string UserLang { get; }
        string UserLangFromCurrentContext { get; }
        string UserOperationGuid { get; }
        string UserOperationGuidFromCurrentContext { get; }
        int ViewId { get; }
        int ViewIdFromCurrentContext { get; }

        void OverwriteApplicationLangs(List<string> applicationLangs);
        void OverwriteCacheControl(string cacheControl);
        void OverwriteDashboardId(int? dashboardId);
        void OverwriteIgnoreCache(bool? ignoreCache);
        void OverwriteIsCacheRefresh(bool? isCacheRefresh);
        void OverwriteLoggedUser(ClaimsPrincipal loggedUser);
        void OverwriteLoggedUserId(int? loggedUserId);
        void OverwriteNodeCode(string nodeCode);
        void OverwriteNodeId(int? nodeId);
        void OverwriteTemplateId(int? templateId);
        void OverwriteUserGuid(string userGuid);
        void OverwriteUserLang(string userLang);
        void OverwriteUserOperationGuid(string userOperationGuid);
        void OverwriteViewId(int? viewId);
    }
}