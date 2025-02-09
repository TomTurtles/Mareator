namespace Mareator;

public interface IMareator
{
    Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default) where TRequest : Request<TResponse>;
    Task SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : Request;
    Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : Notification;
}
