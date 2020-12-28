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
using DataBrowser.Services.Interfaces;
using DataBrowser.Specifications.Nodes;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DataBrowser.Query.Nodes
{
    public class AllNodesWithMinimalInfoQuery : NodeQueryBase, IQuery<IReadOnlyList<NodeMinimalInfoDto>>
    {
        public AllNodesWithMinimalInfoQuery(bool filterByPermissionNodeConfig = false,
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
            AllNodesWithMinimalInfoHandler : IRequestHandler<AllNodesWithMinimalInfoQuery,
                IReadOnlyList<NodeMinimalInfoDto>>
        {
            private readonly IFilterNode _filterNode;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly ILogger<AllNodesWithMinimalInfoHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IMediatorService _mediatorService;
            private readonly IRepository<Node> _repository;

            public AllNodesWithMinimalInfoHandler(ILogger<AllNodesWithMinimalInfoHandler> logger,
                IRepository<Node> repository,
                IMapper mapper,
                IFilterNode filterNode,
                IMediatorService mediatorService,
                IHttpContextAccessor httpContextAccessor)
            {
                _logger = logger;
                _repository = repository;
                _mapper = mapper;
                _filterNode = filterNode;
                _mediatorService = mediatorService;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<IReadOnlyList<NodeMinimalInfoDto>> Handle(AllNodesWithMinimalInfoQuery request,
                CancellationToken cancellationToken)
            {
                _logger.LogDebug("START");

                var allNodesRepository = await _repository.FindAsync(new NodeByIdWithDataSpecification());

                var allNodes = allNodesRepository.Select(i => i.ConvertToNodeDataView(_mapper)).ToList();

                var nodes = new List<NodeMinimalInfoDto>();
                if (_filterNode != null)
                    foreach (var itemNode in allNodes)
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
                    nodes = allNodes;

                _logger.LogDebug("END");
                return nodes.AsReadOnly();
            }
        }
    }
}