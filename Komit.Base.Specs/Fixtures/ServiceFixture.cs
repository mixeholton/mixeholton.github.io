using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Komit.Base.Specs.Fixtures;
public class ServiceFixture: IDisposable
{
    public IConfiguration StoryConfiguration { get; protected set; }
    public IConfiguration AppConfiguration { get; protected set; }
    public IServiceProvider Provider { get; protected set; }
    protected virtual IEnumerable<string> AppSettingsFiles => new[]
    {
        "appsettings.json",
        "appsettings.development.json"
    };
    public ServiceFixture(string[]? args = default)
    {
        StoryConfiguration = new ConfigurationBuilder().AddJsonFile("appsettings.stories.json", optional: false, true)
            .AddEnvironmentVariables()
            .AddCommandLine(args ?? Array.Empty<string>())
            .Build();
    }
    public virtual IConfiguration BuildAppConfiguration(params string[] args)
    {
        var ConfigurationBuilder = new ConfigurationBuilder();
        foreach (string? appsetting in AppSettingsFiles)
            ConfigurationBuilder.AddJsonFile(appsetting, optional: true, true);
        return AppConfiguration = ConfigurationBuilder
            .AddEnvironmentVariables()
            .AddCommandLine(args ?? Array.Empty<string>())
            .Build();
    }
    public void SetProvider(IServiceProvider provider)
    {
        Provider = provider;
        AppConfiguration = Provider.GetService<IConfiguration>();
    }
    /// <summary>
    /// !Important! Remember to dispose the scope
    /// </summary>
    public IServiceScope NewScope(SessionValue session)
    {
        var scope = Provider.CreateScope();
        if (session != default)
            scope.ServiceProvider.GetService<ISetSession>()?.Set(session);
        return scope;
    }
    public async Task ScopedAction(SessionValue session, Func<IServiceProvider, Task> action)
    {
        using var scope = NewScope(session);
        await action(scope.ServiceProvider);
    }
    public void Dispose()
        => (Provider as ServiceProvider)?.Dispose();
}
