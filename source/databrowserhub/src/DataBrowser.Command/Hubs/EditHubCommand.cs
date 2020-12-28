using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DataBrowser.AC.Exceptions;
using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.Hubs;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Interfaces.Mediator;
using DataBrowser.Specifications.Hubs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataBrowser.Command.Hubs
{
    public class EditHubCommand : ICommand<bool>
    {
        public EditHubCommand(HubDto hubDto)
        {
            if (hubDto.HubId <= 0) throw new ArgumentNullException(nameof(hubDto.HubId));
            Hub = hubDto;
        }

        public HubDto Hub { get; set; }

        public class EditHubHandler : IRequestHandler<EditHubCommand, bool>
        {
            private readonly ILogger<EditHubHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IRepository<Hub> _repository;

            public EditHubHandler(ILogger<EditHubHandler> logger,
                IMapper mapper,
                IRepository<Hub> repository)
            {
                _logger = logger;
                _mapper = mapper;
                _repository = repository;
            }

            public async Task<bool> Handle(EditHubCommand request, CancellationToken cancellationToken)
            {
                _logger.LogDebug("START");
                var hubs = await _repository.FindAsync(new HubByIdWithAllDataSpecification(request.Hub.HubId));
                var hub = hubs.FirstOrDefault();
                if (hub == null) new ClientErrorException("NODE_NOT_FOUND", $"Hub {request.Hub.HubId} not found");

                hub.EditHub(request.Hub);

                _logger.LogDebug("edit to repository");
                _repository.Update(hub);

                _logger.LogDebug("SaveChangeAsync");
                await _repository.UnitOfWork.SaveChangesAsync();

                _logger.LogDebug("END");
                return true;
            }
        }
    }
}