using System;
using System.Linq;
using System.Threading.Tasks;
using DataBrowser.Interfaces.Configuration;
using DataBrowser.Interfaces.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace DataBrowser.AuthenticationAuthorization.Policy
{
    public class AllowUploadFilePolicy : IAuthorizationRequirement
    {
    }

    public class AllowUploadFileHandler : AuthorizationHandler<AllowUploadFilePolicy>
    {
        private readonly AuthenticationConfig _authenticationConfig;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AllowUploadFileHandler(IHttpContextAccessor httpContextAccessor,
            IOptionsSnapshot<AuthenticationConfig> authenticationConfig)
        {
            _httpContextAccessor = httpContextAccessor;
            _authenticationConfig = authenticationConfig.Value;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            AllowUploadFilePolicy requirement)
        {
            if (!_authenticationConfig.IsActive)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var user = _httpContextAccessor?.HttpContext?.User;
            if (user?.Identity == null ||
                !user.Identity.IsAuthenticated)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var havePermission = user?.IsInRole(UserAndGroup.Roles.Administrator.ToString());

            if (!havePermission.HasValue || !havePermission.Value)
            {
                var permTypeSingleNode = $"{PermissionType.PermissionSingleNodeType}";
                havePermission = user?.Claims?.Any(i =>
                    i.Type.StartsWith(permTypeSingleNode, StringComparison.OrdinalIgnoreCase) &&
                    (i.Value.Equals(PermissionType.NodePermission.Full.ToString(), StringComparison.OrdinalIgnoreCase) ||
                     i.Value.Equals(PermissionType.NodePermission.ManageConfig.ToString(), StringComparison.OrdinalIgnoreCase)));
            }
            if (!havePermission.HasValue || !havePermission.Value)
            {
                var permTypeAllNode = $"{PermissionType.PermissionCroosNodeType}";
                havePermission = user?.Claims?.Any(i =>
                    i.Type.Equals(permTypeAllNode, StringComparison.OrdinalIgnoreCase) &&
                    (i.Value.Equals(PermissionType.NodePermission.Full.ToString(), StringComparison.OrdinalIgnoreCase) ||
                     i.Value.Equals(PermissionType.NodePermission.ManageConfig.ToString(), StringComparison.OrdinalIgnoreCase)));
            }

            if (havePermission.HasValue && havePermission.Value)
                context.Succeed(requirement);
            else
                context.Fail();
            return Task.CompletedTask;
        }
    }
}