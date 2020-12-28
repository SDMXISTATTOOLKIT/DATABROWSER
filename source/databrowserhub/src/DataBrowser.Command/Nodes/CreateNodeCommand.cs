using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DataBrowser.AC.Exceptions;
using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.Nodes;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Interfaces.Mediator;
using DataBrowser.Specifications.Nodes;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataBrowser.Command.Nodes
{
    public class CreateNodeCommand : ICommand<int>
    {
        public CreateNodeCommand(NodeDto nodeDto)
        {
            Node = nodeDto;
        }

        public NodeDto Node { get; set; }

        class CreateNodeHandler : IRequestHandler<CreateNodeCommand, int>
        {
            private readonly ILogger<CreateNodeHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IRepository<Node> _repository;

            public CreateNodeHandler(ILogger<CreateNodeHandler> logger,
                IMapper mapper,
                IRepository<Node> repository)
            {
                _logger = logger;
                _mapper = mapper;
                _repository = repository;
            }

            public async Task<int> Handle(CreateNodeCommand request, CancellationToken cancellationToken)
            {
                _logger.LogDebug("START");

                var nodeExist = await _repository.FindAsync(new NodeByCodeSpecification(request.Node.Code,
                    NodeByCodeSpecification.ExtraInclude.Nothing));

                if (nodeExist != null && nodeExist.Count > 0)
                    throw new ClientErrorException("NODE_DUPLICATE_CODE",
                        $"Node code '{request.Node.Code}' is already present");

                var maxPosition = 1;
                var maxPositionNode = await _repository.FindAsync(new NodePositionHighSpecification());
                if (maxPositionNode?.FirstOrDefault() != null)
                {
                    maxPosition = maxPositionNode.First().Order;
                    if (maxPosition < Int32.MaxValue)
                    {
                        maxPosition++;
                    }
                }

                request.Node.Order = maxPosition;
                var nodeEntity = Domain.Entities.Nodes.Node.CreateNode(request.Node);

                _logger.LogDebug("add to repository");
                _repository.Add(nodeEntity);

                _logger.LogDebug("SaveChangeAsync");
                var result = await _repository.UnitOfWork.SaveChangesAsync() > 0;

                if (!result) return -1;

                _logger.LogDebug("END");
                return nodeEntity.NodeId;
            }
        }
    }
}