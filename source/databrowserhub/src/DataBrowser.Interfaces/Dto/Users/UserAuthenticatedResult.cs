using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DataBrowser.Interfaces.Dto.Users
{
    public class UserAuthenticatedResult
    {
        public string Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Type { get; set; }
        public List<string> Roles { get; set; }
        public List<string> Permissions { get; set; }
        public string Token { get; set; }

        [JsonIgnore] public string RefreshToken { get; set; }

        public DateTime RefreshTokenExpiration { get; set; }
        public double TokenTTL { get; set; }
    }
}