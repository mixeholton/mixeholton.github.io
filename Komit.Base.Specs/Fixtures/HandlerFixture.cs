using Komit.Base.Module.Handlers.Context;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace Komit.Base.Specs.Fixtures;
public abstract class HandlerFixture<TDbContext, TModelContext, TReadContext>

    where TDbContext : KomitDbContext, TReadContext
    where TModelContext : ModelContext<TDbContext, TReadContext>
    where TReadContext : IReadContext
{
    public ServiceFixture Services { get; protected set; }
    public AppFixture App { get; protected set; }
    public SessionFixture Session { get; protected set; }
    public ModelFixture<TDbContext, TModelContext, TReadContext> Model { get; protected set; }
    public virtual async Task InitializeModule(ServiceFixture services = null)
    {
        Services = services ?? new ServiceFixture();
        Session ??= new SessionFixture(Services.Provider.GetRequiredService<IDistributedCache>());
        await ModelFixture<TDbContext, TModelContext, TReadContext>.HandleExistingDatabase(Services);
        await ModelFixture<TDbContext, TModelContext, TReadContext>.InitializeDatabase(Services);
        Model ??= new ModelFixture<TDbContext, TModelContext, TReadContext>(Services, Session);
        App ??= new AppFixture(Services, Session);
        await InitialAppSetup();
    }
    public virtual async Task InitialAppSetup() { }
    public virtual async Task CleanupModule()
    {
        await ModelFixture<TDbContext, TModelContext, TReadContext>.CleanupDatabase(Services);
    }
}