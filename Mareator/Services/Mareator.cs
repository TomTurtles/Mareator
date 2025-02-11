
namespace Mareator;

public sealed class Mareator(IEventDispatcher eventDispatcher, ICommandDispatcher commandDispatcher) : IMareator
{
    private readonly IEventDispatcher _eventDispatcher = eventDispatcher;
    private readonly ICommandDispatcher _commandDispatcher = commandDispatcher;

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
}