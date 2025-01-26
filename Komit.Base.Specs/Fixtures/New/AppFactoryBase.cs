using Komit.Base.Module.Handlers.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Komit.Base.Specs.Fixtures.New;
public abstract class AppFactoryBase<TDbContext, TModelContext, TReadContext>
        where TDbContext : KomitDbContext, TReadContext
        where TModelContext : ModelContext<TDbContext, TReadContext>
        where TReadContext : IReadContext
{
    public IConfiguration Configuration { get; }
    protected virtual IEnumerable<string> AppSettingsFiles =>
    [
        "appsettings.json",
        "appsettings.development.json",
        "appsettings.test.json"
    ];
    public AppFactoryBase(string[]? args = default)
    {
        var ConfigurationBuilder = new ConfigurationBuilder();
        foreach (string? appsetting in AppSettingsFiles)
            ConfigurationBuilder.AddJsonFile(appsetting, optional: true, true); // Maybe not nescesarry
        Configuration = ConfigurationBuilder
           .AddJsonFile("appsettings.stories.json", optional: false, true)
           .AddEnvironmentVariables()
           .AddCommandLine(args ?? [])
           .Build();
    }
    public abstract ServiceCollection ModuleServices(ServiceCollection services);
    public TAppInstance BuildAppInstance<TAppInstance>()
        where TAppInstance : AppInstance<TDbContext, TModelContext, TReadContext>, new()
    {
        var serviceRegistration = new ServiceCollection();
        ModuleServices(serviceRegistration);
        var instance = new TAppInstance();
        instance.InitializeFrom(serviceRegistration);
        // Ensure migrations
        return instance;
    }

}
