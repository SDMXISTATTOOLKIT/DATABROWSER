using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Domain.Entities.SeedWork
{
    public interface IPublicEvent : INotification
    {
        //DateTime OccurredOn { get; }
    }
}
