using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataBrowser.AC.Utility;
using DataBrowser.DomainServices.Interfaces;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Dto.UseCases.Requests;
using DataBrowser.Interfaces.Dto.UseCases.Responses;
using DataBrowser.Interfaces.Mediator;
using DataBrowser.Query.Nodes;
using DataBrowser.Services.Interfaces;
using EndPointConnector.Interfaces;
using EndPointConnector.Models.Dto;
using Microsoft.Extensions.Logging;

namespace DataBrowser.UseCase
{
    public class GetNodeCatalogTreeUseCase : IUseCaseHandler<NodeCatalogTreeRequest, GetNodeCatalogTreeResponse>
    {
        private readonly IEndPointConnectorFactory _endPointConnectorFactory;
        private readonly ILogger<GetNodeCatalogTreeUseCase> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IMediatorService _mediatorService;
        private readonly INodeConfigService _nodeConfigService;
        private readonly IRequestContext _requestContext;

        public GetNodeCatalogTreeUseCase(ILogger<GetNodeCatalogTreeUseCase> logger,
            IRequestContext requestContext,
            IEndPointConnectorFactory endPointConnectorFactory,
            IMediatorService mediatorService,
            ILoggerFactory loggerFactory,
            INodeConfigService nodeConfigService)
        {
            _logger = logger;
            _requestContext = requestContext;
            _endPointConnectorFactory = endPointConnectorFactory;
            _mediatorService = mediatorService;
            _loggerFactory = loggerFactory;
            _nodeConfigService = nodeConfigService;
        }

        public async Task<GetNodeCatalogTreeResponse> Handle(NodeCatalogTreeRequest request,
            CancellationToken cancellationToken)
        {
            _logger.LogDebug("Start Handle");

            var originalNodeContext = RequestContextUtility.GetOriginalUseCaseRequestNodeContext(_requestContext);

            var isActive = await _mediatorService.QueryAsync(new NodeIsActiveQuery(_requestContext.NodeId,
                request.FilterByPermissionUser,
                filterBySpecificUser: request.FilterBySpecificUser));
            if (!isActive)
            {
                _logger.LogDebug($"Node {_requestContext.NodeId} not found");
                return null;
            }

            _logger.LogDebug("Get Node");
            var nodeConfig = await _nodeConfigService.GenerateNodeConfigAsync(_requestContext.NodeId);
            var endPointConnector = await _endPointConnectorFactory.Create(nodeConfig, _requestContext);

            var watch = Stopwatch.StartNew();
            var dataTree = await endPointConnector.GetNodeCatalogAsync(request.OrderLang);

            dataTree.DatasetMap = dataTree.DatasetMap ?? new Dictionary<string, DatasetDto>();
            dataTree.DatasetUncategorized = dataTree.DatasetUncategorized ?? new List<DatasetDto>();
            await VirtualDataflowUtility.FindVirtualDataflowAndPopolateWithRealDataAsync(
                dataTree.DatasetMap?.Values?.Union(dataTree.DatasetUncategorized).ToList(),
                _requestContext,
                _endPointConnectorFactory,
                _nodeConfigService,
                _loggerFactory,
                nodeConfig);

            _logger.LogTrace($"GetTreeCategoriesAsync Execution Time: {watch.ElapsedMilliseconds} ms");
            watch.Stop();
            if (dataTree == null) return new GetNodeCatalogTreeResponse {NodeCatalogDto = null};

            RequestContextUtility.RestoreOriginalUseCaseRequestNodeContext(_requestContext, originalNodeContext.Item1,
                originalNodeContext.Item2);

            _logger.LogDebug("End Handle");
            return new GetNodeCatalogTreeResponse {NodeCatalogDto = dataTree, NodeCode = nodeConfig.Code};
        }
    }
}