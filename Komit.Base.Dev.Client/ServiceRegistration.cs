using Komit.Base.Ui.Components.Services;
using Komit.Infrastructure.CqrsClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Komit.Base.Dev.Client;
public static class ServiceRegistration
{
    public static async Task<IServiceCollection> AddDevClient(this IServiceCollection services, string baseAddress)
    {
        services.AddHttpClient(Options.DefaultName, x => x.BaseAddress = new Uri(baseAddress));
        services.AddSingleton<SessionHandler>();
        services.AddScoped<HttpClientFactoryUiAdaptor>();
        services.AddTransient<ICqrsClientErrorHandler, ErrorHandler>();
        services.AddTransient<IHttpClientFactoryAdaptor, HttpClientFactoryUiAdaptor>();
        services.AddTransient(x => new CqrsClient(x.GetService<IHttpClientFactoryAdaptor>(), x.GetService<ICqrsClientErrorHandler>()));
        return services;
    }
}
