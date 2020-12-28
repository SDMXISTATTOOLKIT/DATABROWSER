using DataBrowser.Domain.Entities.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Domain.Entities.Users
{
    public class UserRemovedPublicEvent : PublicEventBase
    {
        public int UserId { get; }

        public UserRemovedPublicEvent(int userId)
        {
            UserId = userId;
        }

    }
}
