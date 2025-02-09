using Microsoft.Extensions.DependencyInjection;

namespace Mareator;

public static class Module
{
    public static IServiceCollection AddMareator(this IServiceCollection services, params Assembly[] assemblies)
    {
        if (assemblies.Length <= 0)
        {
            assemblies = [Assembly.GetExecutingAssembly()];
        }

        foreach (var assembly in assemblies)
        {
            services
                .RegisterRequestHandlers(assembly)
                .RegisterNotificationHandlers(assembly);
        }

        return services
            .AddSingleton<IMareator, Mareator>();
    }

    private static IServiceCollection RegisterRequestHandlers(this IServiceCollection services, Assembly assembly)
    {
        Type[] requestHandlerTypes = [
            typeof(IRequestHandler<,>),
            typeof(IRequestHandler<>),
        ];

        var handlerTypes = assembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && requestHandlerTypes.Contains(i.GetGenericTypeDefinition())))
            .ToList();

        foreach (var handlerType in handlerTypes)
        {
            var handlerInterfaces = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType && requestHandlerTypes.Contains(i.GetGenericTypeDefinition()));

            foreach (var handlerInterface in handlerInterfaces)
            {
                if (services.Select(serviceDescriptor => serviceDescriptor.ServiceType).Contains(handlerInterface))
                {
                    throw new InvalidOperationException($"ServiceCollection already contains a request handler '{handlerInterface}'");
                }
                else
                {
                    services.AddTransient(handlerInterface, handlerType);
                }
                
            }
        }

        return services;
    }
    private static IServiceCollection RegisterNotificationHandlers(this IServiceCollection services, Assembly assembly)
    {
        Type[] requestHandlerTypes = [
            typeof(INotificationHandler<>),
        ];

        var handlerTypes = assembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && requestHandlerTypes.Contains(i.GetGenericTypeDefinition())))
            .ToList();

        foreach (var handlerType in handlerTypes)
        {
            var handlerInterfaces = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType && requestHandlerTypes.Contains(i.GetGenericTypeDefinition()));

            foreach (var handlerInterface in handlerInterfaces)
            {
                services.AddTransient(handlerInterface, handlerType);
            }
        }

        return services;
    }

}
