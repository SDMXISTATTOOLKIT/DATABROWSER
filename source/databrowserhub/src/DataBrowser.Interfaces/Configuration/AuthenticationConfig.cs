namespace DataBrowser.Interfaces.Configuration
{
    public class AuthenticationConfig
    {
        public bool IsActive { get; set; }
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public double JwtTokenLifeTime { get; set; }
        public bool EnableRefreshToken { get; set; }
        public bool EnableAuditLogin { get; set; }
        public string UserServicesPassword { get; set; }
        public int TryLoginMax { get; set; }
        public int TryLoginTime { get; set; }
        public int DelayLogin { get; set; }
        public Cookieoptions RefreshCookieOptions { get; set; }
        public Userpolicy UserPolicy { get; set; }


        public class Cookieoptions
        {
            public string Name { get; set; }
            public string Domain { get; set; }
            public bool? HttpOnly { get; set; }
            public string Path { get; set; }
            public string SameSite { get; set; }
            public bool? Secure { get; set; }
            public double? RefreshTokenLifeTime { private get; set; }

            public double RefreshTokenLifeTimeOrDefaultValue
            {
                get
                {
                    if (!RefreshTokenLifeTime.HasValue) return 14400;

                    return RefreshTokenLifeTime.Value;
                }
            }
        }

        public class Userpolicy
        {
            public int PasswordRequiredLength { get; set; }
            public bool PasswordRequireNonAlphanumeric { get; set; }
            public bool PasswordRequireLowercase { get; set; }
            public bool PasswordRequireUppercase { get; set; }
            public bool PasswordRequireDigit { get; set; }
        }
    }
}