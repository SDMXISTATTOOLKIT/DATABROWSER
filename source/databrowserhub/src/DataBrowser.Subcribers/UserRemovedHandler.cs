using System.Threading;
using System.Threading.Tasks;
using DataBrowser.Domain.Entities.Users;
using DataBrowser.Domain.Entities.ViewTemplates;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Specifications.ViewTemplates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataBrowser.Subcribers
{
    public class UserRemovedHandler : INotificationHandler<UserRemovedPublicEvent>
    {
        private readonly ILogger _logger;
        private readonly IRepository<ViewTemplate> _repositoryViewTemplate;

        public UserRemovedHandler(ILogger<NodeEndPointReferenceChangedHandler> logger,
            IRepository<ViewTemplate> repositoryViewTemplate)
        {
            _logger = logger;
            _repositoryViewTemplate = repositoryViewTemplate;
        }

        public async Task Handle(UserRemovedPublicEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogDebug("START UserRemovedHandler");

            if (_repositoryViewTemplate != null)
            {
                var list = await _repositoryViewTemplate.FindAsync(new ViewByUserIdSpecification(notification.UserId));
                foreach (var item in list) _repositoryViewTemplate.Delete(item);
                await _repositoryViewTemplate.UnitOfWork.SaveChangesAsync();
            }

            _logger.LogDebug($"END {GetType().Name}");
        }
    }
}