using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DataBrowser.DB.EFCore;
using DataBrowser.Domain.Entities.DBoard;
using DataBrowser.Domain.Entities.Hubs;
using DataBrowser.Domain.Entities.Nodes;
using DataBrowser.Domain.Entities.SeedWork;
using DataBrowser.Domain.Entities.TransatableItems;
using DataBrowser.Domain.Entities.Users;
using DataBrowser.Domain.Entities.ViewTemplates;
using DataBrowser.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DataBrowser.Entities.SQLite
{
    public class DatabaseContext : BaseDbContext, IUnitOfWork
    {
        private readonly IMediator _mediator;
        private IDbContextTransaction _currentTransaction;

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DatabaseContext(DbContextOptions<DatabaseContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public bool HasActiveTransaction => _currentTransaction != null;

        public DbSet<Hub> Hubs { get; set; }
        public DbSet<Node> Nodes { get; set; }
        public DbSet<ViewTemplate> ViewTemplates { get; set; }
        public DbSet<TransatableItem> TransatableItem { get; set; }
        public DbSet<UserAudit> UserAuditEvents { get; set; }
        public DbSet<Dashboard> Dashboards { get; set; }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default,
            bool dispatchDomainEvent = true)
        {
            var entities = ChangeTracker.Entries<Entity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
                .ToList();

            if (dispatchDomainEvent) await _mediator.DispatchDomainEventsAsync(entities);

            var result = await base.SaveChangesAsync(cancellationToken);

            if (dispatchDomainEvent) await _mediator.DispatchPublicDomainEventsAsync(entities);

            return result;
        }

        public IDbContextTransaction GetCurrentTransaction()
        {
            return _currentTransaction;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //WARNING this code load all configure, with multiple context need to take only DatabaseContext config class
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }


        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null) return null;

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction, bool dispatchDomainEvent = true)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction)
                throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync(dispatchDomainEvent: dispatchDomainEvent);
                await transaction.CommitAsync();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
    }
}