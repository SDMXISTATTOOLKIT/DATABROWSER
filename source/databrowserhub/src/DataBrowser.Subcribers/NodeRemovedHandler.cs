using System.Threading;
using System.Threading.Tasks;
using DataBrowser.Domain.Entities.Nodes;
using DataBrowser.Domain.Entities.ViewTemplates;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Authentication;
using DataBrowser.Interfaces.Cache;
using DataBrowser.Specifications.ViewTemplates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataBrowser.Subcribers
{
    public class NodeRemovedHandler : INotificationHandler<NodeRemovedPublicEvent>
    {
        private readonly IClearAuthDb _clearAuthDb;
        private readonly IDataflowDataCache _dataflowDataCache;
        private readonly ILogger _logger;
        private readonly IRepository<ViewTemplate> _repositoryViewTemplate;
        private readonly IUserService _userService;

        public NodeRemovedHandler(ILogger<NodeEndPointReferenceChangedHandler> logger,
            IUserService userService,
            IRepository<ViewTemplate> repositoryViewTemplate,
            IDataflowDataCache dataflowDataCache,
            IClearAuthDb clearAuthDb)
        {
            _logger = logger;
            _userService = userService;
            _repositoryViewTemplate = repositoryViewTemplate;
            _clearAuthDb = clearAuthDb;
            _dataflowDataCache = dataflowDataCache;
        }

        public async Task Handle(NodeRemovedPublicEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogDebug("START NodeRemovedHandler");

            if (_userService != null) await _userService.RemoveAllNodePermissionAsync(notification.NodeId);

            if (_repositoryViewTemplate != null)
            {
                var list = await _repositoryViewTemplate.FindAsync(
                    new ViewTemplateByNodeIdSpecification(notification.NodeId));
                foreach (var item in list) _repositoryViewTemplate.Delete(item);
                await _repositoryViewTemplate.UnitOfWork.SaveChangesAsync();
            }

            if (_dataflowDataCache != null) _dataflowDataCache.DropDatabase(notification.NodeId);

            clearAuthDb(notification.NodeId);

            _logger.LogDebug("END NodeRemovedHandler");
        }


        private void clearAuthDb(int nodeId)
        {
            if (_clearAuthDb == null)
            {
                _logger.LogInformation("clearAuthDb exit with null");
                return;
            }

            _clearAuthDb.ClearAfterNodeRemove(nodeId);
        }
    }
}