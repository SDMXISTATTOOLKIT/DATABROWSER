using DataBrowser.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Domain.Entities.Users
{
    public class UserAudit : IAggregateRoot
    {
        public int UserAuditId { get; private set; }
        public string UserId { get; private set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public UserAuditEventType AuditEvent { get; set; }

        public string IpAddress { get; private set; }

        public static UserAudit CreateAuditEvent(string userId, UserAuditEventType auditEventType, string ipAddress)
        {
            return new UserAudit { UserId = userId, AuditEvent = auditEventType, IpAddress = ipAddress };
        }
    }

    public enum UserAuditEventType
    {
        Login = 1,
        FailedLogin = 2,
        LogOut = 3
    }
}
