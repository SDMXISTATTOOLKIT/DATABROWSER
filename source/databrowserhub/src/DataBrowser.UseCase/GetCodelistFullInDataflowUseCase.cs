using DataBrowser.AC.Utility;
using DataBrowser.DomainServices.Interfaces;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Cache;
using DataBrowser.Interfaces.Dto.UseCases.Requests;
using DataBrowser.Interfaces.Dto.UseCases.Responses;
using DataBrowser.Interfaces.Mediator;
using DataBrowser.Services.Interfaces;
using DataBrowser.UseCase.Common;
using EndPointConnector.Interfaces;
using EndPointConnector.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataBrowser.UseCase
{
    public class
        GetCodelistFullInDataflowUseCase : IUseCaseHandler<GetCodelistFullInDataflowRequest,
            GetCodelistFullInDataflowResponse>
    {
        private readonly IDataBrowserMemoryCache _dataBrowserMemoryCache;
        private readonly IEndPointConnectorFactory _endPointConnectorFactory;
        private readonly ILogger<GetCodelistFullInDataflowUseCase> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IMediatorService _mediatorService;
        private readonly INodeConfigService _nodeConfigService;
        private readonly IRequestContext _requestContext;

        public GetCodelistFullInDataflowUseCase(ILogger<GetCodelistFullInDataflowUseCase> logger,
            IEndPointConnectorFactory endPointConnectorFactory,
            IRequestContext requestContext,
            IMediatorService mediatorService,
            ILoggerFactory loggerFactory,
            INodeConfigService nodeConfigService,
            IDataBrowserMemoryCache dataBrowserMemoryCache)
        {
            _logger = logger;
            _endPointConnectorFactory = endPointConnectorFactory;
            _requestContext = requestContext;
            _mediatorService = mediatorService;
            _loggerFactory = loggerFactory;
            _nodeConfigService = nodeConfigService;
            _dataBrowserMemoryCache = dataBrowserMemoryCache;
        }

        public async Task<GetCodelistFullInDataflowResponse> Handle(GetCodelistFullInDataflowRequest request,
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

            var isAllInOne = false;
            if (request.DimensionIds == null || request.DimensionIds.Count <= 0)
            {
                isAllInOne = true;
                request.DimensionIds = new List<string>();
                foreach (var item in dsd.Dimensions)
                {
                    request.DimensionIds.Add(item.Id);
                }
            }

            var criterias = new List<Criteria>();
            foreach (var itemDimension in request.DimensionIds)
            {
                foreach (var dsdDim in dsd.Dimensions)
                {
                    if (dsdDim?.Representation?.RefType != null &&
                    dsdDim.Representation.RefType == ArtefactType.ArtefactEnumType.CodeList &&
                    dsdDim.Id.Equals(itemDimension, StringComparison.InvariantCultureIgnoreCase))
                    {
                        criterias.Add(new Criteria
                        {
                            Id = dsdDim.Id,
                            DataStructureRef = new ArtefactRef
                            { Id = dsdDim.Representation.Id, RefType = ArtefactType.ArtefactEnumType.CodeList }
                        });
                    }
                    else if (dsdDim?.Representation?.RefType != null &&
                         dsdDim.Representation.RefType != ArtefactType.ArtefactEnumType.CodeList &&
                         dsdDim.Id.Equals(itemDimension, StringComparison.InvariantCultureIgnoreCase))
                    {
                        criterias.Add(new Criteria { Id = dsdDim.Id });
                    }
                }
            }

            if (isAllInOne)
            {
                var responseTestWatch = Stopwatch.StartNew();
                var container = await endPointConnector.GetArtefactAsync(ArtefactType.ArtefactEnumType.Dsd,
                    dataflow.DataStructureRef.Id, orderItems: true,
                    refDetail: ArtefactType.ReferenceDetailEnumType.Children);
                responseTestWatch.Stop();


                foreach (var itemCriteria in criterias)
                {
                    if (itemCriteria.DataStructureRef != null)
                    {
                        var codelistAdd =
                            container.Codelists.FirstOrDefault(i => i.Id.Equals(itemCriteria.DataStructureRef.Id));
                        if (itemCriteria.Id.Equals("TIME_PERIOD", StringComparison.InvariantCultureIgnoreCase))
                        {
                            try
                            {
                                var inputHanlde = new GetCodelistDynamicForDataflowRequest
                                {
                                    DataflowId = request.DataflowId,
                                    DimensionId = itemCriteria.Id
                                };
                                var useCaseResult = await _mediatorService.Send(inputHanlde);
                                itemCriteria.Values = useCaseResult?.ArtefactContainer?.Criterias?.FirstOrDefault()
                                    ?.Values;
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(
                                    "Specific value for TIME_PERIOD not supported, return empty value for call back",
                                    ex);
                            }
                        }
                        else if (codelistAdd != null)
                        {
                            itemCriteria.Values = codelistAdd.Items;
                        }
                        else
                        {
                            _logger.LogWarning(
                                $"Criteria {itemCriteria.Id}. Artefact not found {ArtefactType.ArtefactEnumType.CodeList}\t{itemCriteria.DataStructureRef.Id}");
                        }
                    }
                    else
                    {
                        itemCriteria.Values = null;
                    }
                }
            }
            else
            {
                foreach (var itemCriteria in criterias)
                {
                    if (itemCriteria.DataStructureRef != null)
                    {
                        var codelistAdd = await endPointConnector.GetArtefactAsync(
                            ArtefactType.ArtefactEnumType.CodeList, itemCriteria.DataStructureRef.Id, orderItems: true);

                        if (itemCriteria.Id.Equals("TIME_PERIOD", StringComparison.InvariantCultureIgnoreCase))
                        {
                            try
                            {
                                var inputHanlde = new GetCodelistDynamicForDataflowRequest
                                {
                                    DataflowId = request.DataflowId,
                                    DimensionId = itemCriteria.Id
                                };
                                var useCaseResult = await _mediatorService.Send(inputHanlde);
                                itemCriteria.Values = useCaseResult?.ArtefactContainer?.Criterias?.FirstOrDefault()
                                    ?.Values;
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(
                                    "Specific value for TIME_PERIOD not supported, return empty value for call back",
                                    ex);
                            }
                        }
                        else if (codelistAdd != null && codelistAdd.Codelists.Any())
                        {
                            itemCriteria.Values = codelistAdd.Codelists.First().Items;
                        }
                        else
                        {
                            _logger.LogWarning(
                                $"Criteria {itemCriteria.Id}. Artefact not found {ArtefactType.ArtefactEnumType.CodeList}\t{itemCriteria.DataStructureRef.Id}");
                        }
                    }
                    else
                    {
                        itemCriteria.Values = null;
                    }
                }
            }

            criterias = Utility.RemoveHiddenCriteria(dataflow, dsd, criterias, _requestContext.UserLang);

            RequestContextUtility.RestoreOriginalUseCaseRequestNodeContext(_requestContext, originalNodeContext.Item1,
                originalNodeContext.Item2);

            _logger.LogDebug("End Handle");
            return new GetCodelistFullInDataflowResponse
            { ArtefactContainer = new ArtefactContainer { Criterias = criterias } };
        }
    }
}