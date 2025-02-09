namespace Mareator;

public class Mareator(IServiceProvider serviceProvider) : IMareator
{
    public IServiceProvider ServiceProvider { get; } = serviceProvider;

    public async Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : Request<TResponse>
    {
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(typeof(TRequest), typeof(TResponse));

        var handler = ServiceProvider.GetService(handlerType)
            ?? throw new InvalidOperationException($"No handler for {handlerType}");

        var castedHandler = (IRequestHandler<TRequest, TResponse>)handler;

        return await castedHandler.HandleAsync(request, cancellationToken);
    }


    public async Task SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : Request
    {
        var handlerType = typeof(IRequestHandler<>).MakeGenericType(typeof(TRequest));

        var handler = ServiceProvider.GetService(handlerType)
            ?? throw new InvalidOperationException($"No handler for {handlerType}");

        var castedHandler = (IRequestHandler<TRequest>)handler;

        await castedHandler.HandleAsync(request, cancellationToken);
    }


    public async Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : Notification
    {
        var serviceType = typeof(INotificationHandler<>).MakeGenericType(typeof(TNotification));

        var handlers = ServiceProvider.GetServices(serviceType)
            ?? throw new InvalidOperationException($"Notification handler(s) '{serviceType.Name}' not registered");

        if (handlers.Count() <= 0) return;

        var handleTasks = handlers.Select((object handlerObject) =>
        {
            var handler = (INotificationHandler<TNotification>)handlerObject;
            return handler.Handle(notification);
        });

        await Task.WhenAll(handleTasks);
    }
}