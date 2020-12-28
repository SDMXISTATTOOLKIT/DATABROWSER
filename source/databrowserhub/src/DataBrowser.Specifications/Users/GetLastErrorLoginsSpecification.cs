using DataBrowser.Domain.Entities.Users;
using DataBrowser.Domain.Specifications.Query;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Specifications.Users
{
    public class GetLastErrorLoginsSpecification : BaseSpecification<UserAudit>
    {
        public GetLastErrorLoginsSpecification(int userId, int minuteTimeRange)
            : base(b => b.UserId == userId.ToString() && b.Timestamp > DateTime.UtcNow.AddMinutes(-minuteTimeRange))
        {
            ApplyOrderByDescending(p => p.Timestamp);
        }
    }
}
