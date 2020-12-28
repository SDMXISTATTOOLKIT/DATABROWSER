using System;

namespace DataBrowser.Interfaces.Constants
{
    public static class UserAndGroup
    {
        [Flags]
        public enum Roles
        {
            Administrator = 1,
            User = 1 << 1,
            Anonymous = 1 << 2,
            Service = 1 << 3
        }

        public const string RoleAdministrator = "Administrator";
        public const string RoleUser = "User";
        public const string RoleAnonymous = "Anonymous";
        public const string RoleService = "Service";

        public const string SuperAdminUsername = "admin@databrowser.com";
        public const string SuperAdminEmail = "admin@databrowser.com";
        public const string SuperAdminPassword = "DataBrowser1!";
        public const Roles DefaultRegisterRole = Roles.User;
        public const string ServiceUsername = "UserService";
        public const int ServiceUserId = int.MaxValue - 1;
    }
}