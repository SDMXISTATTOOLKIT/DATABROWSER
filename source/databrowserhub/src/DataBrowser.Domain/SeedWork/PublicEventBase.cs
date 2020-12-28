using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Domain.Entities.SeedWork
{
    public class PublicEventBase : IPublicEvent
    {
        public PublicEventBase()
        {
            this.OccurredOn = DateTime.UtcNow;
        }

        public DateTime OccurredOn { get; }
    }
}
