using DataBrowser.Domain.Entities.Hubs;
using DataBrowser.Interfaces.Cache;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace DataBrowser.Subcribers
{
    public class
        HubDataflowDataParamiterChangedHandler : INotificationHandler<HubDataflowDataParamiterChangedPublicEvent>
    {
        private readonly IDataBrowserMemoryCache _dataBrowserMemoryCache;
        private readonly ILogger _logger;

        public HubDataflowDataParamiterChangedHandler(ILogger<HubDataflowDataParamiterChangedHandler> logger,
            IDataBrowserMemoryCache dataBrowserMemoryCache)
        {
            _logger = logger;
            _dataBrowserMemoryCache = dataBrowserMemoryCache;
        }

        public Task Handle(HubDataflowDataParamiterChangedPublicEvent notification,
            CancellationToken cancellationToken)
        {
            _logger.LogDebug("START HubDataflowDataParamiterChangedHandler");

            if (_dataBrowserMemoryCache != null) _dataBrowserMemoryCache.ClearCacheAsync();


            _logger.LogDebug("END");

            return Task.CompletedTask;
        }
    }
}