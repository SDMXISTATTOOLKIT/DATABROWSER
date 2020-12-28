using System.Threading;
using System.Threading.Tasks;
using DataBrowser.Services.Interfaces;
using MediatR;

namespace DataBrowser.Services
{
    public class MediatorService : IMediatorService
    {
        private readonly IMediator _mediator;

        public MediatorService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<TResult> QueryAsync<TResult>(IRequest<TResult> query,
            CancellationToken cancellationToken = default)
        {
            return await _mediator.Send(query);
        }

        public async Task<TResult> CommandAsync<TResult>(IRequest<TResult> command,
            CancellationToken cancellationToken = default)
        {
            return await _mediator.Send(command);
        }

        public async Task Publish(object notification, CancellationToken cancellationToken = default)
        {
            await _mediator.Publish(notification, cancellationToken);
        }

        public async Task Publish<TNotification>(TNotification notification,
            CancellationToken cancellationToken = default) where TNotification : INotification
        {
            await _mediator.Publish(notification, cancellationToken);
        }

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request,
            CancellationToken cancellationToken = default)
        {
            return await _mediator.Send(request, cancellationToken);
        }

        public async Task<object> Send(object request, CancellationToken cancellationToken = default)
        {
            return await _mediator.Send(request, cancellationToken);
        }
    }
}