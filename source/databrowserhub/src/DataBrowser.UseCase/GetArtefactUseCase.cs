using System.Threading;
using System.Threading.Tasks;
using DataBrowser.DomainServices.Interfaces;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Dto.UseCases.Requests;
using DataBrowser.Interfaces.Dto.UseCases.Responses;
using DataBrowser.Interfaces.Mediator;
using EndPointConnector.Interfaces;
using Microsoft.Extensions.Logging;

namespace DataBrowser.UseCase
{
    public class GetArtefactUseCase : IUseCaseHandler<ArtefactRequestRequest, GetArtefactResponse>
    {
        private readonly IEndPointConnectorFactory _endPointConnectorFactory;
        private readonly INodeConfigService _endPointService;
        private readonly ILogger<GetArtefactUseCase> _logger;
        private readonly IRequestContext _requestContext;

        public GetArtefactUseCase(ILogger<GetArtefactUseCase> logger,
            IEndPointConnectorFactory endPointConnectorFactory,
            IRequestContext requestContext,
            INodeConfigService endPointService)
        {
            _logger = logger;
            _endPointConnectorFactory = endPointConnectorFactory;
            _requestContext = requestContext;
            _endPointService = endPointService;
        }

        public async Task<GetArtefactResponse> Handle(ArtefactRequestRequest request,
            CancellationToken cancellationToken)
        {
            _logger.LogDebug("Start Handle");
            if (request == null)
            {
                _logger.LogWarning("Null UseCase");
                return null;
            }

            _logger.LogDebug("Get Node");

            var endPointConfig = await _endPointService.GenerateNodeConfigAsync(_requestContext.NodeId);
            var endPointConnector = await _endPointConnectorFactory.Create(endPointConfig, _requestContext);

            _logger.LogDebug("Get Artefact");
            var containerArtefact = await endPointConnector.GetArtefactAsync(request.Type, request.Id,
                request.RefDetail, request.RespDetail,
                true, true);

            _logger.LogDebug("End Handle");

            return new GetArtefactResponse {ArtefactContainer = containerArtefact};
        }
    }
}