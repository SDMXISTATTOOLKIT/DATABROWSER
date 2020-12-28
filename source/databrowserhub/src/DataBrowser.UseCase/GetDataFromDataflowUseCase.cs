using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataBrowser.AC.Utility;
using DataBrowser.DomainServices.Interfaces;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Cache;
using DataBrowser.Interfaces.Cache.Key;
using DataBrowser.Interfaces.Dto.UseCases.Requests;
using DataBrowser.Interfaces.Dto.UseCases.Responses;
using DataBrowser.Interfaces.Mediator;
using DataBrowser.Services.Interfaces;
using DataBrowser.UseCase.Common;
using EndPointConnector.Interfaces;
using EndPointConnector.Interfaces.JsonStat;
using EndPointConnector.Models;
using Microsoft.Extensions.Logging;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;

namespace DataBrowser.UseCase
{
    public class GetDataFromDataflowUseCase : IUseCaseHandler<DataFromDataflowRequest, GetDataFromDataflowResponse>
    {
        private readonly IDataBrowserMemoryCache _dataBrowserMemoryCache;
        private readonly IDataflowDataCache _dataMemoryCache;
        private readonly IEndPointConnectorFactory _endPointConnectorFactory;
        private readonly IFromJsonStatToJsonStatConverterFactory _jsonStatConverterFactory;
        private readonly ILogger<GetDataFromDataflowUseCase> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IMediatorService _mediatorService;
        private readonly INodeConfigService _nodeConfigService;
        private readonly IRequestContext _requestContext;
        private readonly IDatasetService _datasetService;

        public GetDataFromDataflowUseCase(ILoggerFactory loggerFactory,
            IEndPointConnectorFactory endPointConnectorFactory,
            IRequestContext requestContext,
            IMediatorService mediatorService,
            IDataflowDataCache dataMemoryCache,
            IFromJsonStatToJsonStatConverterFactory jsonStatConverter,
            INodeConfigService nodeConfigService,
            IDataBrowserMemoryCache dataBrowserMemoryCache,
            IDatasetService datasetService)
        {
            _logger = loggerFactory.CreateLogger<GetDataFromDataflowUseCase>();
            _endPointConnectorFactory = endPointConnectorFactory;
            _requestContext = requestContext;
            _mediatorService = mediatorService;
            _dataMemoryCache = dataMemoryCache;
            _jsonStatConverterFactory = jsonStatConverter;
            _loggerFactory = loggerFactory;
            _nodeConfigService = nodeConfigService;
            _dataBrowserMemoryCache = dataBrowserMemoryCache;
            _datasetService = datasetService;
        }

