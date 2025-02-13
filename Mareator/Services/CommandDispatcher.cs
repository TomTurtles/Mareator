namespace Mareator;
public sealed class CommandDispatcher : ICommandDispatcher
{
    private readonly Dictionary<Type, object> _handlers = new();

    public CommandDispatcher(IEnumerable<ICommandHandler> handlers)
    {
        // Discover any ICommandHandler<T> interfaces implemented by each handler
        foreach (var handler in handlers)
        {
            var handlerInterfaces = handler.GetType().GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<>))
                .ToList();

            foreach (var intf in handlerInterfaces)
            {
                var commandType = intf.GetGenericArguments()[0];

                if (_handlers.ContainsKey(commandType))
                {
                    throw new InvalidOperationException(
                        $"A handler for command type {commandType.Name} is already registered."
                    );
                }

                _handlers.Add(commandType, handler);
            }
        }
    }

    public async Task RunAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        var commandType = typeof(TCommand);

        if (!_handlers.TryGetValue(commandType, out var handlerObj))
        {
            throw new InvalidOperationException(
                $"No async command handler registered for command type {commandType.Name}."
            );
        }

        if (cancellationToken.IsCancellationRequested) return;

        // Cast to the appropriate async command handler and invoke
        var handler = (ICommandHandler<TCommand>)handlerObj;
        await handler.HandleAsync(command, cancellationToken);
    }
}
