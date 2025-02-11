namespace Mareator;

public interface IMareator
{
    /// <summary>
    /// Subscribes a handler for a specific EventArgs type.
    /// </summary>
    /// <typeparam name="TEventArgs">The type of event args to subscribe to.</typeparam>
    /// <param name="handler">The event handler to be added.</param>
    void Subscribe<TEventArgs>(EventHandler<TEventArgs> handler)
        where TEventArgs : EventArgs;

    /// <summary>
    /// Unsubscribes a handler from a specific EventArgs type.
    /// </summary>
    /// <typeparam name="TEventArgs">The type of event args to unsubscribe from.</typeparam>
    /// <param name="handler">The event handler to be removed.</param>
    void Unsubscribe<TEventArgs>(EventHandler<TEventArgs> handler)
        where TEventArgs : EventArgs;

    /// <summary>
    /// Publishes an event for the given EventArgs type.
    /// </summary>
    /// <typeparam name="TEventArgs">The type of event args being published.</typeparam>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="eventArgs">The event data.</param>
    void Publish<TEventArgs>(object sender, TEventArgs eventArgs)
        where TEventArgs : EventArgs;

    /// <summary>
    /// Runs a command by invoking the appropriate command handler.
    /// </summary>
    /// <typeparam name="TCommand">The command type.</typeparam>
    /// <param name="command">An instance of the command to be processed.</param>
    Task RunAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand;

    /// <summary>
    /// Processes a request and returns a response.
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <param name="request">An instance of the request.</param>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    /// <returns>A Task that yields the response.</returns>
    Task<TResponse> RequestAsync<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken = default
    ) where TRequest : IRequest<TResponse>;
}
