using System.Diagnostics;

namespace Mareator;

public static class Module
{
    public static IServiceCollection AddMareator(
        this IServiceCollection services)
    {
        return services.AddMareator(new Assembly[] { });
    }

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
        Assembly[] assemblies)
    {
        // If no assemblies provided, use the calling assembly as a best guess
        if (assemblies == null || assemblies.Length == 0)
        {
            assemblies = [Assembly.GetCallingAssembly()];
        }

        Debug.WriteLine($"Assemblies: {string.Join(",", assemblies.Select(a => a.GetName().Name))}");

        var types = assemblies
            .SelectMany(a => a.GetTypes())
            .ToArray();

        return services.AddMareator(types);
    }

    public static IServiceCollection AddMareator(
        this IServiceCollection services,
        Type[] types)
    {
        if (types == null || types.Length == 0)
        {
            throw new ArgumentException("At least one type must be provided", nameof(types));
        }

        Debug.WriteLine($"Types: {string.Join(",", types.Select(t => t.Name))}");

        RegisterCommandHandlers(services, types);
        RegisterRequestHandlers(services, types);

        // 3. Register the core dispatchers
        services.AddSingleton<IMareatorEventDispatcher, EventDispatcher>();
        services.AddSingleton<IMareatorCommandDispatcher, CommandDispatcher>();
        services.AddSingleton<IMareatorRequestDispatcher, RequestDispatcher>();

        // 4. Register Mareator (Facade)
        services.AddSingleton<IMareator, Mareator>();

        return services;
    }

    private static void RegisterCommandHandlers(IServiceCollection services, Type[] types)
    {
        var handlerTypes = types
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<>)))
            .ToList();

        Debug.WriteLine($"CommandHandlers: {string.Join(",", handlerTypes.Select(t => t.Name))}");

        foreach (var type in handlerTypes)
        {
            services.AddSingleton(typeof(ICommandHandler), type);

            var handlerInterfaces = type.GetInterfaces().Where(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<>));

            foreach (var intf in handlerInterfaces)
            {
                services.AddSingleton(intf, type);
            }
        }
    }

    private static void RegisterRequestHandlers(IServiceCollection services, Type[] types)
    {
        var requestHandlerTypes = types
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
            .ToList();

        Debug.WriteLine($"RequestHandlers: {string.Join(",", requestHandlerTypes.Select(t => t.Name))}");

        foreach (var type in requestHandlerTypes)
        {
            services.AddSingleton(typeof(IRequestHandler), type);

            var handlerInterfaces = type.GetInterfaces().Where(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));

            foreach (var intf in handlerInterfaces)
            {
                services.AddSingleton(intf, type);
            }
        }
    }

}
