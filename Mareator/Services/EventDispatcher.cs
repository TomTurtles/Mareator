namespace Mareator;

public sealed class EventDispatcher : IMareatorEventDispatcher
{
    // An object used for thread-safety (locking).
    private readonly object _lockObj = new();

    // Dictionary: Key = type of EventArgs, Value = a combined delegate (EventHandler<T>).
    private readonly Dictionary<Type, Delegate> _eventHandlers = new();

    // Public or internal constructor, depending on your needs.
    // For DI, this can be registered as a singleton.
    public EventDispatcher()
    {
    }

    /// <summary>
    /// Subscribes a handler for the specified EventArgs type.
    /// </summary>
    /// <typeparam name="TEventArgs">The EventArgs type to subscribe to.</typeparam>
    /// <param name="handler">The event handler to be added to the invocation list.</param>
    public void Subscribe<TEventArgs>(EventHandler<TEventArgs> handler)
        where TEventArgs : EventArgs
    {
        lock (_lockObj)
        {
            if (!_eventHandlers.TryGetValue(typeof(TEventArgs), out var existingDelegate))
            {
                // No delegate registered yet for this EventArgs type
                _eventHandlers[typeof(TEventArgs)] = handler;
            }
            else
            {
                // Existing delegate found -> combine delegates (add to invocation list)
                _eventHandlers[typeof(TEventArgs)] =
                    (EventHandler<TEventArgs>)existingDelegate + handler;
            }
        }
    }

    /// <summary>
    /// Unsubscribes a handler from the specified EventArgs type.
    /// </summary>
    /// <typeparam name="TEventArgs">The EventArgs type to unsubscribe from.</typeparam>
    /// <param name="handler">The event handler to be removed from the invocation list.</param>
    public void Unsubscribe<TEventArgs>(EventHandler<TEventArgs> handler)
        where TEventArgs : EventArgs
    {
        lock (_lockObj)
        {
            if (_eventHandlers.TryGetValue(typeof(TEventArgs), out var existingDelegate))
            {
                // Remove the handler from the delegate chain
                var newDelegate = (EventHandler<TEventArgs>)existingDelegate - handler;
                if (newDelegate is null)
                {
                    // No more handlers remain -> remove from dictionary
                    _eventHandlers.Remove(typeof(TEventArgs));
                }
                else
                {
                    // Update the dictionary with the new delegate (minus the unsubscribed handler)
                    _eventHandlers[typeof(TEventArgs)] = newDelegate;
                }
            }
        }
    }

    /// <summary>
    /// Publishes (triggers) an event for the given EventArgs type.
    /// </summary>
    /// <typeparam name="TEventArgs">The EventArgs type being published.</typeparam>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="eventArgs">The event data.</param>
    public void Publish<TEventArgs>(object sender, TEventArgs eventArgs)
        where TEventArgs : EventArgs
    {
        EventHandler<TEventArgs> handlerCopy = null;

        lock (_lockObj)
        {
            if (_eventHandlers.TryGetValue(typeof(TEventArgs), out var existingDelegate))
            {
                // Copy the delegate to invoke outside the lock
                handlerCopy = (EventHandler<TEventArgs>)existingDelegate;
            }
        }

        // Invoke the copied delegate outside of the lock to prevent blocking
        handlerCopy?.Invoke(sender, eventArgs);
    }
}
