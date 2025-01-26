using Komit.Base.Module.Handlers.Context;
using Komit.Base.Module.Handlers.Context.Transaction;
using Komit.Base.Specs.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Komit.Base.Specs.Fixtures;
public class ModelFixture<TDbContext, TModelContext, TReadContext>
        where TDbContext : KomitDbContext, TReadContext
        where TModelContext : ModelContext<TDbContext, TReadContext>
        where TReadContext : IReadContext
{
    protected ServiceFixture Services { get; }
    protected SessionFixture Session { get; }
    public ModelFixture(ServiceFixture services, SessionFixture session)
    {
        Services = services;
        Session = session;
    }
    public async Task State(Guid sessionId, Func<TReadContext, Task> action)
    {
        using var scope = Services.NewScope(await Session.Get(sessionId));
        var context = scope.ServiceProvider.GetRequiredService<TReadContext>();
        await action(context);
    }
    public async Task<T> State<T>(Guid sessionId, Func<TReadContext, Task<T>> action)
    {
        using var scope = Services.NewScope(await Session.Get(sessionId));
        var context = scope.ServiceProvider.GetRequiredService<TReadContext>();
        return await action(context);
    }
    public async Task<CommandResult> Domain(Guid sessionId, Func<TModelContext, Task> action)
    {
        using var scope = Services.NewScope(await Session.Get(sessionId));
        var context = scope.ServiceProvider.GetRequiredService<TModelContext>();
        await action(context);
        return await (context as ITransactionService).Commit(new CommandBase("CustomSpecCommand"));
    }
    private static SessionValue SystemDbSession => new(Guid.NewGuid(), 1, Guid.NewGuid(), 1, Guid.NewGuid(), [], []);
    public static async Task HandleExistingDatabase(ServiceFixture services)
    {
        var databaseSettings = services.StoryConfiguration.GetRequiredSection(DatabaseSettings.Name).Get<DatabaseSettings>();
        using var scope = services.NewScope(SystemDbSession);
        scope.ServiceProvider.GetRequiredService<TDbContext>();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        if (databaseSettings?.DropDatabaseBefore ?? false)
            await dbContext.Database.EnsureDeletedAsync();
    }
    public static async Task InitializeDatabase(ServiceFixture services)
    {
        using var scope = services.NewScope(SystemDbSession);
        scope.ServiceProvider.GetRequiredService<TDbContext>();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        if(services.StoryConfiguration.GetRequiredSection(DatabaseSettings.Name).Get<DatabaseSettings>()?.IgnoreMigrations ?? false)
            await dbContext.Database.EnsureCreatedAsync();
        else
            await dbContext.Database.MigrateAsync();
    }
    public static async Task CleanupDatabase(ServiceFixture services)
    {
        if (services.StoryConfiguration.GetRequiredSection(DatabaseSettings.Name).Get<DatabaseSettings>()?.DropDatabaseAfter ?? false)
        {
            using var scope = services.NewScope(SystemDbSession);
            var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
            await dbContext.Database.EnsureDeletedAsync();
        }
    }
}
