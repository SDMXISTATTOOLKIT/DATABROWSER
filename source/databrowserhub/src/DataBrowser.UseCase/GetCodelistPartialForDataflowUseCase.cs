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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataBrowser.UseCase
{
    public class GetCodelistPartialForDataflowUseCase : IUseCaseHandler<GetCodelistPartialForDataflowRequest,
        GetCodelistPartialForDataflowResponse>
    {
        private readonly IDataBrowserMemoryCache _dataBrowserMemoryCache;
        private readonly IEndPointConnectorFactory _endPointConnectorFactory;
        private readonly ILogger<GetCodelistPartialForDataflowUseCase> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly INodeConfigService _nodeConfigService;
        private readonly IRequestContext _requestContext;
        private readonly IMediatorService _mediatorService;

        public GetCodelistPartialForDataflowUseCase(ILogger<GetCodelistPartialForDataflowUseCase> logger,
            IEndPointConnectorFactory endPointConnectorFactory,
            IRequestContext requestContext,
            ILoggerFactory loggerFactory,
            INodeConfigService nodeConfigService,
            IDataBrowserMemoryCache dataBrowserMemoryCache,
            IMediatorService mediatorService)
        {
            _logger = logger;
            _endPointConnectorFactory = endPointConnectorFactory;
            _requestContext = requestContext;
            _loggerFactory = loggerFactory;
            _nodeConfigService = nodeConfigService;
            _dataBrowserMemoryCache = dataBrowserMemoryCache;
            _mediatorService = mediatorService;
        }

        public async Task<GetCodelistPartialForDataflowResponse> Handle(GetCodelistPartialForDataflowRequest request,
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
                _logger.LogDebug("try to use cache");
                nodeConfig = tupleResult.Item2;
                endPointConnector = tupleResult.Item3;
                dataflow = tupleResult.Item4;
                dsd = tupleResult.Item5;
            }


            if (dataflow == null ||
                dsd == null)
            {
                _logger.LogDebug("not found from cache, use connector");
                var resultData = await Utility.GetDataflowInfoFromEndPointWithConnectorAsync(request.DataflowId,
                    nodeConfig, endPointConnector, _loggerFactory, _requestContext, _endPointConnectorFactory,
                    _nodeConfigService);
                nodeConfig = resultData.Item1;
                endPointConnector = resultData.Item2;
                dataflow = resultData.Item3;
                dsd = resultData.Item4;
            }

            var allInOne = false;
            ArtefactContainer containerAllCodelist = null;
            if (request.DimensionIds == null || request.DimensionIds.Count <= 0)
            {
                //TODO remove comment when NSI resolve BUG
                //allInOne = true;
                request.DimensionIds = new List<string>();
                foreach (var item in dsd.Dimensions)
                {
                    request.DimensionIds.Add(item.Id);
                }

                //TODO remove comment when NSI resolve BUG
                //containerAllCodelist = await endPointConnector.GetArtefactAsync(ArtefactType.ArtefactEnumType.Dataflow,
                //    request.DataflowId, ArtefactType.ReferenceDetailEnumType.All,
                //    ArtefactType.ResponseDetailEnumType.ReferencePartial, orderItems: true);
            }

            var criterias = new List<Criteria>();
            foreach (var itemDimension in request.DimensionIds)
            {
                foreach (var dsdDim in dsd.Dimensions)
                {
                    if (dsdDim?.Representation?.RefType != null &&
                        (dsdDim.Representation.RefType == ArtefactType.ArtefactEnumType.CodeList ||
                        dsdDim.Representation.RefType == ArtefactType.ArtefactEnumType.ConceptScheme) &&
                        dsdDim.Id.Equals(itemDimension, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var criteria = new Criteria
                        {
                            Id = dsdDim.Id,
                            DataStructureRef = new ArtefactRef
                            { Id = dsdDim.Representation.Id, RefType = dsdDim.Representation.RefType }
                        };
                        if (allInOne && containerAllCodelist != null)
                        {
                            if (dsdDim.Id.Equals("TIME_PERIOD", StringComparison.InvariantCultureIgnoreCase))
                            {
                                try
                                {
                                    var inputHanlde = new GetCodelistDynamicForDataflowRequest
                                    {
                                        DataflowId = request.DataflowId,
                                        DimensionId = criteria.Id
                                    };
                                    var useCaseResult = await _mediatorService.Send(inputHanlde);
                                    criteria.Values = useCaseResult?.ArtefactContainer?.Criterias?.FirstOrDefault()
                                        ?.Values;

                                    criterias.Add(criteria);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogWarning(
                                        "Specific value for TIME_PERIOD not supported, return empty value for call back",
                                        ex);
                                }
                            }
                            else
                            {
                                criteria.Values = containerAllCodelist.Codelists.FirstOrDefault(i =>
                                        i.Id.Equals(dsdDim.Representation.Id, StringComparison.InvariantCultureIgnoreCase))
                                    ?.Items;
                                criterias.Add(criteria);
                            }
                        }
                        else
                        {
                            if (dsdDim.Id.Equals("TIME_PERIOD", StringComparison.InvariantCultureIgnoreCase))
                            {
                                try
                                {
                                    var inputHanlde = new GetCodelistDynamicForDataflowRequest
                                    {
                                        DataflowId = request.DataflowId,
                                        DimensionId = criteria.Id
                                    };
                                    var useCaseResult = await _mediatorService.Send(inputHanlde);
                                    criteria.Values = useCaseResult?.ArtefactContainer?.Criterias?.FirstOrDefault()
                                        ?.Values;
                                    criterias.Add(criteria);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogWarning(
                                        "Specific value for TIME_PERIOD not supported, return empty value for call back",
                                        ex);
                                }
                            }
                            else
                            {
                                var allCodelist =
                                await endPointConnector.GetCodeListCostraintAsync(dataflow, dsd, itemDimension, true);

                                criteria.Values = setSelectableCriteriaCode(allCodelist, dsdDim);

                                criterias.Add(criteria);
                            }
                        }
                    }
                    else if (dsdDim?.Representation?.RefType != null &&
                             (dsdDim.Representation.RefType != ArtefactType.ArtefactEnumType.CodeList &&
                             dsdDim.Representation.RefType != ArtefactType.ArtefactEnumType.ConceptScheme) &&
                             dsdDim.Id.Equals(itemDimension, StringComparison.InvariantCultureIgnoreCase))
                    {
                        criterias.Add(new Criteria { Id = dsdDim.Id });
                    }
                }
            }

            criterias = Utility.RemoveHiddenCriteria(dataflow, dsd, criterias, _requestContext.UserLang);

            RequestContextUtility.RestoreOriginalUseCaseRequestNodeContext(_requestContext, originalNodeContext.Item1,
                originalNodeContext.Item2);

            _logger.LogDebug("End Handle");
            return new GetCodelistPartialForDataflowResponse
            { ArtefactContainer = new ArtefactContainer { Criterias = criterias } };
        }

        private static List<Code> setSelectableCriteriaCode(ArtefactContainer allCodelist, Dimension itemFind)
        {
            var allItemFromCodelist = allCodelist?.Codelists?.FirstOrDefault(i =>
                i.Id.Equals(itemFind.Representation.Id, StringComparison.InvariantCultureIgnoreCase))?.Items;
            var allItemCriteria = allCodelist?.Criterias?.FirstOrDefault();

            if (allItemFromCodelist != null)
            {
                foreach (var item in allItemFromCodelist)
                {
                    var criteriaItem = allItemCriteria?.Values?.FirstOrDefault(i => i.Id.Equals(item.Id));
                    item.IsSelectable = criteriaItem != null ? true : false;
                }
            }


            //if (allItemCriteria?.Values != null && allItemFromCodelist != null)
            //{
            //foreach (var item in allItemCriteria.Values)
            //{
            //    var codelistItem = allItemFromCodelist.FirstOrDefault(i => i.Id.Equals(item.Id));
            //    if (codelistItem != null)
            //    {
            //        codelistItem.IsSelectable = true;
            //    }
            //}
            //}

            return allItemFromCodelist;
        }
    }
}