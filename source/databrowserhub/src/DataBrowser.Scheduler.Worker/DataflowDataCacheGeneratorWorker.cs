using AutoMapper;
using DataBrowser.AC.Utility;
using DataBrowser.Domain.Entities.Hubs;
using DataBrowser.Domain.Entities.Nodes;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Configuration;
using DataBrowser.Interfaces.Workers;
using DataBrowser.Specifications.Nodes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WSHUB.HostedService.Workers
{
    public class DataflowDataCacheGeneratorWorker : IDataflowCacheGeneratorWorker
    {
        private readonly ILogger<DataflowDataCacheGeneratorWorker> _logger;
        private static DateTime lastExecutionTime;

        public DataflowDataCacheGeneratorWorker(ILogger<DataflowDataCacheGeneratorWorker> logger,
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
                    _logger.LogDebug("Timed DataflowDataCacheGeneratorWorker Service running.");

                    // Update number of running jobs in one atomic operation. 
                    Interlocked.Increment(ref State.numberOfActiveJobs);

                    try
                    {
                        DataflowDataCacheGeneratorWorkerConfig dataflowDataCacheGeneratorWorkerConfig = null;
                        SchedulerConfig schedulerConfig = null;
                        using (var scope = Services.CreateScope())
                        {
                            dataflowDataCacheGeneratorWorkerConfig = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<DataflowDataCacheGeneratorWorkerConfig>>()?.Value;
                            schedulerConfig = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<SchedulerConfig>>()?.Value;
                        }

                        if (dataflowDataCacheGeneratorWorkerConfig.StartTime == null)
                        {
                            _logger.LogDebug("DataflowDataCacheGeneratorWorker null config.");
                            return;
                        }

                        if (!dataflowDataCacheGeneratorWorkerConfig.IsEnable)
                        {
                            _logger.LogDebug("DataflowDataCacheGeneratorWorker not enable");
                            return;
                        }

                        var minRange = currentDate.AddTicks(-schedulerConfig.Timer.Ticks);
                        var maxRange = currentDate.AddSeconds(15);
                        if (dataflowDataCacheGeneratorWorkerConfig.StartTime < minRange ||
                            dataflowDataCacheGeneratorWorkerConfig.StartTime > maxRange ||
                            !dataflowDataCacheGeneratorWorkerConfig.Days.Contains((int)currentDate.DayOfWeek))
                        {
                            _logger.LogDebug($"DataflowDataCacheGeneratorWorker out of time minRange:{minRange}\tStartDate:{dataflowDataCacheGeneratorWorkerConfig.StartTime}\tmaxRange:{maxRange}\tDay:{(int)currentDate.DayOfWeek}");
                            return;
                        }
                        _logger.LogDebug($"DataflowDataCacheGeneratorWorker minRange:{minRange}\tStartDate:{dataflowDataCacheGeneratorWorkerConfig.StartTime}\tmaxRange:{maxRange}\tDay:{(int)currentDate.DayOfWeek}");

                        //if (lastExecutionTime.AddMinutes(5) >= currentDate)
                        //{
                        //    _logger.LogDebug("DataflowDataCacheGeneratorWorker exit for prevent multischeduler in the same range");
                        //    return;
                        //}

                        lastExecutionTime = currentDate;

                        if (cancellationToken.IsCancellationRequested)
                        {
                            _logger.LogError("Received cancellation request DataflowDataCacheGeneratorWorker before starting timer.");

                            cancellationToken.ThrowIfCancellationRequested();
                        }

                        //dataflowDataCacheGeneratorWorkerConfig.EnableMultiGenerator
                        if (true)
                        {
                            await multiThreadAsync();
                        }
                        else
                        {
                            await singleThreadAsync();
                        }

                        _logger.LogDebug("Timed DataflowDataCacheGeneratorWorker END.");
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
                        "Job DataflowDataCacheGeneratorWorker skipped since max number of active processes reached.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in DataflowDataCacheGeneratorWorker", ex);
            }
        }

        private async Task singleThreadAsync()
        {
            using (var scope = Services.CreateScope())
            {
                var dataflowDataCacheGenerator =
                    scope.ServiceProvider.GetRequiredService<IDataflowDataCacheGenerator>();

                await dataflowDataCacheGenerator.RefreshAllDataflowCrossNodeAsync();
            }
        }

        private async Task multiThreadAsync()
        {
            IEnumerable<IGrouping<string, DataflowDataCacheGeneratorWorkerConfig.Dataflowsrefresh>> grouped = null;
            var nodesMapper = new Dictionary<string, int>();
            using (var scope = Services.CreateScope())
            {
                var repositoryNode = scope.ServiceProvider.GetRequiredService<IRepository<Node>>();
                var dataflowDataCacheGenerator =
                    scope.ServiceProvider.GetRequiredService<IDataflowDataCacheGenerator>();
                var dataflowRefresh = dataflowDataCacheGenerator.GetDataflowRefresh();

                grouped = dataflowRefresh?.GroupBy(i => i.NodeCode);
                if (grouped != null)
                {
                    foreach (var itemNode in grouped)
                    {
                        var nodeId = await repositoryNode.FindAsync(
                            new NodeByCodeSpecification(itemNode.Key, NodeByCodeSpecification.ExtraInclude.Nothing));
                        if (nodeId != null &&
                            nodeId.Any())
                        {
                            nodesMapper.Add(itemNode.Key, nodeId[0].NodeId);
                        }
                        else
                        {
                            nodesMapper.Add(itemNode.Key, -1);
                        }
                    }
                }
            }

            if (grouped != null &&
                grouped.Any())
            {
                var tasks = grouped.Select(i => executeDataflowRefreshByNodeCodeAsync(nodesMapper[i.Key], i.Key));
                await Task.WhenAll(tasks);
            }
            _logger.LogDebug("MultiThreadAsync");
        }

        private async Task executeDataflowRefreshByNodeCodeAsync(int nodeId, string nodeCode)
        {
            var langs = new List<string>();
            using (var scope = Services.CreateScope())
            {
                _logger.LogDebug($"Create context for nodeId:{nodeId}\tnodeCode:{nodeCode}");
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                var hubRep = scope.ServiceProvider.GetRequiredService<IRepository<Hub>>();
                var requestContext = scope.ServiceProvider.GetRequiredService<IRequestContext>();
                requestContext.OverwriteNodeId(nodeId);
                requestContext.OverwriteNodeCode(nodeCode);

                var hub = await hubRep.GetByIdAsync(1);
                if (hub == null)
                {
                    return;
                }

                var hubDto = hub.ConvertToHubDto(mapper);
                langs = hubDto.SupportedLanguages;
            }

            _logger.LogDebug(
                $"Create multitask callRefreshAllDataflowForCurrentNodeAndLangAsync for nodeId:{nodeId}\tnodeCode:{nodeCode}");
            var tasks = langs.Select(i => callRefreshAllDataflowForCurrentNodeAndLangAsync(nodeId, nodeCode, i));
            await Task.WhenAll(tasks);
        }

        private async Task callRefreshAllDataflowForCurrentNodeAndLangAsync(int nodeId, string nodeCode, string lang)
        {
            using (var scope = Services.CreateScope())
            {
                _logger.LogDebug($"Create context for nodeId:{nodeId}\tnodeCode:{nodeCode}\tlang{lang}");
                var requestContext = scope.ServiceProvider.GetRequiredService<IRequestContext>();
                requestContext.OverwriteNodeId(nodeId);
                requestContext.OverwriteNodeCode(nodeCode);
                requestContext.OverwriteUserLang(lang);

                var dataflowDataCacheGenerator =
                    scope.ServiceProvider.GetRequiredService<IDataflowDataCacheGenerator>();
                await dataflowDataCacheGenerator.RefreshAllDataflowAsync();

                _logger.LogDebug($"End context for nodeId:{nodeId}\tnodeCode:{nodeCode}\tlang{lang}");
            }
        }

        private struct State
        {
            public static int numberOfActiveJobs;
            public const int maxNumberOfActiveJobs = 1;
        }
    }
}