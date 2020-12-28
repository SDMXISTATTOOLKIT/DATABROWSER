using System.Threading.Tasks;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Authentication;
using DataBrowser.Interfaces.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace DataBrowser.AuthenticationAuthorization.Policy
{
    public class AllowManageCachePolicy : IAuthorizationRequirement
    {
    }

    public class AllowManageCacheHandler : AuthorizationHandler<AllowManageCachePolicy>
    {
        private readonly AuthenticationConfig _authenticationConfig;
        private readonly IFilterNode _filterNode;
        private readonly IRequestContext _requestContext;

        public AllowManageCacheHandler(IRequestContext requestContext,
            IFilterNode filterNode,
            IOptionsSnapshot<AuthenticationConfig> authenticationConfig)
        {
            _requestContext = requestContext;
            _filterNode = filterNode;
            _authenticationConfig = authenticationConfig.Value;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            AllowManageCachePolicy requirement)
        {
            if (!_authenticationConfig.IsActive)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var havePermission = _filterNode.CheckPermissionNodeManageCache(_requestContext.NodeId);

            if (havePermission)
                context.Succeed(requirement);
            else
                context.Fail();
            return Task.CompletedTask;
        }
    }
}