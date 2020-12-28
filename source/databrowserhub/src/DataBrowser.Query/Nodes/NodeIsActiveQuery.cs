using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using DataBrowser.Domain.Entities.Nodes;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Interfaces.Authentication;
using DataBrowser.Interfaces.Mediator;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataBrowser.Query.Nodes
{
    public class NodeIsActiveQuery : NodeQueryBase, IQuery<bool>
    {
        public NodeIsActiveQuery(int nodeId,
            bool filterByPermissionNodeConfig = false,
            bool filterByPermissionNodeView = false,
            bool filterByPermissionNodeTemplate = false,
            bool filterByPermissionNodeCache = false,
            bool filterIsInAnd = true,
            ClaimsPrincipal filterBySpecificUser = null) :
            base(filterByPermissionNodeConfig, filterByPermissionNodeView, filterByPermissionNodeTemplate,
                filterByPermissionNodeCache, filterIsInAnd, filterBySpecificUser)
        {
            NodeId = nodeId;
        }

        public int NodeId { get; set; }

        public class NodeIsActiveHandler : IRequestHandler<NodeIsActiveQuery, bool>
        {
            private readonly IFilterNode _filterNode;
            private readonly ILogger<NodeIsActiveHandler> _logger;
            private readonly IRepository<Node> _repository;

            public NodeIsActiveHandler(ILogger<NodeIsActiveHandler> logger,
                IRepository<Node> repository,
                IFilterNode filterNode)
            {
                _logger = logger;
                _repository = repository;
                _filterNode = filterNode;
            }

            public async Task<bool> Handle(NodeIsActiveQuery request, CancellationToken cancellationToken)
            {
                _logger.LogDebug("START");

                var nodeEntity = await _repository.GetByIdAsync(request.NodeId);

                if (nodeEntity == null)
                {
                    _logger.LogDebug("Node not found");
                    return false;
                }

                if (_filterNode != null)
                {
                    var havePermission =
                        NodesHandlerUtility.CheckPermissionNode(request, nodeEntity.NodeId, _filterNode, _logger);
                    if (!havePermission)
                    {
                        _logger.LogDebug("Check user haven't permission");
                        return false;
                    }
                }


                var result = nodeEntity.Active;

                _logger.LogDebug("END");
                return result;
            }
        }
    }
}