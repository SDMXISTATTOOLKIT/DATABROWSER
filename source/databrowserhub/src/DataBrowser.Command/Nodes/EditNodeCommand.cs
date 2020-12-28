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
    public class EditNodeCommand : ICommand<bool>
    {
        public EditNodeCommand(NodeDto nodeDto)
        {
            if (nodeDto.NodeId <= 0) throw new ArgumentNullException(nameof(nodeDto.NodeId));
            Node = nodeDto;
        }

        public NodeDto Node { get; set; }

        public class EditNodeHandler : IRequestHandler<EditNodeCommand, bool>
        {
            private readonly ILogger<EditNodeHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IRepository<Node> _repository;

            public EditNodeHandler(ILogger<EditNodeHandler> logger,
                IMapper mapper,
                IRepository<Node> repository)
            {
                _logger = logger;
                _mapper = mapper;
                _repository = repository;
            }

            public async Task<bool> Handle(EditNodeCommand request, CancellationToken cancellationToken)
            {
                _logger.LogDebug("START");
                var nodes = await _repository.FindAsync(new NodeByIdWithDataSpecification(request.Node.NodeId));
                var node = nodes.FirstOrDefault();
                if (node == null)
                    throw new ClientErrorException("NODE_NOT_FOUND", $"Node {request.Node.NodeId} not found");

                if (!request.Node.Code.Equals(node.Code, StringComparison.InvariantCultureIgnoreCase))
                {
                    var nodeExist = await _repository.FindAsync(new NodeByCodeSpecification(request.Node.Code,
                        NodeByCodeSpecification.ExtraInclude.Nothing));

                    if (nodeExist != null && nodeExist.Count > 0)
                        throw new ClientErrorException("NODE_DUPLICATE_CODE",
                            $"Node code '{request.Node.Code}' is already present");
                }


                node.EditNode(request.Node);

                _logger.LogDebug("edit to repository");
                _repository.Update(node);

                _logger.LogDebug("SaveChangeAsync");
                await _repository.UnitOfWork.SaveChangesAsync();


                _logger.LogDebug("END");
                return true;
            }
        }
    }
}