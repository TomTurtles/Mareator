namespace Mareator;

public sealed class RequestDispatcher : IRequestDispatcher
{
    private readonly Dictionary<Type, object> _handlers = new();

    /// <summary>
    /// Constructor that takes a collection of all request handler base implementations.
    /// These are typically registered via DI and scanned from your assemblies.
    /// </summary>
    /// <param name="handlers">An enumeration of all request handlers.</param>
    public RequestDispatcher(IEnumerable<IRequestHandler> handlers)
    {
        // We reflect on each handler to find out which IRequestHandler<TRequest, TResponse> it implements
        foreach (var handler in handlers)
        {
            var handlerInterfaces = handler.GetType().GetInterfaces()
                .Where(i =>
                    i.IsGenericType
                    && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)
                )
                .ToList();

            foreach (var intf in handlerInterfaces)
            {
                // The generic args: [0] = TRequest, [1] = TResponse
                var requestType = intf.GetGenericArguments()[0];
                // var responseType = intf.GetGenericArguments()[1]; 
                // We don't necessarily need to store responseType in the dictionary 
                // if we assume one TRequest -> one TResponse.

                if (_handlers.ContainsKey(requestType))
                {
                    throw new InvalidOperationException(
                        $"A handler for request type {requestType.Name} is already registered."
                    );
                }

                // Add mapping from TRequest to the handler object
                _handlers.Add(requestType, handler);
            }
        }
    }

    /// <summary>
    /// Processes the given request by invoking the appropriate IRequestHandler.
    /// </summary>
    public async Task<TResponse> RequestAsync<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken = default
    ) where TRequest : IRequest<TResponse>
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var requestType = typeof(TRequest);

        // Retrieve the associated handler from the dictionary
        if (!_handlers.TryGetValue(requestType, out var handlerObj))
        {
            throw new InvalidOperationException(
                $"No handler registered for request type {requestType.Name}."
            );
        }

        cancellationToken.ThrowIfCancellationRequested();

        // Safely cast to IRequestHandler<TRequest,TResponse> and invoke
        var typedHandler = (IRequestHandler<TRequest, TResponse>)handlerObj;
        return await typedHandler.HandleAsync(request, cancellationToken);
    }
}

