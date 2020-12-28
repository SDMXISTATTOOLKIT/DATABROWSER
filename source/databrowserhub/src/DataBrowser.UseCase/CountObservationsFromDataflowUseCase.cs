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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataBrowser.UseCase
{
    public class CountObservationsFromDataflowUseCase : IUseCaseHandler<CountObservationsFromDataflowRequest,
        CountObservationsFromDataflowResponse>
    {
        private readonly IDataBrowserMemoryCache _dataBrowserMemoryCache;
        private readonly IEndPointConnectorFactory _endPointConnectorFactory;
        private readonly ILogger<CountObservationsFromDataflowUseCase> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly INodeConfigService _nodeConfigService;
        private readonly IRequestContext _requestContext;

        public CountObservationsFromDataflowUseCase(ILogger<CountObservationsFromDataflowUseCase> logger,
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

        public async Task<CountObservationsFromDataflowResponse> Handle(CountObservationsFromDataflowRequest request,
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

            var constraints = new List<Criteria>();
            foreach (var dsdDim in dsd.Dimensions)
            {
                if (dsdDim.Representation.RefType != ArtefactType.ArtefactEnumType.CodeList) continue;
                constraints.Add(new Criteria
                {
                    Id = dsdDim.Id,
                    DataStructureRef = new ArtefactRef
                        {Id = dsdDim.Representation.Id, RefType = ArtefactType.ArtefactEnumType.CodeList}
                });
            }

            var results = await endPointConnector.GetDataflowObservationCountAsync(dataflow, dsd,
                request.DataCriterias?.Select(i => new FilterCriteria {Id = i.Id, FilterValues = i.FilterValues})
                    ?.ToList());

            RequestContextUtility.RestoreOriginalUseCaseRequestNodeContext(_requestContext, originalNodeContext.Item1,
                originalNodeContext.Item2);

            _logger.LogDebug("End Handle");
            return new CountObservationsFromDataflowResponse {Count = results};
        }
    }
}