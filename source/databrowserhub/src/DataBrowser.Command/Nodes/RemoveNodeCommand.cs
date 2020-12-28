using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DataBrowser.Domain.Entities.Nodes;
using DataBrowser.Domain.Events;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Interfaces.Mediator;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataBrowser.Command.Nodes
{
    public class RemoveNodeCommand : ICommand<bool>
    {
        public RemoveNodeCommand(int nodeId)
        {
            NodeId = nodeId;
        }

        public int NodeId { get; set; }

        public class RemoveNodeHandler : IRequestHandler<RemoveNodeCommand, bool>
        {
            private readonly ILogger<RemoveNodeHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IRepository<Node> _repository;

            public RemoveNodeHandler(ILogger<RemoveNodeHandler> logger,
                IMapper mapper,
                IRepository<Node> repository)
            {
                _logger = logger;
                _mapper = mapper;
                _repository = repository;
            }

            public async Task<bool> Handle(RemoveNodeCommand request, CancellationToken cancellationToken)
            {
                var entity = await _repository.GetByIdAsync(request.NodeId);
                if (entity != null)
                {
                    entity.AddDomainEvent(new NodeEndPointReferenceChangedEvent(request.NodeId, "", null));
                    entity.AddDomainEvent(new NodeRemovedPublicEvent(request.NodeId));
                    entity.AddDomainEvent(new ImageChangePublicEvent(this.GetType().Name, entity.BackgroundMediaURL, ""));
                    entity.AddDomainEvent(new ImageChangePublicEvent(this.GetType().Name, entity.Logo, ""));
                    _repository.Delete(entity);

                    await _repository.UnitOfWork.SaveChangesAsync();

                    return true;
                }

                return false;
            }
        }
    }
}