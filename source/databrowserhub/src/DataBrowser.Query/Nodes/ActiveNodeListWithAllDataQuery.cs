using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.Nodes;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Interfaces.Authentication;
using DataBrowser.Interfaces.Mediator;
using DataBrowser.Specifications.Nodes;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataBrowser.Query.Nodes
{
    public class ActiveNodeListWithAllDataQuery : NodeQueryBase, IQuery<IReadOnlyList<NodeDto>>
    {
        public ActiveNodeListWithAllDataQuery(bool filterByPermissionNodeConfig = false,
            bool filterByPermissionNodeView = false,
            bool filterByPermissionNodeTemplate = false,
            bool filterByPermissionNodeCache = false,
            bool filterIsInAnd = true,
            ClaimsPrincipal filterBySpecificUser = null) :
            base(filterByPermissionNodeConfig, filterByPermissionNodeView, filterByPermissionNodeTemplate,
                filterByPermissionNodeCache, filterIsInAnd, filterBySpecificUser)
        {
        }

        public class
            ActiveNodeListWithAllDataHandler : IRequestHandler<ActiveNodeListWithAllDataQuery, IReadOnlyList<NodeDto>>
        {
            private readonly IFilterNode _filterNode;
            private readonly ILogger<ActiveNodeListWithAllDataHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IRepository<Node> _repository;

            public ActiveNodeListWithAllDataHandler(ILogger<ActiveNodeListWithAllDataHandler> logger,
                IRepository<Node> repository,
                IMapper mapper,
                IFilterNode filterNode)
            {
                _logger = logger;
                _repository = repository;
                _mapper = mapper;
                _filterNode = filterNode;
            }

            public async Task<IReadOnlyList<NodeDto>> Handle(ActiveNodeListWithAllDataQuery request,
                CancellationToken cancellationToken)
            {
                _logger.LogDebug("START");

                var allNodes = await _repository.FindAsync(new NodeByIdWithDataSpecification());

                var nodesActive = allNodes.Where(i => i.Active).Select(i => _mapper.Map<NodeDto>(i)).ToList();

                var nodes = new List<NodeDto>();
                if (_filterNode != null)
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
                else
                    nodes = nodesActive;

                _logger.LogDebug("END");
                return nodes.AsReadOnly();
            }
        }
    }
}