namespace Mareator;

public static class Module
{
    /// <summary>
    /// Registers IEventDispatcher, ICommandDispatcher, and IMareator as singletons.
    /// Also scans the provided assemblies for any ICommandHandler<TCommand> implementations
    /// and registers them. If no assemblies are specified, it uses the calling assembly by default.
    /// </summary>
    /// <param name="services">The DI service collection.</param>
    /// <param name="assemblies">Assemblies to scan for ICommandHandler implementations.</param>
    /// <returns>The updated IServiceCollection.</returns>
    public static IServiceCollection AddMareator(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        // If no assemblies provided, use the calling assembly as a best guess
        if (assemblies == null || assemblies.Length == 0)
        {
            assemblies = [Assembly.GetCallingAssembly()];
        }

        // 1. Register all ICommandHandler<TCommand> implementations from the given assemblies
        foreach (var assembly in assemblies)
        {
            var handlerTypes = assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<>)))
                .ToList();

            foreach (var type in handlerTypes)
            {
                // Register the handler as itself for the DI container
                services.AddSingleton(typeof(ICommandHandler), type);

                // Also register for each ICommandHandler<TCommand> interface it implements
                var handlerInterfaces = type.GetInterfaces().Where(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<>));

                foreach (var intf in handlerInterfaces)
                {
                    services.AddSingleton(intf, type);
                }
            }
        }

        var requestHandlerTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
            .ToList();

        foreach (var type in requestHandlerTypes)
        {
            // Register a base interface if you have it, e.g. IRequestHandlerBase
            // This ensures the RequestDispatcher can accept IEnumerable<IRequestHandlerBase>.
            services.AddSingleton(typeof(IRequestHandler), type);

            // Also register each IRequestHandler<TRequest, TResponse> interface
            var handlerInterfaces = type.GetInterfaces().Where(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));

            foreach (var intf in handlerInterfaces)
            {
                services.AddSingleton(intf, type);
            }
        }

        // 2. Register the core dispatchers
        services.AddSingleton<IEventDispatcher, EventDispatcher>();
        services.AddSingleton<ICommandDispatcher, CommandDispatcher>();
        services.AddSingleton<IRequestDispatcher, RequestDispatcher>();

        // 3. Register Mareator (Facade)
        services.AddSingleton<IMareator, Mareator>();

        return services;
    }
}
