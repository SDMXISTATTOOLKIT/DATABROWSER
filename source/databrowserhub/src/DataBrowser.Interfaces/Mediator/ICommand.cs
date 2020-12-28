using MediatR;

namespace DataBrowser.Interfaces.Mediator
{
    public interface ICommand : IRequest
    {
    }

    public interface ICommand<out TResult> : IRequest<TResult>
    {
    }
}