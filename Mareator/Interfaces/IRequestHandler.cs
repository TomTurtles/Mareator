namespace Mareator;

public interface IRequestHandler<in TRequest, TResponse> where TRequest : Request<TResponse>
{
    Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
}

public interface IRequestHandler<in TRequest> where TRequest : Request
{
    Task HandleAsync(TRequest request, CancellationToken cancellationToken);
}