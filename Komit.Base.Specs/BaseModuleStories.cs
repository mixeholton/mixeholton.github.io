using Komit.Base.Module.Handlers.Context;
using Komit.Base.Specs.Fixtures;
using Komit.Infrastructure.CqrsClient;
using Microsoft.Playwright;

namespace Komit.Base.Specs;
public class BaseModuleStories<TProgram, TDbContext, TModelContext, TReadContext>
    : BaseAppStories<TDbContext, TModelContext, TReadContext>
    where TProgram : class
    where TDbContext : KomitDbContext, TReadContext
    where TModelContext : ModelContext<TDbContext, TReadContext>
    where TReadContext : IReadContext
{
    protected HostFixture<TProgram> Host { get; set; }
    public CqrsClient Client => Host.Client;
    public BrowserFixture Browser { get; protected set; }
    public string Address(string path = "") => $"{Browser.Address}/{path}";
    public IPage Page { get; protected set; }
    protected void SetModule(ModuleFixture<TProgram, TDbContext, TModelContext, TReadContext> module)
    {
        Services = module.Host.Services;
        Session = module.Session;
        Model = module.Model;
        App = module.App;
        Host = module.Host;
        Browser = module.Browser;
    }
    protected virtual async Task BeginStory(Guid sessionKey, string? address = default) => Page = await Browser.NewPage(sessionKey, Address(address));
    protected virtual Task EndStory() => Page.CloseAsync();
}