using Komit.Base.Module.Handlers.Context;
using Komit.Base.Module.Handlers.Context.Markers;
using Komit.Base.Module.Handlers.Context.Transaction;
using Komit.Base.Module.Http;
using Komit.Base.Module.Session.Implementation;
using Komit.Infrastructure.CqrsClient;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace Komit.Base.Module.App;

public static class KomitAppServiceExtensions
{
    private static KomitModuleService _moduleService = new();
    private static List<Func<IServiceProvider, DbContext>> _moduleDbRegistrations = [];
    public static IEnumerable<Func<IServiceProvider, DbContext>> ModuleDbRegistrations => [.. _moduleDbRegistrations];
    public enum HandlerTypes
    {
        Command,
        Query,
        Request,
        Event
    }
    public static IServiceCollection AddDefaultAppConfiguration(this IServiceCollection services)
    {
        return services
            .AddScoped<SessionService>()
            .AddTransient<ISessionService>(x => x.GetRequiredService<SessionService>())
            .AddTransient<ISetSession>(x => x.GetRequiredService<SessionService>())
            .AddTransient<CqrsClient>()
            .AddTransient<IHttpClientFactoryAdaptor, HttpClientFactoryAppAdaptor>()
            .AddTransient<ICqrsClientErrorHandler, CqrsClientAppErrorHandler>()
            .AddSingleton<AppHandlerIndex>(x => new(x.GetServices<ModuleHandlerIndex>()))
            .AddSingleton(_moduleService)
            .AddTransient<KomitAppClient>()
            .AddTransient<IKomitApp, KomitApp>();
    }
    /// <summary>
    /// Expects following services to be available via dependency injection
    /// DbContext
    /// IConfiguration
    /// IHttpClientFactory
    /// </summary>
    public static IServiceCollection AddDefaultModuleConfiguration<TDbContext, TModelContext, TReadContext>(this IServiceCollection services, KomitModuleRegistrationBase moduleRegistration, bool allowRequestHandlers = false)
        where TDbContext : KomitDbContext, TReadContext
        where TModelContext : ModelContext<TDbContext, TReadContext>
        where TReadContext : IReadContext
    {
        _moduleService.RegisterModule(moduleRegistration);
        _moduleDbRegistrations.Add(x => x.GetRequiredService<TDbContext>());
        var handlerTypes = new List<HandlerTypes>()
        {
            HandlerTypes.Command,
            HandlerTypes.Query,
            HandlerTypes.Event
        };

        if (allowRequestHandlers)
            handlerTypes.Add(HandlerTypes.Request);
        return KomitHandlerRegistration.AddHandlersForContextAssembly<TDbContext, TModelContext, TReadContext>(services, moduleRegistration.Schema, handlerTypes)
            .AddTransient(typeof(TReadContext), x => x.GetRequiredService<TDbContext>())
            .AddScoped<TModelContext>()
            .AddTransient<ITransactionService>(x => x.GetRequiredService<TModelContext>());
    }

    internal static (object request, object handler) GetRequestHandling(this IServiceProvider services, string moduleName, string requestName, string requestJson, bool useCamelCase = false)
    {
        var handlerIndex = services.GetRequiredService<AppHandlerIndex>().ModuleHandlers[moduleName.ToLower()].Handlers;
        if (string.IsNullOrWhiteSpace(requestJson))
            requestJson = "{}";
        var requestKey = requestName.ToLower();
        if (!handlerIndex.ContainsKey(requestKey))
            throw new ArgumentException(requestKey);
        var requestType = Type.GetType(handlerIndex[requestKey].request);
        var requestHandlerType = Type.GetType(handlerIndex[requestKey].handler);
        if (requestType == null || requestHandlerType == null)
            throw new ArgumentException(requestKey);
        var request = JsonSerializer.Deserialize(requestJson, requestType, useCamelCase ? new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase } : default);
        var requestHandler = services.GetService(requestHandlerType);
        if (request == null || requestHandler == null)
            throw new ArgumentException(requestKey);
        return (request, requestHandler);
    }
    internal static IRequestMarker<T> GetHandler<T>(this IServiceProvider services) where T : RequestBase
        => services.GetRequiredService<IRequestMarker<T>>();
    internal static object GetHandler(this IServiceProvider services, RequestBase request)
        => services.GetRequiredService(typeof(IRequestMarker<>).MakeGenericType(request.GetType()));
    public static IServiceCollection AddMigrationSessionHandling(this IServiceCollection services, string? dbName = null)
    {
        var sessionService = new SessionService();
        sessionService.SetMigrationSession(dbName);
        services.AddSingleton(x => sessionService);
        services.AddSingleton<ISessionService>(sessionService);
        return services;
    }
    public static SessionValue SetMigrationSession(this ISetSession session, string? dbName = null)
    {
        var value = new SessionValue(Guid.NewGuid(), int.MaxValue, Guid.NewGuid(), int.MaxValue, Guid.NewGuid(), dbName == null ? [] : [new("DB", dbName)], []);
        session.Set(value);
        return value;
    }
}
