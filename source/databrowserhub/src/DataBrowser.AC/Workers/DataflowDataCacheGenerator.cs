using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataBrowser.Domain.Entities.Nodes;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Cache;
using DataBrowser.Interfaces.Configuration;
using DataBrowser.Interfaces.Dto.UseCases.Requests;
using DataBrowser.Interfaces.Dto.UseCases.Responses;
using DataBrowser.Interfaces.Workers;
using DataBrowser.Services.Interfaces;
using DataBrowser.Specifications.Nodes;
using EndPointConnector.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DataBrowser.AC.Workers
{
    public class DataflowDataCacheGenerator : IDataflowDataCacheGenerator
    {
        private readonly IDataflowDataCache _dataflowDataCache;
        private readonly DataflowDataCacheGeneratorWorkerConfig _dataflowDataCacheGeneratorWorkerConfig; 
        private readonly ILogger<DataflowDataCacheGenerator> _logger;
        private readonly IMediatorService _mediatorService;
        private readonly IRepository<Node> _repository;
        private readonly IRequestContext _requestContext;

        public DataflowDataCacheGenerator(IDataflowDataCache dataflowDataCache,
            IOptionsSnapshot<DataflowDataCacheGeneratorWorkerConfig> dataflowDataCacheGeneratorWorkerConfig,
            IRepository<Node> repository,
            IRequestContext requestContext,
            IMediatorService mediatorService,
            ILogger<DataflowDataCacheGenerator> logger)
        {
            _dataflowDataCache = dataflowDataCache;
            _dataflowDataCacheGeneratorWorkerConfig = dataflowDataCacheGeneratorWorkerConfig.Value;
            _repository = repository;
            _requestContext = requestContext;
            _mediatorService = mediatorService;
            _logger = logger;
        }

        public async Task RefreshAllDataflowCrossNodeAsync()
        {
            _logger.LogInformation("Call GenerateAsync");

            if (_dataflowDataCacheGeneratorWorkerConfig?.DataflowsRefresh == null) return;

            _requestContext.OverwriteIgnoreCache(false);

            foreach (var dataflow in _dataflowDataCacheGeneratorWorkerConfig.DataflowsRefresh)
            {
                _logger.LogInformation($"start to refresh dataflow {dataflow.Id}");

                var node = await _repository.FindAsync(new NodeByCodeSpecification(dataflow.NodeCode,
                    NodeByCodeSpecification.ExtraInclude.Nothing));
                if (node == null || node.Count <= 0)
                {
                    _logger.LogInformation($"dataflow {dataflow.Id} with unfound node code {dataflow.NodeCode}");
                    continue;
                }

                _logger.LogDebug("Overwrite RequestContext");
                _requestContext.OverwriteNodeId(node.First().NodeId);
                _requestContext.OverwriteNodeCode(dataflow.NodeCode);

                await commonGenerateRefreshCacheAsync(dataflow);

                _logger.LogInformation($"end to refresh dataflow {dataflow.Id}");
            }

            _requestContext.OverwriteIgnoreCache(_requestContext.IgnoreCacheFromCurrentContext);
            _requestContext.OverwriteNodeId(_requestContext.NodeIdFromCurrentContext);
            _requestContext.OverwriteNodeCode(_requestContext.NodeCode);

            _logger.LogInformation("END GenerateAsync");
        }

        public async Task RefreshSingleDataflowAsync(string id, string nodeCode)
        {
            var dataflow = _dataflowDataCacheGeneratorWorkerConfig.DataflowsRefresh.FirstOrDefault(i =>
                i.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase) &&
                i.NodeCode.Equals(nodeCode, StringComparison.InvariantCultureIgnoreCase));


            _logger.LogInformation($"start to refresh dataflow {dataflow.Id}");

            var node = await _repository.FindAsync(new NodeByCodeSpecification(dataflow.NodeCode,
                NodeByCodeSpecification.ExtraInclude.Nothing));
            if (node == null || node.Count <= 0)
            {
                _logger.LogInformation($"dataflow {dataflow.Id} with unfound node code {nodeCode}");
                return;
            }

            if (node[0].NodeId != _requestContext.NodeId)
            {
                _logger.LogInformation(
                    $"Request nodeId {_requestContext.NodeId} is different to nodeId of {dataflow.NodeCode}");
                return;
            }

            _logger.LogDebug("Overwrite RequestContext");

            await commonGenerateRefreshCacheAsync(dataflow);

            _logger.LogInformation($"end to refresh dataflow {dataflow.Id}");
        }

        public async Task RefreshAllDataflowAsync()
        {
            _logger.LogDebug($"start to refresh all nodeId:{_requestContext.NodeId}\tLang:{_requestContext.UserLang}");

            var node = await _repository.GetByIdAsync(_requestContext.NodeId);
            if (node == null)
            {
                _logger.LogInformation($"unable to find nodeId {node.NodeId}");
                return;
            }

            foreach (var dataflowConfig in _dataflowDataCacheGeneratorWorkerConfig.DataflowsRefresh.Where(i =>
                i.NodeCode != null && i.NodeCode.Equals(node.Code, StringComparison.InvariantCultureIgnoreCase)))
            {
                _logger.LogInformation(
                    $"start to refresh dataflow {dataflowConfig.Id}\tnodeId:{node.NodeId}\tnodeCode:{node.Code}\tLang:{_requestContext.UserLang}");

                var dataflow = _dataflowDataCacheGeneratorWorkerConfig.DataflowsRefresh.FirstOrDefault(i =>
                    i.NodeCode.Equals(node.Code, StringComparison.InvariantCultureIgnoreCase) &&
                    i.Id.Equals(dataflowConfig.Id, StringComparison.InvariantCultureIgnoreCase));

                if (dataflow == null)
                {
                    _logger.LogInformation(
                        $"dataflow {dataflow.Id} have different node code from nodeId {_requestContext.NodeId} [{node.Code}] ");
                    return;
                }

                await commonGenerateRefreshCacheAsync(dataflow);

                _logger.LogInformation(
                    $"end to refresh dataflow {dataflow.Id}\tnodeId:{node.NodeId}\tnodeCode:{node.Code}\tLang:{_requestContext.UserLang}");
            }
        }

        public List<DataflowDataCacheGeneratorWorkerConfig.Dataflowsrefresh> GetDataflowRefresh()
        {
            return _dataflowDataCacheGeneratorWorkerConfig.DataflowsRefresh;
        }

        public Task RefreshAllDataflowInStaticDashboardCrossNodeAsync()
        {
            throw new NotImplementedException();
        }


        private async Task commonGenerateRefreshCacheAsync(DataflowDataCacheGeneratorWorkerConfig.Dataflowsrefresh dataflow)
        {
            _logger.LogDebug($"Call partial criteria\tnodeCode:{dataflow.NodeCode}");
            var partialCodelist = new GetCodelistPartialForDataflowRequest
                {DataflowId = dataflow.Id, DimensionIds = dataflow.Dimensions};
            var container = await _mediatorService.Send(partialCodelist);

            await _dataflowDataCache.InvalidateAllKeysForCurrentNodeAndLanguages(dataflow.Id);
            //TODO maybe invalid only cache that containt only one filter with DimensId to recreate?

            _logger.LogDebug($"Call generateCallWithOneDimensionAsync\tnodeCode:{dataflow.NodeCode}");
            await generateCallWithOneDimensionAsync(container, dataflow);
        }

        private async Task generateCallWithOneDimensionAsync(
            GetCodelistPartialForDataflowResponse codelistPartialForDataflow,
            DataflowDataCacheGeneratorWorkerConfig.Dataflowsrefresh dataflowsrefresh)
        {
            if (dataflowsrefresh.ObservationMax < 0) dataflowsrefresh.ObservationMax = 300000;
            _logger.LogDebug(
                $"dataflowsrefresh.ObservationMax {dataflowsrefresh.ObservationMax}\tnodeCode:{dataflowsrefresh.NodeCode}");

            foreach (var dimension in codelistPartialForDataflow.ArtefactContainer.Criterias)
            {
                _logger.LogDebug($"dimension Id {dimension.Id}\tnodeCode:{dataflowsrefresh.NodeCode}");
                var refreshStartDate = DateTime.UtcNow;

                Dictionary<int, FilterCriteria> filtersByGroupFilter = null;
                if (dataflowsrefresh.GruopByNumber != null)
                    filtersByGroupFilter = groupCacheByNumberItemCode(dataflowsrefresh, dimension);
                else
                    filtersByGroupFilter = await groupCacheByMaxObservation(dataflowsrefresh, dimension);

                foreach (var item in filtersByGroupFilter)
                {
                    _logger.LogDebug(
                        $"item number: {item.Key} of {filtersByGroupFilter.Count}\tnodeCode:{dataflowsrefresh.NodeCode}\tLang:{_requestContext.UserLang}");

                    if (item.Value.FilterValues.Count <= 0) continue;

                    var dataCriterias = new List<FilterCriteria> {item.Value};

                    try
                    {
                        if (_logger.IsEnabled(LogLevel.Debug))
                            _logger.LogDebug(
                                $"Call dataflow data for with filter dataCriterias {JsonConvert.SerializeObject(dataCriterias)}\tnodeCode:{dataflowsrefresh.NodeCode}\tLang:{_requestContext.UserLang}");
                        var dataflowRequest = new DataFromDataflowRequest
                        {
                            DataflowId = dataflowsrefresh.Id, DataCriterias = dataCriterias,
                            TryUseCompatibleKeyCache = false
                        };
                        await _mediatorService.Send(dataflowRequest);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            $"Some error during refresh item cache with dataCriterias: {JsonConvert.SerializeObject(dataCriterias)}\tnodeCode:{dataflowsrefresh.NodeCode}\tLang:{_requestContext.UserLang}");
                    }
                }

                _logger.LogDebug(
                    $"refresh old key\tnodeCode:{dataflowsrefresh.NodeCode}\tLang:{_requestContext.UserLang}");
                var keyFiles = await _dataflowDataCache.GetValidCacheFileIdForDataflowId(dataflowsrefresh.Id);
                foreach (var itemFile in keyFiles)
                {
                    if (itemFile.CreationDate >= refreshStartDate) continue;
                    try
                    {
                        if (_logger.IsEnabled(LogLevel.Debug))
                            _logger.LogDebug(
                                $"Call dataflow data for with filter dataCriterias {JsonConvert.SerializeObject(itemFile.Filter)}\tnodeCode:{dataflowsrefresh.NodeCode}");
                        var dataflowRequest = new DataFromDataflowRequest
                            {DataflowId = dataflowsrefresh.Id, DataCriterias = itemFile.Filter};
                        await _mediatorService.Send(dataflowRequest);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            $"Some error during refresh item cache with dataCriterias: {JsonConvert.SerializeObject(itemFile.Filter)}\tnodeCode:{dataflowsrefresh.NodeCode}\tLang:{_requestContext.UserLang}");
                        _logger.LogError(ex,
                            $"Error Refresh\tnodeCode:{dataflowsrefresh.NodeCode}\tLang:{_requestContext.UserLang}");
                    }
                }

                if (dataflowsrefresh.GruopByNumber != null &&
                    dataflowsrefresh.GruopByNumber.GroupSize > 1)
                {
                    _logger.LogDebug(
                        $"splititems group to single key item\tnodeCode:{dataflowsrefresh.NodeCode}\tLang:{_requestContext.UserLang}");
                    foreach (var item in filtersByGroupFilter)
                    {
                        _logger.LogDebug(
                            $"item number: {item.Key} of {filtersByGroupFilter.Count} (split {item.Value.FilterValues.Count})\tnodeCode:{dataflowsrefresh.NodeCode}\tLang:{_requestContext.UserLang}");

                        if (item.Value.FilterValues.Count <= 0) continue;

                        foreach (var singleItem in item.Value.FilterValues)
                        {
                            var dataCriterias = new List<FilterCriteria>
                                {new FilterCriteria {Id = item.Value.Id, FilterValues = new List<string> {singleItem}}};

                            try
                            {
                                if (_logger.IsEnabled(LogLevel.Debug))
                                    _logger.LogDebug(
                                        $"refresh single cache item:{singleItem}\tnodeCode:{dataflowsrefresh.NodeCode}\tnodeCode:{dataflowsrefresh.NodeCode}\tnodeCode:{dataflowsrefresh.NodeCode}");
                                var dataflowRequest = new DataFromDataflowRequest
                                {
                                    DataflowId = dataflowsrefresh.Id, DataCriterias = dataCriterias,
                                    TryUseCompatibleKeyCache = true
                                };
                                await _mediatorService.Send(dataflowRequest);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex,
                                    $"Some error during refresh single cache item:{singleItem}\tnodeCode:{dataflowsrefresh.NodeCode}\tnodeCode:{dataflowsrefresh.NodeCode}\tnodeCode:{dataflowsrefresh.NodeCode}");
                            }
                        }
                    }
                }
            }
        }

        private async Task<Dictionary<int, FilterCriteria>> groupCacheByMaxObservation(
            DataflowDataCacheGeneratorWorkerConfig.Dataflowsrefresh dataflowsrefresh, Criteria dimension)
        {
            var filtersByGroupFilter = new Dictionary<int, FilterCriteria>();
            var filtersCounter = new Dictionary<int, long>();
            var filter = new FilterCriteria {Id = dimension.Id, FilterValues = new List<string>()};

            var currentKeyGroup = createGroupForFilter(filtersByGroupFilter, filtersCounter, filter);

            var obsCount = 0L;
            foreach (var itemCode in dimension.Values)
            {
                _logger.LogDebug($"Code {itemCode.Id}");
                var dataflowObsCount = 0L;
                if (dataflowsrefresh.ObservationMax > 0)
                {
                    var dataflowCount = new CountObservationsFromDataflowRequest
                    {
                        DataflowId = dataflowsrefresh.Id,
                        DataCriterias = new List<FilterCriteria>
                        {
                            new FilterCriteria {Id = dimension.Id, FilterValues = new List<string> {itemCode.Id}}
                        }
                    };
                    try
                    {
                        dataflowObsCount = (await _mediatorService.Send(dataflowCount)).Count;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            $"Some error during try count item Code: {JsonConvert.SerializeObject(itemCode.Id)}");
                        dataflowObsCount = dataflowsrefresh.ObservationMax;
                    }
                }

                if (obsCount + dataflowObsCount > dataflowsrefresh.ObservationMax ||
                    dataflowsrefresh.ObservationMax == 0)
                {
                    //If group si full, create new group
                    setCountForGroupFilter(filtersCounter, currentKeyGroup, obsCount);

                    if (dataflowsrefresh.ObservationMax > 0 &&
                        addFilterInGroupWithFreeSpace(filtersByGroupFilter, filtersCounter, dataflowObsCount,
                            itemCode.Id, dataflowsrefresh.ObservationMax))
                    {
                        //Check for free space in oldes group
                        obsCount = 0;
                        filter = new FilterCriteria {Id = dimension.Id, FilterValues = new List<string>()};
                    }
                    else
                    {
                        //Oldes group haven't space, create new group with current filter
                        obsCount = dataflowObsCount;
                        filter = new FilterCriteria {Id = dimension.Id, FilterValues = new List<string> {itemCode.Id}};
                    }

                    currentKeyGroup = createGroupForFilter(filtersByGroupFilter, filtersCounter, filter);
                }
                else
                {
                    //Gruop is still free, add item codeId and increase obsCount
                    filter.FilterValues.Add(itemCode.Id);
                    obsCount += dataflowObsCount;
                }
            }

            setCountForGroupFilter(filtersCounter, currentKeyGroup, obsCount);
            return filtersByGroupFilter;
        }

        protected Dictionary<int, FilterCriteria> groupCacheByNumberItemCode(
            DataflowDataCacheGeneratorWorkerConfig.Dataflowsrefresh dataflowsrefresh, Criteria dimension)
        {
            if (dimension.Values.Count == 0) return new Dictionary<int, FilterCriteria>();
            if (dataflowsrefresh.GruopByNumber.GroupSize <= 0) dataflowsrefresh.GruopByNumber.GroupSize = 1;

            var filtersByGroupFilter = new Dictionary<int, FilterCriteria>();
            var filter = new FilterCriteria {Id = dimension.Id, FilterValues = new List<string>()};
            filtersByGroupFilter.Add(filtersByGroupFilter.Count + 1, filter);

            var i = 1;
            foreach (var itemCode in dimension.Values)
            {
                if (i % dataflowsrefresh.GruopByNumber.GroupSize == 0 ||
                    haveItemCodeToKeepInSingleGroup(itemCode.Id, dataflowsrefresh))
                {
                    filter.FilterValues.Add(itemCode.Id);
                    filter = new FilterCriteria {Id = dimension.Id, FilterValues = new List<string>()};
                    filtersByGroupFilter.Add(filtersByGroupFilter.Count + 1, filter);
                }
                else
                {
                    filter.FilterValues.Add(itemCode.Id);
                }

                i++;
            }

            return filtersByGroupFilter;
        }

        private static bool haveItemCodeToKeepInSingleGroup(string itemCode,
            DataflowDataCacheGeneratorWorkerConfig.Dataflowsrefresh dataflowsrefresh)
        {
            if (dataflowsrefresh?.GruopByNumber?.ExceptionValues == null) return false;

            return dataflowsrefresh.GruopByNumber.ExceptionValues.Any(i =>
                i.Equals(itemCode, StringComparison.InvariantCultureIgnoreCase));
        }

        private int createGroupForFilter(Dictionary<int, FilterCriteria> groupFilter, Dictionary<int, long> groupSize,
            FilterCriteria filterCriteria)
        {
            var key = groupFilter.Count;
            groupFilter.Add(key, filterCriteria);
            groupSize.Add(key, 0);

            return key;
        }

        private void setCountForGroupFilter(Dictionary<int, long> filtersCounter, int key, long obsCount)
        {
            filtersCounter[key] = obsCount;
        }

        private bool addFilterInGroupWithFreeSpace(Dictionary<int, FilterCriteria> groupFilter,
            Dictionary<int, long> filtersCounter, long dataflowObsCount, string codeId, int observationMaxForGroup)
        {
            if (observationMaxForGroup == 0) return false;

            foreach (var item in filtersCounter)
                if (item.Value + dataflowObsCount <= observationMaxForGroup)
                {
                    setCountForGroupFilter(filtersCounter, item.Key, filtersCounter[item.Key] + dataflowObsCount);
                    groupFilter[item.Key].FilterValues.Add(codeId);
                    return true;
                }

            return false;
        }
    }
}