using DataBrowser.AC.Utility;
using DataBrowser.DomainServices.Interfaces;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Cache;
using DataBrowser.Interfaces.Dto.UseCases.Requests;
using DataBrowser.Interfaces.Dto.UseCases.Responses;
using DataBrowser.Interfaces.Mediator;
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
    public class GetCodelistDynamicForDataflowUseCase : IUseCaseHandler<GetCodelistDynamicForDataflowRequest,
        GetCodelistDynamicForDataflowResponse>
    {
        private readonly IDataBrowserMemoryCache _dataBrowserMemoryCache;
        private readonly IEndPointConnectorFactory _endPointConnectorFactory;

        private readonly ILogger<GetCodelistDynamicForDataflowUseCase> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly INodeConfigService _nodeConfigService;
        private readonly IRequestContext _requestContext;

        public GetCodelistDynamicForDataflowUseCase(ILogger<GetCodelistDynamicForDataflowUseCase> logger,
            IEndPointConnectorFactory endPointConnectorFactory,
            IRequestContext requestContext,
            ILoggerFactory loggerFactory,
            INodeConfigService nodeConfigService,
            IDataBrowserMemoryCache dataBrowserMemoryCache)
        {
            _logger = logger;
            _endPointConnectorFactory = endPointConnectorFactory;
            _requestContext = requestContext;
            _loggerFactory = loggerFactory;
            _nodeConfigService = nodeConfigService;
            _dataBrowserMemoryCache = dataBrowserMemoryCache;
        }

        public async Task<GetCodelistDynamicForDataflowResponse> Handle(GetCodelistDynamicForDataflowRequest request,
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

            var filterCriteria = new List<FilterCriteria>();
            if (request.DataCriterias == null)
            {
                request.DataCriterias = new List<FilterCriteria>();
            }

            foreach (var itemFilter in request.DataCriterias)
            {
                var itemFind = dsd.Dimensions.FirstOrDefault(i =>
                    i.Id.Equals(itemFilter.Id, StringComparison.InvariantCultureIgnoreCase) &&
                    i?.Representation?.RefType != null &&
                    i.Representation.RefType == ArtefactType.ArtefactEnumType.CodeList);
                if (itemFind == null)
                {
                    _logger.LogDebug($"GetCriteriaFilterForDataflowUseCase Not found criteria {itemFilter.Id}");
                    throw new Exception(
                        $"FILTER_CRITERIA_INVALID:GetCodeListCostraintFilterAsync Not found criteria {itemFilter.Id}");
                }

                filterCriteria.Add(new FilterCriteria { Id = itemFilter.Id, FilterValues = itemFilter.FilterValues });
            }

            var allCodelist =
                await endPointConnector.GetCodeListCostraintFilterAsync(dataflow, dsd, request.DimensionId,
                    filterCriteria, true);

            var criterias = new List<Criteria>();
            if (!string.IsNullOrWhiteSpace(request.DimensionId))
            {
                var itemFind = dsd?.Dimensions?.FirstOrDefault(i =>
                    i.Id.Equals(request.DimensionId, StringComparison.InvariantCultureIgnoreCase));
                if (itemFind != null)
                {
                    var allItemFromCodelist = setSelectableCriteriaCode(allCodelist, itemFind);

                    criterias = new List<Criteria>
                    {
                        new Criteria
                        {
                            Id = itemFind.Id,
                            DataStructureRef = new ArtefactRef
                                {Id = itemFind.Representation.Id, RefType = ArtefactType.ArtefactEnumType.CodeList},
                            Values = allItemFromCodelist
                        }
                    };
                }
            }
            else
            {
                foreach (var itemDim in dsd.Dimensions)
                {
                    if (itemDim?.Representation?.RefType == null ||
                        itemDim.Representation.RefType != ArtefactType.ArtefactEnumType.CodeList)
                    {
                        continue;
                    }

                    var allItemFromCodelist = setSelectableCriteriaCode(allCodelist, itemDim);

                    var criteria = new Criteria
                    {
                        Id = itemDim.Id,
                        DataStructureRef = new ArtefactRef
                        { Id = itemDim.Representation.Id, RefType = ArtefactType.ArtefactEnumType.CodeList },
                        Values = allItemFromCodelist
                    };
                    criterias.Add(criteria);
                }
            }

            criterias = Utility.RemoveHiddenCriteria(dataflow, dsd, criterias, _requestContext.UserLang);


            RequestContextUtility.RestoreOriginalUseCaseRequestNodeContext(_requestContext, originalNodeContext.Item1,
                originalNodeContext.Item2);

            _logger.LogDebug("End Handle");
            return new GetCodelistDynamicForDataflowResponse
            { ArtefactContainer = new ArtefactContainer { Criterias = criterias, ObsCount = allCodelist.ObsCount } };
        }

        private static List<Code> setSelectableCriteriaCode(ArtefactContainer allCodelist, Dimension itemFind)
        {
            var allItemFromCodelist = allCodelist?.Codelists?.FirstOrDefault(i =>
                i.Id.Equals(itemFind.Representation.Id, StringComparison.InvariantCultureIgnoreCase))?.Items;
            var allItemCriteria = allCodelist?.Criterias?.FirstOrDefault();

            if (allItemFromCodelist != null)
            {
                if (allItemCriteria == null)
                {
                    foreach (var item in allItemFromCodelist)
                    {
                        item.IsSelectable = true;
                    }
                }
                else
                {
                    foreach (var item in allItemFromCodelist)
                    {
                        var criteriaItem = allItemCriteria?.Values?.FirstOrDefault(i => i.Id.Equals(item.Id));
                        item.IsSelectable = criteriaItem != null ? true : false;
                    }
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