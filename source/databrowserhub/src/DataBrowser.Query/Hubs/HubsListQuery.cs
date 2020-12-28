using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DataBrowser.AC.Utility;
using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.Hubs;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Interfaces.Mediator;
using DataBrowser.Specifications.Hubs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataBrowser.Query.Hubs
{
    public class HubsListQuery : IQuery<IReadOnlyList<HubDto>>
    {
        public HubsListQuery()
        {
            HubId = -1;
        }

        public HubsListQuery(int hubId)
        {
            HubId = hubId;
        }

        public int HubId { get; set; }

        public class HubsListHandler : IRequestHandler<HubsListQuery, IReadOnlyList<HubDto>>
        {
            private readonly ILogger<HubsListHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IRepository<Hub> _repository;

            public HubsListHandler(ILogger<HubsListHandler> logger,
                IRepository<Hub> repository,
                IMapper mapper)
            {
                _logger = logger;
                _repository = repository;
                _mapper = mapper;
            }

            public async Task<IReadOnlyList<HubDto>> Handle(HubsListQuery request, CancellationToken cancellationToken)
            {
                _logger.LogDebug("START");

                var allHubs =
                    await _repository.FindAsync(
                        new HubByIdWithAllDataSpecification(request.HubId, excludeDescription: false));

                var hubs = new List<HubDto>();
                foreach (var item in allHubs)
                {
                    _logger.LogDebug($"Node {item.HubId}");
                    hubs.Add(item.ConvertToHubDto(_mapper));
                }

                _logger.LogDebug("END");
                return hubs.AsReadOnly();
            }
        }
    }
}