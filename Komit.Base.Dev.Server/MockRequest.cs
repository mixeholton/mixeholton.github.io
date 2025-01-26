using Komit.Base.Module.Handlers.Context.Markers;
using Komit.Base.Module.Session;
using Komit.Base.Values;
using Komit.Base.Values.Cqrs;
using Microsoft.Extensions.DependencyInjection;

namespace Komit.Base.Dev.Server;
public static class MockRequestExtensions
{
    public static IServiceCollection AddMockRequestHandler<TRequest, TResult>(this IServiceCollection services)
        where TRequest : RequestBase<TResult>
    {
        services.AddScoped<IRequestMarker<TRequest>, MockRequest<TRequest, TResult>>();
        return services;
    }
}

public class MockRequest<TRequest, TResult> : IRequestMarker<TRequest>
    where TRequest : RequestBase<TResult>
{
    protected static Dictionary<Guid, IRequestMarker<TRequest>> MockHandlerIndex { get; } = [];
    public MockRequest(ISessionService session) => Session = session;
    protected ISessionService Session { get; }
    public static void RegisterDefault<TMock>(TMock mockHandler)
        where TMock : IRequestMarker<TRequest>
        => RegisterForSession(Guid.Empty, mockHandler);
    public static void RegisterForSession<TMock>(ISessionService session, TMock mockHandler)
        where TMock : IRequestMarker<TRequest>
        => RegisterForSession(session.Value, mockHandler);
    public static void RegisterForSession<TMock>(SessionValue session, TMock mockHandler)
        where TMock : IRequestMarker<TRequest>
        => RegisterForSession(session.Id, mockHandler);
    public static void RegisterForSession<TMock>(Guid sessionId, TMock mockHandler)
        where TMock : IRequestMarker<TRequest>
    {
        if (MockHandlerIndex.ContainsKey(sessionId))
            throw new DomainException($"{nameof(RegisterForSession)} failed", $"The session already has a registered {nameof(mockHandler)}");
        MockHandlerIndex.Add(sessionId, mockHandler);
    }
    protected IRequestMarker<TRequest> GetMockHandler()
        => MockHandlerIndex.ContainsKey(Session.Value.Id)
        ? MockHandlerIndex[Session.Value.Id]
        : MockHandlerIndex[default];
    public async Task<TResult> Perform(TRequest input)
    {
        dynamic handler = GetMockHandler();
        if (input is CommandBase)
        {
            await handler.Perform(input);
            return default;
        }
        return handler.Perform(input);
    }
    public async Task Handle(TRequest input)
    {
        dynamic handler = GetMockHandler();
        await handler.Handle(input);
    }
}
