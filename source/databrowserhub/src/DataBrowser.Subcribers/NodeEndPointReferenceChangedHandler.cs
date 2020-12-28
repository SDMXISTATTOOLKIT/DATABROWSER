using DataBrowser.Domain.Entities.Nodes;
using DataBrowser.Interfaces.Cache;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DataBrowser.Subcribers
{
    public class NodeEndPointReferenceChangedHandler : INotificationHandler<NodeEndPointReferenceChangedPublicEvent>
    {
        private readonly IDataBrowserMemoryCache _dataBrowserMemoryCache;
        private readonly IDataflowDataCache _dataflowDataCache;
        //private readonly IEndPointCache _endPointCache;
        private readonly ILogger _logger;

        public NodeEndPointReferenceChangedHandler(ILogger<NodeEndPointReferenceChangedHandler> logger,
                                                    IDataBrowserMemoryCache dataBrowserMemoryCache,
                                                    IDataflowDataCache dataflowDataCache)
        {
            _logger = logger;
            _dataBrowserMemoryCache = dataBrowserMemoryCache;
            _dataflowDataCache = dataflowDataCache;
        }

        public Task Handle(NodeEndPointReferenceChangedPublicEvent notification,
            CancellationToken cancellationToken)
        {
            _logger.LogDebug("START NodeEndPointReferenceChangedPublicEvent");

            //if (_endPointCache != null)
            //{
            //    if (!notification.NodeCode.Equals(notification.PreviusNodeCode,
            //        StringComparison.InvariantCultureIgnoreCase))
            //        await _endPointCache.ClearCacheAsync(new List<string>
            //            {notification.NodeCode, notification.PreviusNodeCode});
            //    else
            //        await _endPointCache.ClearCacheAsync(new List<string> {notification.NodeCode});
            //}

            if (_dataBrowserMemoryCache != null)
            {
                _logger.LogDebug("clear memory cache");
                _dataBrowserMemoryCache.ClearCacheAsync(new List<int> { notification.NodeId });
            }
            if (_dataflowDataCache != null)
            {
                _logger.LogDebug("clear dataflow data cache");
                _dataflowDataCache.ClearCacheDataflowDataAsync(notification.NodeId);
            }

            _logger.LogDebug("END NodeEndPointReferenceChangedPublicEvent");

            return Task.CompletedTask;
        }
    }
}