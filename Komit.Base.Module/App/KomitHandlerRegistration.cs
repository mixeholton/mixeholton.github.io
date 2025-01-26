using Komit.Base.Module.Handlers.Context;
using Komit.Base.Module.Handlers.Context.Markers;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Immutable;
using static Komit.Base.Module.App.KomitAppServiceExtensions;

namespace Komit.Base.Module.App;
internal record AppHandlerIndex
{
    public IReadOnlyDictionary<string, ModuleHandlerIndex> ModuleHandlers { get; }
    public AppHandlerIndex(IEnumerable<ModuleHandlerIndex> moduleHandlers)
    {
        ModuleHandlers = moduleHandlers.ToDictionary(x => x.Module.ToLower());
    }
}
internal record ModuleHandlerIndex(string Module, IReadOnlyDictionary<string, (string request, string handler)> Handlers);
public static class KomitHandlerRegistration
{
    public static IServiceCollection AddHandlersForContextAssembly<TDbContext, TModelContext, TReadContext>(IServiceCollection services, string moduleName, IEnumerable<HandlerTypes> handlerTypes)
       where TDbContext : KomitDbContext, TReadContext
       where TModelContext : ModelContext<TDbContext, TReadContext>
       where TReadContext : IReadContext
    {
        var _handlerIndex = new Dictionary<string, (string request, string handler)>();
        var iHandlerType = typeof(IHandlerMarker);
        var readHandlerType = typeof(BaseReadHandler<TReadContext>);
        var writeHandlerType = typeof(BaseWriteHandler<TModelContext>);
        var handlers = typeof(TModelContext).Assembly.GetTypes().Where(x => x.IsAssignableTo(iHandlerType));
        var iRequestMarkerType = typeof(IRequestMarker<>);
        var commandType = typeof(CommandBase);
        var queryType = typeof(QueryBase);
        var requestType = typeof(RequestBase);
        var eventType = typeof(EventBase);
        foreach (var handler in handlers)
        {
            foreach (var iHandler in handler.GetInterfaces().Where(i => i.IsGenericType
                && i.GetGenericTypeDefinition() == iRequestMarkerType))
            {
                var request = iHandler.GenericTypeArguments.Single();
                HandlerTypes handlerType;
                if (request.IsAssignableTo(commandType))
                    handlerType = HandlerTypes.Command;
                else if (request.IsAssignableTo(queryType))
                    handlerType = HandlerTypes.Query;
                else if (request.IsAssignableTo(eventType))
                    handlerType = HandlerTypes.Event;
                else if (request.IsAssignableTo(requestType))
                    handlerType = HandlerTypes.Request;
                else
                    throw new Exception("Invalid handler: " + request.AssemblyQualifiedName);
                if (handlerType == HandlerTypes.Query && !handler.IsAssignableTo(readHandlerType))
                    throw new Exception("Invalid handler: " + handler.AssemblyQualifiedName + " For: " + request.AssemblyQualifiedName);
                if (handlerType != HandlerTypes.Query && !handler.IsAssignableTo(writeHandlerType))
                    throw new Exception("Invalid handler: " + handler.AssemblyQualifiedName + " For: " + request.AssemblyQualifiedName);
                if (handlerTypes.Contains(handlerType))
                {
                    _handlerIndex.Add(request.Name.ToLower(), (request.AssemblyQualifiedName, iHandler.AssemblyQualifiedName));
                    services.AddTransient(iHandler, handler);
                }
            }
        }
        return services.AddSingleton(new ModuleHandlerIndex(moduleName.ToLower(), _handlerIndex));
    }
}
