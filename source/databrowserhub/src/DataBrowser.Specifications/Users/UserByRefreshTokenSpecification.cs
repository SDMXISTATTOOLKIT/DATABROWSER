using DataBrowser.Domain.Entities.Users;
using DataBrowser.Domain.Specifications.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataBrowser.Specifications.Users
{
    public class UserByRefreshTokenSpecification : BaseSpecification<ApplicationUser>
    {
        public UserByRefreshTokenSpecification(string refreshToken)
            : base(b => b.RefreshTokens != null && b.RefreshTokens.Any(i => i.Token != null && i.Token.Equals(refreshToken)))
        {
            AddInclude("RefreshTokens");
        }
    }
}
