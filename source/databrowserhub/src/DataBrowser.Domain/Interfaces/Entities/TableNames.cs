using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace DataBrowser.Domain.Interfaces.Entities
{
    static public class TableNames
    {
        public const string RoleClaims = "AspNetRoleClaims";
        public const string Roles = "AspNetRoles";
        public const string UserClaims = "AspNetUserClaims";
        public const string UserLogins = "AspNetUserLogins";
        public const string UserRoles = "AspNetUserRoles";
        public const string UserTokens = "AspNetUserTokens";
        public const string Users = "AspNetUsers";
        public const string UsersAudit = "AspNetUsersAudit";
        public const string UsersRefreshToken = "AspNetUsersRefreshToken";
        public const string DashboardViewTemplates = "DashboardViewTemplates";
        public const string DashboardNodes = "DashboardNodes";
    }
}
