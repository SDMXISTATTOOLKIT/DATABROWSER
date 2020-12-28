using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DataBrowser.Entities.SQLite;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DataBrowser.DB.EFCore
{
    public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();

            var dbType = configuration["Database:DbType"];
            if (dbType != null && dbType.Equals("SQLite", StringComparison.InvariantCultureIgnoreCase))
                optionsBuilder.UseSqlite(configuration["Database:ConnectionString"]);
            else if (dbType != null && dbType.Equals("SqlServer", StringComparison.InvariantCultureIgnoreCase))
                optionsBuilder.UseSqlServer(configuration["Database:ConnectionString"]);

            return new DatabaseContext(optionsBuilder.Options, new NoMediator());
        }

        private class NoMediator : IMediator
        {
            public Task Publish<TNotification>(TNotification notification,
                CancellationToken cancellationToken = default) where TNotification : INotification
            {
                return Task.CompletedTask;
            }

            public Task Publish(object notification, CancellationToken cancellationToken = default)
            {
                return Task.CompletedTask;
            }

            public Task<TResponse> Send<TResponse>(IRequest<TResponse> request,
                CancellationToken cancellationToken = default)
            {
                return Task.FromResult(default(TResponse));
            }

            public Task<object> Send(object request, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }
        }
    }
}