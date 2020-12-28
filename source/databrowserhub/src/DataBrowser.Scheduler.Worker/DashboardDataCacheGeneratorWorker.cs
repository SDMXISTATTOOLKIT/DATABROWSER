using DataBrowser.Interfaces.Configuration;
using DataBrowser.Interfaces.Workers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WSHUB.HostedService.Workers
{
    public class DashboardDataCacheGeneratorWorker : IDashboardCacheGeneratorWorker
    {
        private readonly ILogger<DashboardDataCacheGeneratorWorker> _logger;
        private static DateTime lastExecutionTime;
        private DashboardDataCacheGeneratorWorkerConfig _workerConfig;

        public DashboardDataCacheGeneratorWorker(ILogger<DashboardDataCacheGeneratorWorker> logger,
            IServiceProvider services)
        {
            Services = services;
            _logger = logger;
        }

        public IServiceProvider Services { get; }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                var currentDate = DateTime.Now;
                // allow only a certain number of concurrent work. In this case, 
                // only allow one job to run at a time. 
                if (State.numberOfActiveJobs < State.maxNumberOfActiveJobs)
                {
                    _logger.LogInformation("Timed DashboardDataCacheGeneratorWorker Service running.");

                    // Update number of running jobs in one atomic operation. 
                    Interlocked.Increment(ref State.numberOfActiveJobs);

                    try
                    {
                        DashboardDataCacheGeneratorWorkerConfig dashboardDataCacheGeneratorWorkerConfig = null;
                        SchedulerConfig schedulerConfig = null;
                        using (var scope = Services.CreateScope())
                        {
                            dashboardDataCacheGeneratorWorkerConfig = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<DashboardDataCacheGeneratorWorkerConfig>>()?.Value;
                            schedulerConfig = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<SchedulerConfig>>()?.Value;
                        }

                        if (dashboardDataCacheGeneratorWorkerConfig.StartTime == null)
                        {
                            _logger.LogDebug("DashboardDataCacheGeneratorWorker null config.");
                            return;
                        }

                        if (!dashboardDataCacheGeneratorWorkerConfig.IsEnable)
                        {
                            _logger.LogDebug("DashboardDataCacheGeneratorWorker not enable");
                            return;
                        }

                        var minRange = currentDate.AddTicks(-schedulerConfig.Timer.Ticks);
                        var maxRange = currentDate.AddSeconds(15);
                        if (dashboardDataCacheGeneratorWorkerConfig.StartTime < minRange ||
                            dashboardDataCacheGeneratorWorkerConfig.StartTime > maxRange ||
                            !dashboardDataCacheGeneratorWorkerConfig.Days.Contains((int)currentDate.DayOfWeek))
                        {
                            _logger.LogDebug($"DashboardDataCacheGeneratorWorker out of time minRange:{minRange}\tStartDate:{dashboardDataCacheGeneratorWorkerConfig.StartTime}\tmaxRange:{maxRange}\tDay:{(int)currentDate.DayOfWeek}");
                            return;
                        }
                        _logger.LogDebug($"DashboardDataCacheGeneratorWorker minRange:{minRange}\tStartDate:{dashboardDataCacheGeneratorWorkerConfig.StartTime}\tmaxRange:{maxRange}\tDay:{(int)currentDate.DayOfWeek}");
                        
                        if (lastExecutionTime.AddHours(10) >= currentDate)
                        {
                            _logger.LogDebug("DashboardDataCacheGeneratorWorker exit for prevent multischeduler in the same range");
                            return;
                        }

                        lastExecutionTime = currentDate;

                        if (cancellationToken.IsCancellationRequested)
                        {
                            _logger.LogError("Received cancellation request DashboardDataCacheGeneratorWorker before starting timer.");

                            cancellationToken.ThrowIfCancellationRequested();
                        }

                        await DoWork();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "DataflowDataCacheGeneratorWorker have error");
                    }
                    finally
                    {
                        Interlocked.Decrement(ref State.numberOfActiveJobs);
                    }
                }
                else
                {
                    _logger.LogDebug(
                        "Job DashboardDataCacheGeneratorWorker skipped since max number of active processes reached.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in DataflowDataCacheGeneratorWorker", ex);
            }
        }

        private async Task DoWork()
        {
            _logger.LogDebug("Timed DashboardDataCacheGeneratorWorker run.");

            // Update number of running jobs in one atomic operation. 
            try
            {


                await singleThreadAsync();

                _logger.LogDebug("Timed DashboardDataCacheGeneratorWorker END.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DashboardDataCacheGeneratorWorker have error");
            }
            finally
            {
                Interlocked.Decrement(ref State.numberOfActiveJobs);
            }
        }

        private async Task singleThreadAsync()
        {
            using (var scope = Services.CreateScope())
            {
                var dashboardDataCacheGenerator =
                    scope.ServiceProvider.GetRequiredService<IDashboardDataCacheGenerator>();

                await dashboardDataCacheGenerator.RefreshAllViewStaticCrossNodeAsync();
            }
        }

        private struct State
        {
            public static int numberOfActiveJobs;
            public const int maxNumberOfActiveJobs = 1;
        }
    }
}