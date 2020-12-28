using DataBrowser.Interfaces.Configuration;
using DataBrowser.Interfaces.Workers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WSHUB.HostedService.Workers
{
    public class SchedulerHostedService : BackgroundService, IDisposable
    {
        private readonly ILogger<SchedulerHostedService> _logger;

        public SchedulerHostedService(ILogger<SchedulerHostedService> logger,
            IServiceProvider services)
        {
            Services = services;
            _logger = logger;
        }

        public IServiceProvider Services { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            SchedulerConfig workerConfig = null;
            using (var scope = Services.CreateScope())
            {
                workerConfig = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<SchedulerConfig>>()?.Value;
            }
            if (workerConfig == null)
            {
                _logger.LogInformation($"Scheduler not configurated");
                return;
            }

            if (!workerConfig.IsEnable)
            {
                return;
                _logger.LogDebug($"Scheduler not enable");
            }

            _logger.LogDebug($"Scheduler start every {workerConfig.Timer}");
            IDashboardCacheGeneratorWorker dashboardCacheGeneratorWorker;
            IDataflowCacheGeneratorWorker dataflowCacheGeneratorWorker;
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = Services.CreateScope())
                {
                    workerConfig = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<SchedulerConfig>>()?.Value;
                    dashboardCacheGeneratorWorker = scope.ServiceProvider.GetRequiredService<IDashboardCacheGeneratorWorker>();
                    dataflowCacheGeneratorWorker = scope.ServiceProvider.GetRequiredService<IDataflowCacheGeneratorWorker>();
                }
                if (workerConfig == null)
                {
                    _logger.LogDebug($"Scheduler not configurated");
                    return;
                }

                if (!workerConfig.IsEnable)
                {
                    _logger.LogDebug($"Scheduler not enable");
                    await Task.Delay(workerConfig.Timer, stoppingToken);
                    continue;
                }

                dashboardCacheGeneratorWorker.ExecuteAsync(stoppingToken);
                dataflowCacheGeneratorWorker.ExecuteAsync(stoppingToken);

                await Task.Delay(workerConfig.Timer, stoppingToken);
            }

            _logger.LogInformation("Scheduler stop");
        }
    }
}
