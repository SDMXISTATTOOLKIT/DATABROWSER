using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace DataBrowser.Services.Interfaces
{
    public interface IMediatorService
    {
        Task<TResult> QueryAsync<TResult>(IRequest<TResult> query, CancellationToken cancellationToken = default);
        Task<TResult> CommandAsync<TResult>(IRequest<TResult> command, CancellationToken cancellationToken = default);
        Task Publish(object notification, CancellationToken cancellationToken = default);

        Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification;

        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
        Task<object> Send(object request, CancellationToken cancellationToken = default);
    }
}