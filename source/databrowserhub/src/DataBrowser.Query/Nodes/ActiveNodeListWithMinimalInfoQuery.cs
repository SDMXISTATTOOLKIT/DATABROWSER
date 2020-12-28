using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DataBrowser.AC.Responses.Services;
using DataBrowser.AC.Utility;
using DataBrowser.Domain.Entities.Nodes;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Interfaces.Authentication;
using DataBrowser.Interfaces.Mediator;
using DataBrowser.Specifications.Nodes;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataBrowser.Query.Nodes
{
    public class ActiveNodeListWithMinimalInfoQuery : NodeQueryBase, IQuery<IReadOnlyList<NodeMinimalInfoDto>>
    {
        public ActiveNodeListWithMinimalInfoQuery(bool filterByPermissionNodeConfig = false,
            bool filterByPermissionNodeView = false,
            bool filterByPermissionNodeTemplate = false,
            bool filterByPermissionNodeCache = false,
            bool filterIsInAnd = true,
            ClaimsPrincipal filterBySpecificUser = null) :
            base(filterByPermissionNodeConfig, filterByPermissionNodeView, filterByPermissionNodeTemplate,
                filterByPermissionNodeCache, filterIsInAnd, filterBySpecificUser)
        {
        }

        public class ActiveNodeListWithMinimalInfoHandler : IRequestHandler<ActiveNodeListWithMinimalInfoQuery,
            IReadOnlyList<NodeMinimalInfoDto>>
        {
            private readonly IFilterNode _filterNode;
            private readonly ILogger<ActiveNodeListWithMinimalInfoHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IRepository<Node> _repository;

            public ActiveNodeListWithMinimalInfoHandler(ILogger<ActiveNodeListWithMinimalInfoHandler> logger,
                IRepository<Node> repository,
                IMapper mapper,
                IFilterNode filterNode)
            {
                _logger = logger;
                _repository = repository;
                _mapper = mapper;
                _filterNode = filterNode;
            }

            public async Task<IReadOnlyList<NodeMinimalInfoDto>> Handle(ActiveNodeListWithMinimalInfoQuery request,
                CancellationToken cancellationToken)
            {
                _logger.LogDebug("START");

                var allNodes = await _repository.FindAsync(new NodeByIdWithDataSpecification());

                var nodesActive = allNodes.Where(i => i.Active).Select(i => i.ConvertToNodeDataView(_mapper)).ToList();

                var nodes = new List<NodeMinimalInfoDto>();
                if (_filterNode != null)
                {
                    _logger.LogDebug("Check permission for nodes");
                    foreach (var itemNode in nodesActive)
                    {
                        var havePermission =
                            NodesHandlerUtility.CheckPermissionNode(request, itemNode.NodeId, _filterNode, _logger);
                        if (!havePermission)
                        {
                            _logger.LogDebug($"Check user haven't permission for node {itemNode.NodeId}");
                            continue;
                        }

                        nodes.Add(itemNode);
                    }
                }
                else
                {
                    nodes = nodesActive;
                }

                _logger.LogDebug("END");
                return nodes.AsReadOnly();
            }
        }
    }
}