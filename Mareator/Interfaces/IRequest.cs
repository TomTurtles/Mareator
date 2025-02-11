namespace Mareator;

/// <summary>
/// A marker interface for requests that yield a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">The type of the response returned by the request.</typeparam>
public interface IRequest<TResponse>
{
    // Optionally, common properties for requests could go here (e.g., RequestId).
}
