using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataBrowser.Domain.Entities.Users;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DataBrowser.Entities.SQLite
{
    public abstract class BaseDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int,
        ApplicationUserClaim, ApplicationUserRole, IdentityUserLogin<int>,
        ApplicationRoleClaim, IdentityUserToken<int>>
    {
        protected BaseDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public IRequestContext requestContext { get; set; }

        public override int SaveChanges()
        {
            ApplyGeneralAudit();
            var result = base.SaveChanges();
            return result;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyGeneralAudit();
            var result = await base.SaveChangesAsync(cancellationToken);
            return result;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        protected virtual void ApplyGeneralAudit()
        {
            var userId = requestContext?.LoggedUserId ?? -1;

            var entries = ChangeTracker.Entries().ToList();
            foreach (var entry in entries)
                switch (entry.State)
                {
                    case EntityState.Added:
                        SetCreationAuditProperties(entry.Entity, userId);
                        break;
                    case EntityState.Modified:
                        SetModificationAuditProperties(entry, userId);

                        break;
                    //case EntityState.Deleted:
                    //CancelDeletionForSoftDelete(entry);
                    //break;
                }

            //AddDomainEvents(changeReport.DomainEvents, entry.Entity);
        }

        protected virtual void SetCreationAuditProperties(object entityAsObj, long? userId)
        {
            var entityWithCreationTime = entityAsObj as IHasCreationTime;
            if (entityWithCreationTime == null) return;

            if (entityWithCreationTime.CreationTime == default) entityWithCreationTime.CreationTime = DateTime.UtcNow;

            if (userId.HasValue && entityAsObj is ICreationAudited)
            {
                var entity = entityAsObj as ICreationAudited;
                if (entity.CreatorUserId == null) entity.CreatorUserId = userId;
            }
        }

        protected virtual void SetModificationAuditProperties(EntityEntry entry, long? userId)
        {
            if (entry.Entity is IHasModificationTime)
                ((IHasModificationTime) entry.Entity).LastModificationTime = DateTime.UtcNow;

            if (entry.Entity is IModificationAudited)
            {
                var entity = (IModificationAudited) entry.Entity;

                if (userId == null)
                {
                    entity.LastModifierUserId = null;
                    return;
                }

                entity.LastModifierUserId = userId;
            }

            /*
            protected virtual void CancelDeletionForSoftDelete(EntityEntry entry)
            {
                if (!(entry.Entity is ISoftDelete))
                {
                    return;
                }

                entry.Reload();
                entry.State = EntityState.Modified;
                entry.Entity.As<ISoftDelete>().IsDeleted = true;
            }

            protected virtual void SetDeletionAuditProperties(object entityAsObj, long? userId)
            {
                if (entityAsObj is IHasDeletionTime)
                {
                    var entity = entityAsObj.As<IHasDeletionTime>();

                    if (entity.DeletionTime == null)
                    {
                        entity.DeletionTime = Clock.Now;
                    }
                }

                if (entityAsObj is IDeletionAudited)
                {
                    var entity = entityAsObj.As<IDeletionAudited>();

                    if (entity.DeleterUserId != null)
                    {
                        return;
                    }

                    if (userId == null)
                    {
                        entity.DeleterUserId = null;
                        return;
                    }

                    //Special check for multi-tenant entities
                    entity.DeleterUserId = userId;
                }
            }
            */
        }
    }
}