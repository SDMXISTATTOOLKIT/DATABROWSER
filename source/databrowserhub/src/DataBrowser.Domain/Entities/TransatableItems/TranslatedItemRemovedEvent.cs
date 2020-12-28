using DataBrowser.Domain.Entities.SeedWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Domain.Entities.TransatableItems
{
    public class TranslatedItemRemovedEvent : IPublicEvent
    {
        public int TranslatedItemId { get; }

        public TranslatedItemRemovedEvent(int translatedItemId)
        {
            TranslatedItemId = translatedItemId;
        }
    }
}
