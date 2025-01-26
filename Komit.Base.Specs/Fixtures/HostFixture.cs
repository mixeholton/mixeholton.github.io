using Komit.Base.Specs.Configuration;
using Komit.Base.Specs.Stubs;
using Komit.Infrastructure.CqrsClient;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Komit.Base.Specs.Fixtures;
public class HostFixture<T> where T : class
{
    public Action<IServiceCollection>? ServiceModifications { get; set; }
    protected HostSettings Settings { get; }
    public ServiceFixture Services { get; }
    protected HostFixtureFactory HostFactory { get; }
    protected ICqrsClientErrorHandler ErrorHandler { get; set; }
    protected Uri HostUri { get; }
    protected virtual HttpClient HttpClient
        => (Settings.InProcess ? HostFactory.CreateClient() : default)
        ?? Services.Provider.GetService<HttpClient>()
        ?? new HttpClient() { BaseAddress = HostUri };
    public virtual CqrsClient Client
        => Services.Provider.GetService<CqrsClient>()
        ?? new CqrsClient(new HttpClientFactoryStub(HttpClient), ErrorHandler ??= new CqrsClientErrorIgnorer());
    public HostFixture(string hostSettingPath)
    {
        Services = new();
        Settings = Services.StoryConfiguration.GetRequiredSection(hostSettingPath).Get<HostSettings>();
        HostUri = new(Settings.Address, UriKind.Absolute);
        if (Settings.InProcess)
        {
            HostFactory = new HostFixtureFactory(hostSettingPath, Settings, Services)
            {
                ServiceModifications = ServiceModifications
            };
            HostFactory.CreateDefaultClient(); // Needed to create the browser host
            Services.SetProvider(HostFactory.BrowserHost.Services);
        }
    }
    public void Cleanup()
    {
        if (HostFactory != default)
        {
            HostFactory.BrowserHost.Dispose();
            HostFactory.Dispose(); // Check if its needed to stop & dispose the BrowserHost
        }
        else
            Services.Dispose();
    }
    public class HostFixtureFactory : WebApplicationFactory<T>
    {
        public IHost BrowserHost { get; private set; }
        protected string HostSettingPath { get; }
        public HostSettings Settings { get; }
        protected ServiceFixture Services { get; }
        protected SessionFixture Session { get; }
        public Action<IServiceCollection>? ServiceModifications { get; set; }
        public HostFixtureFactory(string hostSettingPath, HostSettings settings, ServiceFixture services)
        {
            HostSettingPath = hostSettingPath;
            Settings = settings;
            Services = services;
        }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            if (ServiceModifications != default)
                builder.ConfigureServices(x => ServiceModifications(x));
            builder.UseUrls(Settings.Address);
            var overrides = HostSettingPath + ":InprocessSettingesOverrides";

            builder.UseSetting("ConnectionStrings:Default", Services.StoryConfiguration.GetRequiredSection(DatabaseSettings.Name).Get<DatabaseSettings>().ConnectionString);
            foreach (var setting in Services.StoryConfiguration.GetSection(overrides).AsEnumerable())
            {
                var key = setting.Key.Replace(overrides, string.Empty).TrimStart(':');
                if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(setting.Value))
                    continue;
                builder.UseSetting(key, setting.Value);
            }
            // Notice! The asp.net test host only works for the browser with development environment
            builder.UseSetting("environment", "Development");
            builder.UseSetting("ASPNETCORE_ENVIRONMENT", "Development");
        }
        protected override IHost CreateHost(IHostBuilder builder)
        {
            var httpClientHost = builder.Build();
            if (BrowserHost == default)
            {
                builder.ConfigureWebHost(x => x.UseKestrel());
                BrowserHost = builder.Build();
                BrowserHost.Start();
            }
            return httpClientHost;
        }
    }
}
