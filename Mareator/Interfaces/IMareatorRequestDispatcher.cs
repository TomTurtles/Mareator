namespace Mareator;

/// <summary>
/// Defines a dispatcher that runs a request and retrieves its response.
/// </summary>
public interface IMareatorRequestDispatcher
{
    /// <summary>
    /// Runs the given request by finding its corresponding handler,
    /// executing it asynchronously, and returning the response.
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <typeparam name="TResponse">The response type associated with the request.</typeparam>
    /// <param name="request">The request instance to run.</param>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation, yielding a <typeparamref name="TResponse"/>.</returns>
    Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResponse>;
}