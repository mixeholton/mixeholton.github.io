using Komit.Base.Module.Handlers.Context;

namespace Komit.Base.Specs.Fixtures;
public abstract class ModuleFixture<TProgram, TDbContext, TModelContext, TReadContext>
    : HandlerFixture<TDbContext, TModelContext, TReadContext>
    where TProgram : class
    where TDbContext : KomitDbContext, TReadContext
    where TModelContext : ModelContext<TDbContext, TReadContext>
    where TReadContext : IReadContext
{
    public HostFixture<TProgram> Host { get; protected set; }
    public BrowserFixture Browser { get; protected set; }
    public override async Task InitializeModule(ServiceFixture services = null)
    {
        Host ??= new HostFixture<TProgram>(nameof(Host));
        await base.InitializeModule(Host.Services);
        Browser ??= new BrowserFixture(nameof(Host), Host.Services);
        await Browser.InitializeBrowser();
        await InitialBrowserSetup();
    }
    public virtual async Task InitialBrowserSetup() { }
    public override async Task CleanupModule()
    {
        Browser.Dispose();
        await base.CleanupModule();
        Host.Cleanup();
    }
}