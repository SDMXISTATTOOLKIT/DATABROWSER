using System.Threading.Tasks;
using DataBrowser.DomainServices.Interfaces;
using DataBrowser.Interfaces;
using EndPointConnector.Interfaces;
using EndPointConnector.Interfaces.Sdmx;
using EndPointConnector.Interfaces.Spod;
using Microsoft.Extensions.Logging;

namespace DataBrowser.AC.EndPointConnector
{
    public class EndPointConnectorFactory : IEndPointConnectorFactory
    {
        private readonly ILogger<EndPointConnectorFactory> _logger;
        private readonly IRequestContext _requestContext;
        private readonly ISdmxEndPointFactory _sdmxEndPointFactory;
        private readonly ISpodEndPointFactory _spodEndPointFactory;

        public EndPointConnectorFactory(ILogger<EndPointConnectorFactory> logger, IRequestContext requestContext,
            ISdmxEndPointFactory sdmxEndPointFactory, ISpodEndPointFactory spodEndPointFactory)
        {
            _logger = logger;
            _requestContext = requestContext;
            _sdmxEndPointFactory = sdmxEndPointFactory;
            _spodEndPointFactory = spodEndPointFactory;
        }

        public async Task<IEndPointConnector> Create(INodeConfiguration nodeConfig, IRequestContext requestContext)
        {
            //if (endPointConfig.Type == "")
            //{
            //    return await _spodEndPointFactory.CreateConnector(endPointConfig, SpodEndPointCostant.ConnectorType.FromConfig);
            //}
            //else
            //{
            _logger.LogDebug("Create SDMX Connector");

            return await _sdmxEndPointFactory.CreateConnector(new EndPointConfig(nodeConfig, requestContext));
            //}
        }
    }
}