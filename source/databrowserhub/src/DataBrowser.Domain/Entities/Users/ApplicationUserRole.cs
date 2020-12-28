using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Domain.Entities.Users
{
    public class ApplicationUserRole : IdentityUserRole<int>
    {
        public virtual ApplicationRole Role { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
