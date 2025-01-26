using Komit.Base.Module.App;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace Komit.Base.Specs.Fixtures;
public class AppFixture
{
    protected ServiceFixture Services { get; }
    protected SessionFixture Session { get; }
    public AppFixture(ServiceFixture services, SessionFixture session)
    {
        Services = services;
        Session = session;
    }
    public async Task<CommandResult> Command<T>(Guid sessionId, T command)
        where T : CommandBase
    {
        return await Command(await Session.Get(sessionId), command);
    }

    public async Task<CommandResult> Command<T>(SessionValue session, T command)
    where T : CommandBase
    {
        using var scope = Services.NewScope(session);
        return await scope.ServiceProvider.GetRequiredService<IKomitApp>().Perform(command);
    }

    public async Task<TResult> Query<TQuery, TResult>(Guid sessionId, TQuery query)
        where TQuery : QueryBase<TResult>
    {
        return await Query<TQuery, TResult>(await Session.Get(sessionId), query);
    }

    public async Task<TResult> Query<TQuery, TResult>(SessionValue session, TQuery query)
    where TQuery : QueryBase<TResult>
    {
        using var scope = Services.NewScope(session);
        var result = await scope.ServiceProvider.GetRequiredService<IKomitApp>().Perform<TQuery, TResult>(query);
        if (!result.Success)
        {
            var message = result.Messages.Last();
            throw new DomainException(message.Title, message.Details);
        }
        return result.Result;
    }

    public async Task<RequestResult<TResult>> Request<TRequest, TResult>(Guid sessionId, TRequest request)
        where TRequest : RequestBase
    {
        return await Request<TRequest, TResult>(await Session.Get(sessionId), request);
    }

    public async Task<RequestResult<TResult>> Request<TRequest, TResult>(SessionValue session, TRequest request)
    where TRequest : RequestBase
    {
        using var scope = Services.NewScope(session);
        return await scope.ServiceProvider.GetRequiredService<IKomitApp>().Request<TRequest, TResult>(request);
    }
    // ToDo expose integration events
    // ToDo enable handling of external integration events
}