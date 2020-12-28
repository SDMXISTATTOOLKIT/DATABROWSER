using DataBrowser.Domain.Interfaces.Entities;
using DataBrowser.Interfaces.Entities;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;

namespace DataBrowser.Domain.Entities.Users
{
    public class ApplicationUser : IdentityUser<int>, IAggregateRoot, IEntity
    {
        public string Settings { get; set; }
        public bool IsSuperAdmin { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Organizzation { get; set; }
        public bool IsDisable { get; set; }
        public string Type { get; set; }

        private readonly List<RefreshToken> _refreshTokens = new List<RefreshToken>();
        public virtual IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens?.AsReadOnly();

        private readonly List<ApplicationUserRole> _userRoles = new List<ApplicationUserRole>();
        public virtual IReadOnlyCollection<ApplicationUserRole> UserRoles => _userRoles?.AsReadOnly();

        private readonly List<ApplicationUserClaim> _userClaims = new List<ApplicationUserClaim>();
        public virtual IReadOnlyCollection<ApplicationUserClaim> UserClaims => _userClaims?.AsReadOnly();

        public ApplicationUser()
        {

        }

        public void AddToken(RefreshToken refreshToken)
        {
            if (refreshToken == null)
            {
                return;
            }
            _refreshTokens.Add(refreshToken);
        }

        public void RemoveToken(string token)
        {
            var refreshToken = _refreshTokens.FirstOrDefault(i => i.Token.Equals(token));
            if (refreshToken == null)
            {
                return;
            }
            _refreshTokens.Remove(refreshToken);
        }

    }
}
