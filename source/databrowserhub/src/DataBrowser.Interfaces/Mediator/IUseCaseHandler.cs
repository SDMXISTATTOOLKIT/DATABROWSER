using MediatR;

namespace DataBrowser.Interfaces.Mediator
{
    public interface IUseCaseHandler<in TUseCase> :
        IRequestHandler<TUseCase> where TUseCase : ICommand
    {
    }

    public interface IUseCaseHandler<in TUseCase, TResult> :
        IRequestHandler<TUseCase, TResult> where TUseCase : IUseCase<TResult>
    {
    }
}