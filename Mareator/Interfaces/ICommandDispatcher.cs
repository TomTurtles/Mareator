namespace Mareator;
public interface ICommandDispatcher
{
    /// <summary>
    /// Runs the command by invoking the appropriate async command handler.
    /// </summary>
    /// <typeparam name="TCommand">The command type.</typeparam>
    /// <param name="command">The command to run.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    Task RunAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand;
}
