namespace Mareator;
public interface IEventDispatcher
{
    /// <summary>
    /// Subscribes a handler for a specific EventArgs type.
    /// </summary>
    /// <typeparam name="TEventArgs">The EventArgs type to subscribe to.</typeparam>
    /// <param name="handler">The event handler to be invoked.</param>
    void Subscribe<TEventArgs>(EventHandler<TEventArgs> handler)
        where TEventArgs : EventArgs;

    /// <summary>
    /// Unsubscribes a handler from a specific EventArgs type.
    /// </summary>
    /// <typeparam name="TEventArgs">The EventArgs type to unsubscribe from.</typeparam>
    /// <param name="handler">The event handler to be removed.</param>
    void Unsubscribe<TEventArgs>(EventHandler<TEventArgs> handler)
        where TEventArgs : EventArgs;

    /// <summary>
    /// Publishes (triggers) an event for the given EventArgs type.
    /// </summary>
    /// <typeparam name="TEventArgs">The EventArgs type being published.</typeparam>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="eventArgs">The event data.</param>
    void Publish<TEventArgs>(object sender, TEventArgs eventArgs)
        where TEventArgs : EventArgs;
}

