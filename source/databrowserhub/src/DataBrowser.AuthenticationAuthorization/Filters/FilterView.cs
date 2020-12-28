using System.Security.Claims;
using DataBrowser.Domain.Dtos;
using DataBrowser.Interfaces.Authentication;
using DataBrowser.Interfaces.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace DataBrowser.AuthenticationAuthorization.Filters
{
    public class FilterView : IFilterView
    {
        private readonly AuthenticationConfig _authenticationConfig;
        private readonly IFilterNode _filterNode;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FilterView(IHttpContextAccessor httpContextAccessor,
            IOptionsSnapshot<AuthenticationConfig> authenticationConfig,
            IFilterNode filterNode)
        {
            _httpContextAccessor = httpContextAccessor;
            _authenticationConfig = authenticationConfig.Value;
            _filterNode = filterNode;
        }

        public bool CheckPermission(ViewTemplateDto view, ClaimsPrincipal specificUser = null)
        {
            if (view == null || view.Type != ViewTemplateType.View) return false;

            if (!_authenticationConfig.IsActive) return true;

            if (specificUser == null) specificUser = _httpContextAccessor?.HttpContext?.User;

            if (!_filterNode.CheckPermissionNodeManageView(view.NodeId, specificUser)) return false;

            if (specificUser != null &&
                UtilitySecurity.GetUserId(specificUser) == view.UserId)
                return true;

            return false;
        }
    }
}