using DataBrowser.AC.Utility;
using DataBrowser.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataBrowser.Subcribers
{
    public class ImageChangedHandler : INotificationHandler<ImageChangePublicEvent>
    {
        private readonly ILogger _logger;

        public ImageChangedHandler(ILogger<ImageChangedHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(ImageChangePublicEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogDebug("START ImageChangedHandler");

            if (!string.IsNullOrWhiteSpace(notification.OldFilePath))
            {
                _logger.LogDebug($"delete {notification.OldFilePath}");

                try
                {
                    System.IO.File.Delete(DataBrowserDirectory.GetRootStorageFolder() + "/" + notification.OldFilePath);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Some error during remove old image", ex);
                }
            }

            _logger.LogDebug("END ImageChangedHandler");
        }

    }
}
