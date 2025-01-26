using Komit.Base.Module.Handlers.Context;
using Komit.Base.Specs.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
namespace Komit.Base.Specs.Fixtures.New;

public abstract class HostFixtureBase<TProgram>: WebApplicationFactory<TProgram>
    where TProgram : class
{
    protected IHost BrowserHost { get; private set; }
    protected ServiceProvider RootServiceProvider { get; set; }
    public IServiceProvider Services { get; private set; }
    protected virtual IConfiguration? ConfigurationOverrides { get; }
    protected virtual string Environment { get; } = "Test";
    protected abstract int Port { get; }
    public Uri Url => new("http://localhost:" + Port);
    protected virtual IServiceCollection ServiceModifications(IServiceCollection services)
    {
        return services;
    }
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        if (ServiceModifications != default)
            builder.ConfigureServices(x => ServiceModifications(x));
        if (ConfigurationOverrides != default)
            foreach (var config in ConfigurationOverrides.AsEnumerable())
            {
                builder.UseSetting(config.Key, config.Value);
            }
        builder.UseUrls(Url.ToString());
        builder.UseSetting("environment", Environment);
        builder.UseSetting("ASPNETCORE_ENVIRONMENT", Environment);
    }
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var httpClientHost = builder.Build();
        if (BrowserHost == default)
        {
            builder.ConfigureWebHost(x => x.UseKestrel());
            BrowserHost = builder.Build();
            BrowserHost.Start();
            Services = BrowserHost.Services;
        }
        return httpClientHost;
    }
    private static bool dbHasBeenCleared = false;
    public async Task EnsureDb<TDbContext>() 
        where TDbContext : KomitDbContext
    {
        using var scope = Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<TDbContext>();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        if(dbHasBeenCleared == false)
        {
            await dbContext.Database.EnsureDeletedAsync();
            dbHasBeenCleared = true;
        }
        await dbContext.Database.MigrateAsync();
    }
    protected override void Dispose(bool disposing)
    {
        RootServiceProvider?.Dispose();
        BrowserHost?.Dispose();
        base.Dispose(disposing);
    }
}
