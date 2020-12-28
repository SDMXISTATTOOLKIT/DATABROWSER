using System;
using System.Threading;
using System.Threading.Tasks;
using DataBrowser.DB.EFCore.Context;
using DataBrowser.Domain.Entities.Hubs;
using DataBrowser.Domain.Entities.Users;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Entities.SQLite;
using DataBrowser.Interfaces.Configuration;
using DataBrowser.Interfaces.Updater;
using DataBrowser.Updater.Actions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WSHUB.HostedService.Workers
{
    public class MigratorDBHostedService : IHostedService
    {
        private readonly ILogger<MigratorDBHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public MigratorDBHostedService(ILogger<MigratorDBHostedService> logger,
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Migration DB Hosted Service running.");

            using (var scope = _serviceProvider.CreateScope())
            {
                var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<MigratorDBHostedService>();
                var databaseConfig = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<DatabaseConfig>>();
                var updater = scope.ServiceProvider.GetRequiredService<IUpdater>();

                if (databaseConfig.Value.UseMigrationScript)
                {
                    logger.LogDebug("Run Migrate DataBrowserUpdaterContext");
                    var myDbContext = scope.ServiceProvider.GetRequiredService<DataBrowserUpdaterContext>();
                    await myDbContext.Database.MigrateAsync();
                }

                _logger.LogInformation("Run upgrader before database");
                await updater.ExecutionBeforeDatabaseUpgrade();

                if (databaseConfig.Value.UseMigrationScript)
                {
                    logger.LogDebug("Run Migrate DatabaseContext");
                    var myDbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                    await myDbContext.Database.MigrateAsync();
                }

                try
                {
                    logger.LogDebug("Seed Database Data");
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                    var hubRepository = scope.ServiceProvider.GetRequiredService<IRepository<Hub>>();
                    await ApplicationDbContextSeed.SeedEssentialsAsync(userManager, roleManager, hubRepository);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }

                _logger.LogInformation("Run upgrader after database");
                await updater.ExecutionAfterDatabaseUpgrade();
            }

            _logger.LogDebug("Migration DB END.");
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}