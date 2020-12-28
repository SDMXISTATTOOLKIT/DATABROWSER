using System.Linq;
using System.Threading.Tasks;
using DataBrowser.Interfaces.Configuration;
using DataBrowser.Interfaces.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace DataBrowser.AuthenticationAuthorization.Policy
{
    public class AuthenticatedUserPolicy : IAuthorizationRequirement
    {
    }

    public class AuthenticatedUserHandler : AuthorizationHandler<AuthenticatedUserPolicy>
    {
        private readonly AuthenticationConfig _authenticationConfig;

        public AuthenticatedUserHandler(IOptionsSnapshot<AuthenticationConfig> authenticationConfig)
        {
            _authenticationConfig = authenticationConfig.Value;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            AuthenticatedUserPolicy requirement)
        {
            if (!_authenticationConfig.IsActive)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (context?.User?.Identity == null ||
                !context.User.Identity.IsAuthenticated)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var hasClaimUserService =
                context.User.Claims?.Any(c => c.Type == ClaimValues.ServiceUser && c.Value == "1");
            if (hasClaimUserService.HasValue &&
                hasClaimUserService.Value)
                context.Fail();
            else
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}