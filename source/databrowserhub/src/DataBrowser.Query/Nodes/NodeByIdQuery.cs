using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DataBrowser.AC.Utility;
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
    public class NodeByIdQuery : NodeQueryBase, IQuery<NodeDto>
    {
        public NodeByIdQuery(int nodeId,
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

        public NodeByIdQuery(string nodeCode,
            bool filterByPermissionNodeConfig = false,
            bool filterByPermissionNodeView = false,
            bool filterByPermissionNodeTemplate = false,
            bool filterByPermissionNodeCache = false,
            bool filterIsInAnd = true,
            ClaimsPrincipal filterBySpecificUser = null) :
            base(filterByPermissionNodeConfig, filterByPermissionNodeView, filterByPermissionNodeTemplate,
                filterByPermissionNodeCache, filterIsInAnd, filterBySpecificUser)
        {
        }

        public int NodeId { get; set; }
        public string NodeCode { get; set; }

        class NodeByIdHandler : IRequestHandler<NodeByIdQuery, NodeDto>
        {
            private readonly IFilterNode _filterNode;
            private readonly ILogger<NodeByIdHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IRepository<Node> _repository;

            public NodeByIdHandler(ILogger<NodeByIdHandler> logger,
                IRepository<Node> repository,
                IMapper mapper,
                IFilterNode filterNode)
            {
                _logger = logger;
                _repository = repository;
                _mapper = mapper;
                _filterNode = filterNode;
            }

            public async Task<NodeDto> Handle(NodeByIdQuery request, CancellationToken cancellationToken)
            {
                _logger.LogDebug("START");

                IReadOnlyList<Node> nodeEntities;
                if (string.IsNullOrWhiteSpace(request.NodeCode))
                    nodeEntities = await _repository.FindAsync(new NodeByIdWithDataSpecification(request.NodeId));
                else
                    nodeEntities = await _repository.FindAsync(new NodeByCodeSpecification(request.NodeCode,
                        NodeByCodeSpecification.ExtraInclude.ExtraWithTransaltion));

                var nodeEntity = nodeEntities.FirstOrDefault();
                if (nodeEntity == null)
                {
                    _logger.LogDebug("Node not found");
                    return null;
                }

                var returnDto = nodeEntity.ConvertToNodeDto(_mapper);

                if (_filterNode != null)
                {
                    var havePermission =
                        NodesHandlerUtility.CheckPermissionNode(request, nodeEntity.NodeId, _filterNode, _logger);
                    if (!havePermission)
                    {
                        _logger.LogDebug("Check user haven't permission");
                        return null;
                    }
                }

                _logger.LogDebug("END");
                return returnDto;
            }
        }
    }
}