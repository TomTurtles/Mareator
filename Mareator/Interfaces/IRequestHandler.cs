namespace Mareator;

/// <summary>
/// Defines a handler for a specific request type, returning a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TRequest">The request type to handle.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the request.</typeparam>
public interface IRequestHandler<TRequest, TResponse> : IRequestHandler
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Handles the request asynchronously and returns a response.
    /// </summary>
    /// <param name="request">The request instance.</param>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation, yielding a <typeparamref name="TResponse"/>.</returns>
    Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}

public interface IRequestHandler { };