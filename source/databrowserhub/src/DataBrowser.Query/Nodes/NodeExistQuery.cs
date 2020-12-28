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
    public class NodeExistQuery : NodeQueryBase, IQuery<bool>
    {
        public NodeExistQuery(int nodeId,
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

        public class NodeExistHandler : IRequestHandler<NodeExistQuery, bool>
        {
            private readonly IFilterNode _filterNode;
            private readonly ILogger<NodeExistHandler> _logger;
            private readonly IRepository<Node> _repository;

            public NodeExistHandler(ILogger<NodeExistHandler> logger,
                IRepository<Node> repository,
                IFilterNode filterNode)
            {
                _logger = logger;
                _repository = repository;
                _filterNode = filterNode;
            }

            public async Task<bool> Handle(NodeExistQuery request, CancellationToken cancellationToken)
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


                _logger.LogDebug("END");
                return true;
            }
        }
    }
}