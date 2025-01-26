using Komit.Base.Values.Cqrs;
using Komit.SimpleApiAuth.TokenProvider;
using Komit.SimpleApiAuth.TokenValidator;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Komit.Base.Dev.Server;
public static class ServiceRegistration
{
    public static IServiceCollection AddDevServer(this IServiceCollection services, IConfiguration configuration)
    {
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi();
        services.AddSwaggerGen();
        services.AddHttpClient("SessionApi", x => x.BaseAddress = new Uri(configuration.GetValue<string>("SessionApi")));

        services.AddSimpleTokenProvider(new TokenProviderOptions());
        services.AddSimpleTokenValidation(new TokenValidationOptions());
        return services;
    }
    public static IApplicationBuilder UseDevServer(this WebApplication app)
    {
        app.UseSimpleTokenValidation();
        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            options.RoutePrefix = "navigate";
        });
        return app;
    }
    // ToDo provide db handling via controller
    public static async Task EnsureDatabase<T>(this IApplicationBuilder app, string? dbName = null, bool clean = false)
        where T : DbContext
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var db = scope.ServiceProvider.GetRequiredService<T>();
        {
            scope.ServiceProvider.GetRequiredService<ISetSession>().Set(new SessionValue(Guid.NewGuid(), int.MaxValue, Guid.NewGuid(), int.MaxValue, Guid.NewGuid(), dbName == null ? [] : [new("DB", dbName)], []));
            if (clean)
                await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();
        }
    }
    public static async Task EnsureDatabaseMigration<T>(this IApplicationBuilder app, string? dbName = null, bool clean = false)
        where T : DbContext
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var db = scope.ServiceProvider.GetRequiredService<T>();
        {
            scope.ServiceProvider.GetRequiredService<ISetSession>().Set(new SessionValue(Guid.NewGuid(), int.MaxValue, Guid.NewGuid(), int.MaxValue, Guid.NewGuid(), dbName == null ? [] : [new("DB", dbName)], []));
            if (clean)
                await db.Database.EnsureDeletedAsync();
            await db.Database.MigrateAsync();
        }
    }
}
