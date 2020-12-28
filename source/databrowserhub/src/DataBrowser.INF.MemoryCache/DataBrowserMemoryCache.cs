using AutoMapper;
using DataBrowser.AC.Utility;
using DataBrowser.Domain.Entities.Hubs;
using DataBrowser.Domain.Entities.Nodes;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.DomainServices.Interfaces;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Cache;
using DataBrowser.Interfaces.Cache.Key;
using DataBrowser.Interfaces.Configuration;
using DataBrowser.Services.Interfaces;
using EndPointConnector.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataBrowser.INF.MemoryCache
{
    public class DataBrowserMemoryCache : IDataBrowserMemoryCache
    {
        private readonly IEndPointConnectorFactory _endPointConnectorFactory;
        private readonly ILogger<DataBrowserMemoryCache> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IMediatorService _mediatorService;
        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheConfig _memoryCacheConfig;
        private readonly INodeConfigService _nodeConfigService;
        private readonly IRepository<Node> _repository;
        private readonly IRequestContext _requestContext;
        private readonly IRepository<Hub> _repositoryHub;
        private readonly IMapper _mapper;

        public DataBrowserMemoryCache(ILogger<DataBrowserMemoryCache> logger,
            IMemoryCache memoryCache,
            IOptionsMonitor<MemoryCacheConfig> memoryCacheConfig,
            IRequestContext requestContext,
            IRepository<Node> repository,
            IRepository<Hub> repositoryHub,
            IEndPointConnectorFactory endPointConnectorFactory,
            IMediatorService mediatorService,
            INodeConfigService nodeConfigService,
            ILoggerFactory loggerFactory,
            IMapper mapper)
        {
            _memoryCache = memoryCache;
            _logger = logger;
            _memoryCacheConfig = memoryCacheConfig.CurrentValue;
            _requestContext = requestContext;
            _repository = repository;
            _repositoryHub = repositoryHub;
            if (_memoryCacheConfig == null)
            {
                _memoryCacheConfig = new MemoryCacheConfig
                { ExpirationKeys = new Dictionary<string, string> { { "DefaultTimeSpan", "02:00:00" } } };
            }

            _endPointConnectorFactory = endPointConnectorFactory;
            _mediatorService = mediatorService;
            _nodeConfigService = nodeConfigService;
            _loggerFactory = loggerFactory;
            _mapper = mapper;
        }

        public async Task AddAsync<TItem>(TItem item, ICacheKey<TItem> key)
        {
            var cachedObjectName = key.GetType().Name;
            var timespan = await calcolateTimeSpanLife(cachedObjectName);

            _memoryCache.Set(key.CacheKey, item, timespan);

            var keys = _memoryCache.GetOrCreate<List<string>>(ConstraintKey.KeyList, (entry) => new List<string> { key.CacheKey });

            if (!keys.Contains(key.CacheKey))
            {
                keys.Add(key.CacheKey);
            }
            _memoryCache.Set(ConstraintKey.KeyList, keys);
        }

        public TItem Get<TItem>(ICacheKey<TItem> key) where TItem : class
        {
            if (_memoryCache.TryGetValue(key.CacheKey, out TItem value))
            {
                return value;
            }

            return null;
        }

        public async Task GenerateDataflowDsdCodelistConceptschemeCacheAsync(IServiceProvider serviceProvider, IRequestContext requestContext, bool forceIfExist = true)
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var dataflowDsdCacheGenerator =
                        scope.ServiceProvider.GetRequiredService<IDataBrowserMemoryCache>();
                    var nodeConfigServiceSpecificScope =
                        scope.ServiceProvider.GetRequiredService<INodeConfigService>();
                    var endPointConnectorFactorySpecificScope =
                        scope.ServiceProvider.GetRequiredService<IEndPointConnectorFactory>();
                    var loggerFactorySpecificScope =
                        scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
                    var requestContextSpecificScope =
                        scope.ServiceProvider.GetRequiredService<IRequestContext>();
                    var logSpecificContext = loggerFactorySpecificScope.CreateLogger("GenerateDataflowDsdCodelistConceptschemeCacheAsync");
                    var dataBrowserMemoryCacheSpecificScope =
                        scope.ServiceProvider.GetRequiredService<IDataBrowserMemoryCache>();


                    requestContextSpecificScope.OverwriteNodeId(requestContext.NodeId);
                    requestContextSpecificScope.OverwriteNodeCode(requestContext.NodeCode);
                    requestContextSpecificScope.OverwriteUserLang(requestContext.UserLang);
                    requestContextSpecificScope.OverwriteLoggedUserId(requestContext.LoggedUserId);
                    requestContextSpecificScope.OverwriteLoggedUser(requestContext.LoggedUser);
                    logSpecificContext.LogDebug($"START GenerateDataflowAndDsdCacheAsync node {requestContextSpecificScope.NodeId}");
                    var key = new DataflowWithDsdCacheKey(requestContextSpecificScope.NodeId);
                    var cachedData = Get(key);
                    if (cachedData != null && !forceIfExist)
                    {
                        logSpecificContext.LogDebug($"cache already in cache and forceIfExist is {forceIfExist}");
                        return;
                    }

                    logSpecificContext.LogDebug("begin to create dataflowwithdsd cache");
                    var nodeConfig = await nodeConfigServiceSpecificScope.GenerateNodeConfigAsync(requestContextSpecificScope.NodeId);
                    var endPointConnector = await endPointConnectorFactorySpecificScope.Create(nodeConfig, requestContextSpecificScope);
                    endPointConnector.TryUseCache = false;

                    logSpecificContext.LogDebug("call GetOnlyDataflowsValidForCatalogWithDsdAsync");
                    var container = await endPointConnector.GetOnlyDataflowsValidForCatalogWithDsdCodelistAsync();
                    logSpecificContext.LogDebug("calculate data for virtual dataflow");
                    container = await VirtualDataflowUtility.FindVirtualDataflowAndRecreateAllDataAsync(container,
                        requestContextSpecificScope,
                        endPointConnectorFactorySpecificScope,
                        nodeConfigServiceSpecificScope,
                        nodeConfig,
                        loggerFactorySpecificScope);

                    logSpecificContext.LogDebug("add key in cache");
                    await dataBrowserMemoryCacheSpecificScope.AddAsync(container, key);


                    //var nDataflow = container.Dataflows.Count;
                    //logSpecificContext.LogDebug($"Start to add cache for dataflows items {nDataflow}");
                    //var iElement = 1;
                    //foreach (var item in container.Dataflows)
                    //{
                    //    try
                    //    {
                    //        var dataflowWithData = await endPointConnector.GetDataflowWithAllUsedDataAsync(item);
                    //        if (dataflowWithData == null)
                    //        {
                    //            continue;
                    //        }

                    //        await dataBrowserMemoryCacheSpecificScope.AddAsync(dataflowWithData, new DataflowWithDataCacheKey(requestContextSpecificScope.NodeId, item.Id));
                    //        logSpecificContext.LogDebug($"Cached dataflow {iElement} of {nDataflow} id: {item.Id}");
                    //        iElement++;
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        logSpecificContext.LogError($"Some error in dataflow {item.Id} when try to get data for cache", ex);
                    //    }
                    //}
                    logSpecificContext.LogDebug($"END GenerateDataflowAndDsdCacheAsync node {requestContextSpecificScope.NodeId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error in GenerateDataflowAndDsdCacheAsync");
            }
        }

        private async Task<TimeSpan> calcolateTimeSpanLife(string cachedObjectName)
        {
            var timespan = _memoryCacheConfig.ExpirationKeys.ContainsKey(cachedObjectName)
                ? TimeSpan.Parse(_memoryCacheConfig.ExpirationKeys[cachedObjectName])
                : TimeSpan.Parse(_memoryCacheConfig.ExpirationKeys["DefaultTimeSpan"]);

            if (cachedObjectName.Equals(nameof(CatalogTreeCacheKey), StringComparison.InvariantCultureIgnoreCase) ||
                cachedObjectName.Equals(nameof(DataflowWithDsdCacheKey), StringComparison.InvariantCultureIgnoreCase) ||
                cachedObjectName.Equals(nameof(DsdWithCodelistlistStubCacheKey), StringComparison.InvariantCultureIgnoreCase) ||
                cachedObjectName.Equals(nameof(DataflowWithDataCacheKey), StringComparison.InvariantCultureIgnoreCase))
            {
                var node = await _repository.GetByIdAsync(_requestContext.NodeId);
                if (node?.TtlCatalog != null)
                {
                    timespan = TimeSpan.FromSeconds((double)node.TtlCatalog);
                }
            }

            return timespan;
        }

        public async Task ClearCacheAsync(List<int> nodesId = null, List<string> applicationLangs = null, List<int> userIds = null, string key = null)
        {
            _logger.LogDebug("START ClearCacheAsync");

            if (nodesId == null &&
                applicationLangs == null &&
                userIds == null &&
                key == null)
            {
                _memoryCache.TryGetValue(ConstraintKey.KeyList, out List<string> memoryKeys);
                if (memoryKeys == null)
                {
                    memoryKeys = new List<string>();
                }

                foreach (var itemKey in memoryKeys)
                {
                    _memoryCache.Remove(itemKey);
                }
            }

            if (nodesId == null)
            {
                return;
            }


            _memoryCache.TryGetValue(ConstraintKey.KeyList, out List<string> memoryAllKeys);
            if (memoryAllKeys == null)
            {
                memoryAllKeys = new List<string>();
            }


            if ((applicationLangs == null || applicationLangs.Count == 0 || applicationLangs.Contains(ConstraintKey.AllLanguages)) &&
                (userIds == null || userIds.Count == 0 || userIds.Contains(ConstraintKey.AllUsers)) &&
                string.IsNullOrWhiteSpace(key))
            {
                clearAllCacheForNode(memoryAllKeys, nodesId);
                _logger.LogDebug("END ClearCacheAsync for single nodes");
                return;
            }


            //TODO we can manage specific clear cache (never used for now)
            
            if (userIds == null || userIds.Count == 0 || userIds.Contains(ConstraintKey.AllUsers))
            {
                userIds = new List<int> { ConstraintKey.AllUsers };
            }

            if (applicationLangs == null)
            {
                var hub = await _repositoryHub.GetByIdAsync(1);
                applicationLangs = hub.ConvertToHubDto(_mapper).SupportedLanguages;
                applicationLangs.Add(ConstraintKey.AllLanguages);
            }

            var removedKeys = new List<string>();
            foreach (var node in nodesId)
            {
                foreach (var lang in applicationLangs)
                {
                    foreach (var userId in userIds)
                    {
                        string keyRemove = null;
                        try
                        {
                            keyRemove = $"{key ?? ""}:Node{node}:Lang{lang}:User{userId}";
                            foreach (var itemKey in memoryAllKeys.Where(i => i.Contains(keyRemove)))
                            {
                                _memoryCache.Remove(itemKey);
                                removedKeys.Add(itemKey);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error during remove key: {keyRemove ?? ""}", ex);
                        }
                    }
                }
            }

            foreach (var item in removedKeys)
            {
                memoryAllKeys.Remove(item);
            }
            _memoryCache.Set(ConstraintKey.KeyList, memoryAllKeys);

            _logger.LogDebug("END ClearCacheAsync");

        }

        public async Task ClearNodeCacheAsync(int nodeId)
        {
            await ClearCacheAsync(new List<int> { nodeId });
        }

        private void clearAllCacheForNode(List<string> memoryAllKeys, List<int> nodesId)
        {
            var removedKeys = new List<string>();

            foreach (var itemNode in nodesId)
            {
                string keyRemove = null;
                try
                {
                    keyRemove = $":Node{itemNode}:";
                    foreach (var itemKey in memoryAllKeys.Where(i => i.Contains(keyRemove, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        _memoryCache.Remove(itemKey);
                        removedKeys.Add(itemKey);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error during remove key: {keyRemove ?? ""}", ex);
                }
            }

            foreach (var item in removedKeys)
            {
                memoryAllKeys.Remove(item);
            }
            _memoryCache.Set(ConstraintKey.KeyList, memoryAllKeys);
        }

        //private List<string> getAllPropertyKeyFromCache()
        //{
        //    var prop = _memoryCache.GetType().GetProperty("EntriesCollection",
        //            BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public);
        //    var innerCache = prop.GetValue(_memoryCache);
        //    innerCache.Select();
        //}

    }
}