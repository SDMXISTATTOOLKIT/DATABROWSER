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
    public class ActiveNodeByIdWithMinimalInfoQuery : NodeQueryBase, IQuery<NodeMinimalInfoDto>
    {
        public ActiveNodeByIdWithMinimalInfoQuery(int nodeId,
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

        public class
            ActiveNodeByIdWithMinimalInfoHandler : IRequestHandler<ActiveNodeByIdWithMinimalInfoQuery,
                NodeMinimalInfoDto>
        {
            private readonly IFilterNode _filterNode;
            private readonly ILogger<ActiveNodeByIdWithMinimalInfoHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IRepository<Node> _repository;

            public ActiveNodeByIdWithMinimalInfoHandler(ILogger<ActiveNodeByIdWithMinimalInfoHandler> logger,
                IRepository<Node> repository,
                IMapper mapper,
                IFilterNode filterNode)
            {
                _logger = logger;
                _repository = repository;
                _mapper = mapper;
                _filterNode = filterNode;
            }

            public async Task<NodeMinimalInfoDto> Handle(ActiveNodeByIdWithMinimalInfoQuery request,
                CancellationToken cancellationToken)
            {
                _logger.LogDebug("START");
                var nodeEntities = await _repository.FindAsync(new NodeByIdWithDataSpecification(request.NodeId));
                var nodeEntity = nodeEntities.FirstOrDefault();
                if (nodeEntity == null)
                {
                    _logger.LogDebug("Node not found");
                    return null;
                }

                if (_filterNode != null)
                {
                    var havePermission =
                        NodesHandlerUtility.CheckPermissionNode(request, request.NodeId, _filterNode, _logger);
                    if (!havePermission)
                    {
                        _logger.LogDebug("Check user haven't permission");
                        return null;
                    }
                }

                if (!nodeEntity.Active)
                {
                    _logger.LogDebug("Node not active");
                    return null;
                }

                var returnDto = nodeEntity.ConvertToNodeDto(_mapper).ConvertToNodeDataView(_mapper);

                _logger.LogDebug("END");
                return returnDto;
            }
        }
    }
}