using Komit.Base.Module.App;
using Microsoft.EntityFrameworkCore;

using Komit.Sandbox.App.Context;

namespace Komit.Sandbox.Module;
public static class SandboxAppRegistration
{
    public static readonly SandboxModuleRegistration Module = new(); 
    public static IServiceCollection AddSandboxApp(this IServiceCollection services)
        => services
        .AddDefaultModuleConfiguration<SandboxDbContext, SandboxCommandContext, ISandboxQueryContext>(Module, allowRequestHandlers: true)
        .AddSandboxDb();
    public static IServiceCollection AddSandboxDb(this IServiceCollection services)
        => services
        .AddDbContext<SandboxDbContext>((provider, options) =>
        options.UseSqlServer(provider.GetRequiredService<IConfiguration>().GetConnectionString("Default"), x => x
        .MigrationsHistoryTable($"__{Module.Schema}MigrationsHistory", Module.Schema)
        .MigrationsAssembly(typeof(Program).Assembly.FullName))
        .EnableDetailedErrors()
        .EnableSensitiveDataLogging());
}