        public async Task<GetDataFromDataflowResponse> Handle(DataFromDataflowRequest request,
            CancellationToken cancellationToken)
        {
            _logger.LogDebug("Start Handle");
            if (request == null)
            {
                _logger.LogWarning("Null UseCase");
                return null;
            }

            var originalNodeContext = RequestContextUtility.GetOriginalUseCaseRequestNodeContext(_requestContext);

            _logger.LogDebug("Get Node");
            var responseWatch = Stopwatch.StartNew();
            var nodeConfig = await _nodeConfigService.GenerateNodeConfigAsync(_requestContext.NodeId);
            var endPointConnector = await _endPointConnectorFactory.Create(nodeConfig, _requestContext);
            endPointConnector.TryUseCache = true;

            Dataflow dataflow = null;
            Dsd dsd = null;
            var tupleResult = await Utility.GetDataflowWithDsdAndEndpointConnectorFromCacheAsync(request.DataflowId,
                _dataBrowserMemoryCache, _requestContext, _loggerFactory, nodeConfig, endPointConnector,
                _nodeConfigService);
            if (tupleResult.Item1)
            {
                nodeConfig = tupleResult.Item2;
                endPointConnector = tupleResult.Item3;
                dataflow = tupleResult.Item4;
                dsd = tupleResult.Item5;
            }


            if (dataflow == null ||
                dsd == null)
            {
                var resultData = await Utility.GetDataflowInfoFromEndPointWithConnectorAsync(request.DataflowId,
                    nodeConfig, endPointConnector, _loggerFactory, _requestContext, _endPointConnectorFactory,
                    _nodeConfigService);
                nodeConfig = resultData.Item1;
                endPointConnector = resultData.Item2;
                dataflow = resultData.Item3;
                dsd = resultData.Item4;
            }

            GetDataFromDataflowResponse dataFromDataflowResponse = null;

            var dataset = _datasetService.CreateDataset(dataflow, dsd, _requestContext.UserLang);

            if (_dataMemoryCache != null
                && !_requestContext.IgnoreCache
                && !_requestContext.IsCacheRefresh)
            {
                _logger.LogDebug("GetDataFromDataflowUseCase Try to use cache");

                dataFromDataflowResponse =
                    await _dataMemoryCache.GetJsonStatForDataflowDataFromValidKey(request, dataset);

                if (dataFromDataflowResponse != null)
                {
                    _logger.LogDebug("GetDataFromDataflowUseCase found cache");
                    dataFromDataflowResponse.Timers = new Dictionary<string, string>
                        {{"cacheRead", $"{responseWatch.ElapsedMilliseconds}ms"}};

                    RequestContextUtility.RestoreOriginalUseCaseRequestNodeContext(_requestContext,
                        originalNodeContext.Item1, originalNodeContext.Item2);

                    return dataFromDataflowResponse;
                }

                if (dataFromDataflowResponse == null &&
                    request.TryUseCompatibleKeyCache)
                {
                    _logger.LogDebug("GetDataFromDataflowUseCase Try to use compatible cache");
                    responseWatch.Restart();
                    var jsonCompatible =
                        await _dataMemoryCache.GetJsonStatForDataflowDataFromValidKeyCompatible(request, dataset);
                    if (jsonCompatible != null)
                    {
                        _logger.LogDebug("GetDataFromDataflowUseCase use compatible cache");
                        jsonCompatible.ItemsCount = null;
                        jsonCompatible.ItemsFrom = null;
                        jsonCompatible.ItemsMax = null;
                        jsonCompatible.ItemsTo = null;
                        jsonCompatible.LimitExceeded = false;
                        jsonCompatible.Timers = new Dictionary<string, string>
                            {{"cacheRead", $"{responseWatch.ElapsedMilliseconds}ms"}};

                        responseWatch.Restart();
                        var criterias = request.DataCriterias?.ToList();

                        try
                        {
                            var converter =
                            _jsonStatConverterFactory.GetConverter(jsonCompatible.JsonData, dataset.NotDisplay, criterias, _requestContext.UserLang);
                            jsonCompatible.JsonData = converter.Convert();
                            jsonCompatible.Timers.Add("Filter Data Cache", $"{responseWatch.ElapsedMilliseconds}ms");
                        }
                        catch (Exception ex)
                        {
                            jsonCompatible.JsonData = null;
                            _logger.LogWarning("Some error during convert compatible key jsonstat, skip cache and get new data", ex);
                        }

                        if (jsonCompatible.JsonData != null)
                        {
                            await _dataMemoryCache.SetJsonStatForDataflowData(request, jsonCompatible, dataset);

                            RequestContextUtility.RestoreOriginalUseCaseRequestNodeContext(_requestContext,
                                originalNodeContext.Item1, originalNodeContext.Item2);

                            return jsonCompatible;
                        }
                    }
                }
            }

            _logger.LogDebug("GetDataFromDataflowUseCase not found valid cache");

            ISdmxObjects dataflowWithExtraData = null;
            if (_dataBrowserMemoryCache != null &&
                !_requestContext.IgnoreCache)
            {
                _logger.LogDebug("try to get dataflowWithExtraData from cache");
                dataflowWithExtraData = _dataBrowserMemoryCache.Get(new DataflowWithDataCacheKey(_requestContext.NodeId, dataflow.Id));
                if (dataflowWithExtraData == null)
                {
                    _logger.LogDebug("dataflowWithExtraData not in cache");
                    dataflowWithExtraData = await endPointConnector.GetDataflowWithAllUsedDataAsync(dataflow);
                    if (dataflowWithExtraData != null)
                    {
                        _logger.LogDebug("dataflowWithExtraData save to cache");
                        await _dataBrowserMemoryCache.AddAsync(dataflowWithExtraData, new DataflowWithDataCacheKey(_requestContext.NodeId, dataflow.Id));
                    }
                }
            }
            else if (_requestContext.IgnoreCache)
            {
                _logger.LogDebug("dataflowWithExtraData without cache");
                dataflowWithExtraData = await endPointConnector.GetDataflowWithAllUsedDataAsync(dataflow);
            }
            
            var results = await endPointConnector.GetDataflowDataAsync(dataflow, dsd, request.DataCriterias, dataflowWithExtraData);

            dataFromDataflowResponse = new GetDataFromDataflowResponse
            {
                JsonData = results.Data, Timers = results.Timers, ItemsCount = results.ItemsCount,
                ItemsFrom = results.ItemsFrom, ItemsTo = results.ItemsTo, ItemsMax = results.ItemsMax,
                LimitExceeded = results.LimitExceeded
            };

            if (_dataMemoryCache != null &&
                !_requestContext.IgnoreCache &&
                !dataFromDataflowResponse.LimitExceeded)
                await _dataMemoryCache.SetJsonStatForDataflowData(request, dataFromDataflowResponse, dataset);

            RequestContextUtility.RestoreOriginalUseCaseRequestNodeContext(_requestContext, originalNodeContext.Item1,
                originalNodeContext.Item2);

            _logger.LogDebug("End Handle");
            return dataFromDataflowResponse;
        }
    }
}