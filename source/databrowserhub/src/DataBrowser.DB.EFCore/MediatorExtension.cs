using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataBrowser.Domain.Entities.SeedWork;
using MediatR;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DataBrowser.DB.EFCore
{
    internal static class MediatorExtension
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, List<EntityEntry<Entity>> entities)
        {
            var domainEvents = entities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            entities.ToList()
                .ForEach(entity => entity.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
                await mediator.Publish(domainEvent);
        }

        public static async Task DispatchPublicDomainEventsAsync(this IMediator mediator,
            List<EntityEntry<Entity>> entities)
        {
            var publicDomainEvents = entities
                .SelectMany(x => x.Entity.PublicDomainEvents)
                .ToList();

            entities.ToList()
                .ForEach(entity => entity.Entity.ClearPublicDomainEvents());

            foreach (var domainEvent in publicDomainEvents)
                await mediator.Publish(domainEvent);
        }
    }
}