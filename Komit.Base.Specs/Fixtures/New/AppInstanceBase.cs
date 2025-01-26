using Komit.Base.Module.Handlers.Context;
using Microsoft.Extensions.DependencyInjection;
using Komit.Base.Dev.Server;
using Komit.Base.Module.Handlers.Context.Transaction;
namespace Komit.Base.Specs.Fixtures.New;

public class AppInstance<TDbContext, TModelContext, TReadContext>
        where TDbContext : KomitDbContext, TReadContext
        where TModelContext : ModelContext<TDbContext, TReadContext>
        where TReadContext : IReadContext
{
    protected IServiceProvider Services { get; private set; }
    // Session, tenant & Scope management
    // App & Model access
    public AppInstance() { }
    public void InitializeFrom(ServiceCollection services)
    {
        Services = services.BuildServiceProvider();
    }
    public void InitializeFrom(IServiceProvider serviceProvider)
    {
        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(Services));
        if (Services != null)
            throw new DomainException("Set RootServices failed", "RootServices cannot be changed once set");
        Services = serviceProvider;
    }
    public async Task<CommandResult> Command<T>(SessionValue session, T command)
    where T : CommandBase
    {
        using var scope = Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<ISetSession>().Set(session);
        return await scope.ServiceProvider.GetRequiredService<IKomitApp>().Perform(command);
    }
    public async Task<TResult> Query<TQuery, TResult>(SessionValue session, TQuery query)
    where TQuery : QueryBase<TResult>
    {
        using var scope = Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<ISetSession>().Set(session);
        var result = await scope.ServiceProvider.GetRequiredService<IKomitApp>().Perform<TQuery, TResult>(query);
        if (!result.Success)
        {
            var message = result.Messages.Last();
            throw new DomainException(message.Title, message.Details);
        }
        return result.Result;
    }
    public async Task<RequestResult<TResult>> Request<TRequest, TResult>(SessionValue session, TRequest request)
    where TRequest : RequestBase
    {
        using var scope = Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<ISetSession>().Set(session);
        return await scope.ServiceProvider.GetRequiredService<IKomitApp>().Request<TRequest, TResult>(request);
    }



    public async Task State(SessionValue session, Func<TReadContext, Task> action)
    {
        using var scope = Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<ISetSession>().Set(session);
        var context = scope.ServiceProvider.GetRequiredService<TReadContext>();
        await action(context);
    }
    public async Task<T> State<T>(SessionValue session, Func<TReadContext, Task<T>> action)
    {
        using var scope = Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<ISetSession>().Set(session);
        var context = scope.ServiceProvider.GetRequiredService<TReadContext>();
        return await action(context);
    }
    public async Task<CommandResult> Domain(SessionValue session, Func<TModelContext, Task> action)
    {
        using var scope = Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<ISetSession>().Set(session);
        var context = scope.ServiceProvider.GetRequiredService<TModelContext>();
        await action(context);
        return await (context as ITransactionService).Commit(new CommandBase("CustomSpecCommand"));
    }
}