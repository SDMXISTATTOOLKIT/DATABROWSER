using MediatR;

namespace DataBrowser.Interfaces.Mediator
{
    public interface IUseCase : IRequest
    {
    }

    public interface IUseCase<out TResult> : IRequest<TResult>
    {
    }
}