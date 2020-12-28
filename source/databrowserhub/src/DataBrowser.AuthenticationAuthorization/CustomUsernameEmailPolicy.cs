using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataBrowser.Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;

namespace DataBrowser.AuthenticationAuthorization
{
    public class CustomUsernameEmailPolicy : UserValidator<ApplicationUser>
    {
        public override async Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager,
            ApplicationUser user)
        {
            var result = await base.ValidateAsync(manager, user);
            var errors = result.Succeeded ? new List<IdentityError>() : result.Errors.ToList();

            if (user.UserName.Equals(user.Email,System.StringComparison.InvariantCultureIgnoreCase))
            {
                errors.RemoveAll((error) => error.Code.Equals("DuplicateUserName", System.StringComparison.InvariantCultureIgnoreCase));
            }

            
            return errors.Count == 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray());
        }
    }
}