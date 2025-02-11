
namespace Mareator;

public sealed class Mareator : IMareator
{
    private readonly IEventDispatcher _eventDispatcher;
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IRequestDispatcher _requestDispatcher;

    public Mareator(
        IEventDispatcher eventDispatcher,
        ICommandDispatcher commandDispatcher,
        IRequestDispatcher requestDispatcher)
    {
        _eventDispatcher = eventDispatcher;
        _commandDispatcher = commandDispatcher;
        _requestDispatcher = requestDispatcher;
    }

    public void Subscribe<TEventArgs>(EventHandler<TEventArgs> handler)
        where TEventArgs : EventArgs
    {
        _eventDispatcher.Subscribe(handler);
    }

    public void Unsubscribe<TEventArgs>(EventHandler<TEventArgs> handler)
        where TEventArgs : EventArgs
    {
        _eventDispatcher.Unsubscribe(handler);
    }

    public void Publish<TEventArgs>(object sender, TEventArgs eventArgs)
        where TEventArgs : EventArgs
    {
        _eventDispatcher.Publish(sender, eventArgs);
    }

    public async Task RunAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand
    {
        await _commandDispatcher.RunAsync(command);
    }
    public async Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default) 
        where TRequest : IRequest<TResponse>
    {
        return await _requestDispatcher.RequestAsync<TRequest, TResponse>(request, cancellationToken);
    }
}