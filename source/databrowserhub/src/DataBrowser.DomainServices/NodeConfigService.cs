using DataBrowser.Domain.Entities.Hubs;
using DataBrowser.Domain.Entities.Nodes;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.DomainServices.Interfaces;
using DataBrowser.DomainServices.Models;
using DataBrowser.Specifications.Nodes;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace DataBrowser.DomainServices
{
    public class NodeConfigService : INodeConfigService
    {
        readonly ILogger<NodeConfigService> _logger;
        readonly IRepository<Node> _repositoryNode;
        readonly IRepository<Hub> _repositoryHub;

        public NodeConfigService(ILogger<NodeConfigService> logger,
                                            IRepository<Node> repositoryNode,
                                            IRepository<Hub> repositoryHub)
        {
            _logger = logger;
            _repositoryNode = repositoryNode;
            _repositoryHub = repositoryHub;
        }

        public async Task<INodeConfiguration> GenerateNodeConfigAsync(string nodeCode)
        {
            _logger.LogDebug($"START generate for nodeCode {nodeCode}");
            var nodes = await _repositoryNode.FindAsync(new NodeByCodeSpecification(nodeCode, NodeByCodeSpecification.ExtraInclude.ExtraWithTransaltion));
            return await generateEndPointConfigAsync(nodes?.FirstOrDefault());
        }

        public async Task<INodeConfiguration> GenerateNodeConfigAsync(int nodeId)
        {
            _logger.LogDebug($"START generate for nodeId {nodeId}");
            var node = await _repositoryNode.GetByIdAsync(nodeId);
            return await generateEndPointConfigAsync(node);
        }

        private async Task<INodeConfiguration> generateEndPointConfigAsync(Node node)
        {
            if (node == null)
            {
                _logger.LogDebug($"Node not found");
                return null;
            }

            var hub = await _repositoryHub.GetByIdAsync(1);
            NodeConfiguration endPointConfig = new NodeConfiguration(node, hub?.MaxObservationsAfterCriteria);

            _logger.LogDebug($"END");
            return endPointConfig;
        }
    }
}
