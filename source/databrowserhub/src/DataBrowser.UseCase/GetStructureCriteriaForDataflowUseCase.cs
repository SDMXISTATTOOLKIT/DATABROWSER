using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataBrowser.AC.Utility;
using DataBrowser.AC.Utility.Helpers.ExtMethod;
using DataBrowser.Domain.Dtos;
using DataBrowser.DomainServices.Interfaces;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Cache;
using DataBrowser.Interfaces.Cache.Key;
using DataBrowser.Interfaces.Dto.UseCases.Requests;
using DataBrowser.Interfaces.Dto.UseCases.Responses;
using DataBrowser.Interfaces.Mediator;
using DataBrowser.Query.Hubs;
using DataBrowser.Query.Nodes;
using DataBrowser.Query.ViewTemplates;
using DataBrowser.Services.Interfaces;
using DataBrowser.UseCase.Common;
using EndPointConnector.Interfaces;
using EndPointConnector.Models;
using Microsoft.Extensions.Logging;

namespace DataBrowser.UseCase
{
    public class GetStructureCriteriaForDataflowUseCase : IUseCaseHandler<StructureCriteriaForDataflowRequest,
        StructureCriteriaForDataflowResponse>
    {
        private readonly IDataBrowserMemoryCache _dataBrowserMemoryCache;
        private readonly IEndPointConnectorFactory _endPointConnectorFactory;
        private readonly ILogger<GetStructureCriteriaForDataflowUseCase> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IMediatorService _mediatorService;
        private readonly INodeConfigService _nodeConfigService;
        private readonly IRequestContext _requestContext;
        private readonly IDatasetService _datasetService;

        public GetStructureCriteriaForDataflowUseCase(ILogger<GetStructureCriteriaForDataflowUseCase> logger,
            IEndPointConnectorFactory endPointConnectorFactory,
            IRequestContext requestContext,
            IMediatorService mediatorService,
            ILoggerFactory loggerFactory,
            INodeConfigService nodeConfigService,
            IDataBrowserMemoryCache dataBrowserMemoryCache,
            IDatasetService datasetService)
        {
            _logger = logger;
            _endPointConnectorFactory = endPointConnectorFactory;
            _requestContext = requestContext;
            _mediatorService = mediatorService;
            _loggerFactory = loggerFactory;
            _nodeConfigService = nodeConfigService;
            _dataBrowserMemoryCache = dataBrowserMemoryCache;
            _datasetService = datasetService;
        }

        public async Task<StructureCriteriaForDataflowResponse> Handle(StructureCriteriaForDataflowRequest request,
            CancellationToken cancellationToken)
        {
            _logger.LogDebug("Start Handle");
            if (request == null)
            {
                _logger.LogWarning("Null UseCase");
                return null;
            }

            var originalNodeContext = RequestContextUtility.GetOriginalUseCaseRequestNodeContext(_requestContext);

            _logger.LogDebug("Get Node config");
            var nodeConfig = await _nodeConfigService.GenerateNodeConfigAsync(_requestContext.NodeId);
            var endPointConnector = await _endPointConnectorFactory.Create(nodeConfig, _requestContext);
            endPointConnector.TryUseCache = true;

            Dataflow dataflow = null;
            Dsd dsdWithoutCodelist = null;
            List<Codelist> codelistWithNames = null;
            var tupleResult = await Utility.GetDataflowWithDsdAndEndpointConnectorFromCacheAsync(request.DataflowId,
                _dataBrowserMemoryCache, _requestContext, _loggerFactory, nodeConfig, endPointConnector,
                _nodeConfigService, includeCodelist: true);
            if (tupleResult.Item1)
            {
                _logger.LogDebug("try to use cache");
                nodeConfig = tupleResult.Item2;
                endPointConnector = tupleResult.Item3;
                dataflow = tupleResult.Item4;
                dsdWithoutCodelist = tupleResult.Item5;
                codelistWithNames = tupleResult.Item6;
            }


            if (dataflow == null ||
                dsdWithoutCodelist == null)
            {
                _logger.LogDebug("not found from cache, use connector");
                var resultData = await Utility.GetDataflowInfoFromEndPointWithConnectorAsync(request.DataflowId,
                    nodeConfig, endPointConnector, _loggerFactory, _requestContext, _endPointConnectorFactory,
                    _nodeConfigService);
                nodeConfig = resultData.Item1;
                endPointConnector = resultData.Item2;
                dataflow = resultData.Item3;
                dsdWithoutCodelist = resultData.Item4;
            }


            
            
            if (codelistWithNames == null)
            {
                ArtefactContainer container = null;
                try
                {
                    container = await endPointConnector.GetArtefactAsync(
                        ArtefactType.ArtefactEnumType.Dsd, dsdWithoutCodelist.Id,
                        ArtefactType.ReferenceDetailEnumType.Children, ArtefactType.ResponseDetailEnumType.Stub);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Try callback for error in Stub", ex);
                    container = await endPointConnector.GetArtefactAsync(
                        ArtefactType.ArtefactEnumType.Dsd, dsdWithoutCodelist.Id,
                        ArtefactType.ReferenceDetailEnumType.Children);
                }
                codelistWithNames = container?.Codelists;
            }



            var criterias = new List<Criteria>();
            foreach (var dsdDim in dsdWithoutCodelist.Dimensions)
            {
                var criteria = new Criteria { Id = dsdDim.Id };
                criterias.Add(criteria);
                if (dsdDim?.Representation?.RefType != null &&
                    dsdDim.Representation.RefType == ArtefactType.ArtefactEnumType.CodeList)
                {
                    var codeList = codelistWithNames?.Where(i =>
                            i.Id.Equals(dsdDim.Representation.Id, StringComparison.InvariantCultureIgnoreCase))
                        ?.SingleOrDefault();
                    if (codeList == null)
                    {
                        var container = await endPointConnector.GetArtefactAsync(ArtefactType.ArtefactEnumType.CodeList, dsdDim.Representation.Id, ArtefactType.ReferenceDetailEnumType.None, ArtefactType.ResponseDetailEnumType.Stub);
                        codeList = container?.Codelists?.FirstOrDefault();
                    }
                    if (codeList != null)
                    {
                        var titles = codeList?.Names;
                        criteria.Titles = titles;
                        criteria.DataStructureRef = new ArtefactRef
                        { Id = dsdDim.Representation.Id, RefType = ArtefactType.ArtefactEnumType.CodeList };
                    }
                }
            }

            criterias = criterias
                .Where(cr => !cr.Id.Equals(Utility.CalculateHiddenCriteria(cr, dataflow, dsdWithoutCodelist, _requestContext.UserLang)?.Id )) 
                .ToList();

            RequestContextUtility.RestoreOriginalUseCaseRequestNodeContext(_requestContext, originalNodeContext.Item1,
                originalNodeContext.Item2);

            var node = await _mediatorService.QueryAsync(new NodeByIdQuery(_requestContext.NodeId));
            var hub = await _mediatorService.QueryAsync(new HubsListQuery());

            ViewTemplateDto template = null;
            ViewTemplateDto view = null;
            if (request.ViewId.HasValue)
            {
                view = await _mediatorService.QueryAsync(
                    new ViewTemplateById_Type_NodeIdQuery(request.ViewId.Value,
                        _requestContext.NodeId,
                        ViewTemplateType.View,
                        true));
                if (view?.DatasetId == null ||
                    !view.DatasetId.Equals(request.DataflowId, StringComparison.InvariantCultureIgnoreCase))
                    view = null;
            }

            
            template = (await _mediatorService.QueryAsync(
                    new ViewTemplateByType_Dataset_NodeIdQuery(_requestContext.NodeId, request.DataflowId,
                        ViewTemplateType.Template)))?.FirstOrDefault();

            var dataset = _datasetService.CreateDataset(hub.First(), node, dataflow, dsdWithoutCodelist, _requestContext.UserLang);

            var criteriaResponse = new StructureCriteriaForDataflowResponse
            {
                Criterias = criterias,
                CriteriaMode = dataset.CriteriaMode,
                DecimalNumber = dataset.DecimalNumber,
                DecimalSeparator = dataset.DecimalSeparator,
                EmptyCellPlaceHolder = dataset.EmptyCellPlaceHolder,
                TerritorialDimensions = dataset.TerritorialDimensions,
                MaxCell = dataset.MaxCell,
                LayoutRows = dataset.LayoutRows,
                LayoutColumns= dataset.LayoutColumns,
                LayoutRowSelections = dataset.LayoutRowSections,
                DefaultCodeSelected = dataset.DefaultCodeSelected,
                DefaultView = dataset.DefaultView,
                GeoIds= dataset.GeoIds,
                Template = template,
                View = view,
                LayoutChartPrimaryDim = dataset.LayoutChartPrimaryDim,
                LayoutChartSecondaryDim = dataset.LayoutChartSecondaryDim,
                LayoutChartFilter = dataset.LayoutChartFilter
            };

            _logger.LogDebug("End Handle");
            return criteriaResponse;
        }
    }
}