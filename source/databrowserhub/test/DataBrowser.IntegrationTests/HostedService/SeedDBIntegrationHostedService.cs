using System;
using System.Threading;
using System.Threading.Tasks;
using DataBrowser.IntegrationTests.HelperTest;
using DataBrowser.Interfaces.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DataBrowser.IntegrationTests.HostedService
{
    public class SeedDBIntegrationHostedService : IHostedService
    {
        private readonly IOptionsSnapshot<DatabaseConfig> _databaseConfig;
        private readonly ILogger<SeedDBIntegrationHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public SeedDBIntegrationHostedService(ILogger<SeedDBIntegrationHostedService> logger,
            IServiceProvider serviceProvider,
            IOptionsSnapshot<DatabaseConfig> databaseConfig)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _databaseConfig = databaseConfig;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Seed DB Integration Hosted Service running.");

            await UtilityTest.SeedDatabase(_serviceProvider);
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}