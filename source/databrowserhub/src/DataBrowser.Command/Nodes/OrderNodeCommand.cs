using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataBrowser.Domain.Entities.Nodes;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Interfaces.Mediator;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataBrowser.Command.Nodes
{
    public class OrderNodeCommand : ICommand<bool>
    {
        public OrderNodeCommand(List<int> orderNodes)
        {
            OrderNodes = orderNodes;
        }

        public List<int> OrderNodes { get; set; }

        public class OrderNodeHandler : IRequestHandler<OrderNodeCommand, bool>
        {
            private readonly ILogger<OrderNodeHandler> _logger;
            private readonly IRepository<Node> _repository;

            public OrderNodeHandler(ILogger<OrderNodeHandler> logger,
                IRepository<Node> repository)
            {
                _logger = logger;
                _repository = repository;
            }

            public async Task<bool> Handle(OrderNodeCommand request, CancellationToken cancellationToken)
            {
                _logger.LogDebug("START");

                var allNodes = await _repository.ListAllAsync();
                var orderIndex = 1;
                foreach (var nodeIdFind in request.OrderNodes)
                {
                    var node = allNodes.FirstOrDefault(i => i.NodeId == nodeIdFind);
                    if (node == null) continue;

                    node.Order = orderIndex;
                    _repository.Update(node);
                    orderIndex++;
                }

                _logger.LogDebug("SaveEntitiesAsync");
                await _repository.UnitOfWork.SaveChangesAsync();


                _logger.LogDebug("END");
                return true;
            }
        }
    }
}