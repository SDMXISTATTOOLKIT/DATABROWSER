using System.Security.Claims;
using DataBrowser.Domain.Dtos;
using DataBrowser.Interfaces.Authentication;
using DataBrowser.Interfaces.Configuration;
using DataBrowser.Interfaces.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace DataBrowser.AuthenticationAuthorization.Filters
{
    public class FilterTemplate : IFilterTemplate
    {
        private readonly AuthenticationConfig _authenticationConfig;
        private readonly IFilterNode _filterNode;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FilterTemplate(IHttpContextAccessor httpContextAccessor,
            IOptionsSnapshot<AuthenticationConfig> authenticationConfig,
            IFilterNode filterNode)
        {
            _httpContextAccessor = httpContextAccessor;
            _authenticationConfig = authenticationConfig.Value;
            _filterNode = filterNode;
        }

        public bool CheckPermission(ViewTemplateDto template, ClaimsPrincipal specificUser = null)
        {
            if (template == null || template.Type != ViewTemplateType.Template) return false;

            if (!_authenticationConfig.IsActive) return true;

            if (specificUser == null) specificUser = _httpContextAccessor?.HttpContext?.User;

            if (!_filterNode.CheckPermissionNodeManageTemplate(template.NodeId, specificUser)) return false;

            if (specificUser != null &&
                UtilitySecurity.CheckNodePermission(specificUser,
                    PermissionType.NodePermission.ManageTemplate,
                    template.NodeId))
                return true;

            return false;
        }
    }
}