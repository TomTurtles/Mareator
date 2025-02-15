namespace Mareator;

public sealed class Mareator : IMareator
{
    private readonly IMareatorEventDispatcher _eventDispatcher;
    private readonly IMareatorCommandDispatcher _commandDispatcher;
    private readonly IMareatorRequestDispatcher _requestDispatcher;

    public Mareator(
        IMareatorEventDispatcher eventDispatcher,
        IMareatorCommandDispatcher commandDispatcher,
        IMareatorRequestDispatcher requestDispatcher)
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
        await _commandDispatcher.RunAsync(command, cancellationToken);
    }
    public async Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default) 
        where TRequest : IRequest<TResponse>
    {
        return await _requestDispatcher.RequestAsync<TRequest, TResponse>(request, cancellationToken);
    }
}