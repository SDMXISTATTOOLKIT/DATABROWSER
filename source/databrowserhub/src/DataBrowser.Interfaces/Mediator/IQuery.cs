using MediatR;

namespace DataBrowser.Interfaces.Mediator
{
    public interface IQuery<out TResult> : IRequest<TResult>
    {
    }
}