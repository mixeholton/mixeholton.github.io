using Komit.Base.Specs.Fixtures;
using Komit.Base.Specs.Stubs;
using Komit.Infrastructure.CqrsClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;

namespace Komit.Base.Specs;
public class BaseBrowserStories : BaseSpecs
{
    protected ServiceFixture Services { get; set; }
    public BrowserFixture Browser { get; protected set; }
    protected bool UseServicesForClients { get; set; } = false;
    protected ICqrsClientErrorHandler ErrorHandler { get; set; }
    protected Uri HostUri { get; set; }
    public IPage Page { get; protected set; }
    public string Address(string path = "") => $"{Browser.Address}/{path}";
    protected virtual HttpClient HttpClient
        => (UseServicesForClients ? Services.Provider.GetService<IHttpClientFactory>().CreateClient() : default)
        ?? new HttpClient() { BaseAddress = HostUri ??= new Uri(Browser.Address, UriKind.Absolute) };
    public virtual CqrsClient Client
        => (UseServicesForClients ? Services.Provider.GetService<CqrsClient>() : default)
        ?? new CqrsClient(new HttpClientFactoryStub(HttpClient), ErrorHandler ??= new CqrsClientErrorIgnorer());
    protected virtual async Task BeginStory(Guid sessionId, string? address = default) => Page = await Browser.NewPage(sessionId, address);
    protected virtual Task EndStory() => Page.CloseAsync();
}
