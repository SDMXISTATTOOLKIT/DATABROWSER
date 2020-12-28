using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace DataBrowser.AuthenticationAuthorization.Handler
{
    public class AllowAnonymous : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            foreach (var requirement in context.PendingRequirements.ToList()) context.Succeed(requirement);


            return Task.CompletedTask;
        }
    }
}