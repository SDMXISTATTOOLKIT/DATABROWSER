using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Domain.Entities.Users
{
    public class ApplicationRole : IdentityRole<int>
    {
        public ApplicationRole() : base()
        {

        }
        public ApplicationRole(string roleName) : base(roleName)
        {

        }

        private readonly List<ApplicationUserRole> _userRoles = new List<ApplicationUserRole>();
        public virtual IReadOnlyCollection<ApplicationUserRole> UserRoles => _userRoles?.AsReadOnly();

        private readonly List<ApplicationRoleClaim> _roleClaims = new List<ApplicationRoleClaim>();
        public virtual IReadOnlyCollection<ApplicationRoleClaim> RoleClaims => _roleClaims?.AsReadOnly();
    }
}
