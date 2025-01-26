using Komit.Base.Module.App;
using Komit.BoxOfSand.App.Context;
using Microsoft.EntityFrameworkCore;

namespace Komit.BoxOfSand.Module;
public static class BoxOfSandAppRegistration
{
    public static readonly BoxOfSandModuleRegistration Module = new(); 
    public static IServiceCollection AddBoxOfSandApp(this IServiceCollection services)
        => services
        .AddDefaultModuleConfiguration<BoxOfSandDbContext, BoxOfSandCommandContext, IBoxOfSandQueryContext>(Module, allowRequestHandlers: true)
        .AddBoxOfSandDb();
    public static IServiceCollection AddBoxOfSandDb(this IServiceCollection services)
        => services
        .AddDbContext<BoxOfSandDbContext>((provider, options) =>
        options.UseSqlServer(provider.GetRequiredService<IConfiguration>().GetConnectionString("Default"), x => x
        .MigrationsHistoryTable($"__{Module.Schema}MigrationsHistory", Module.Schema)
        .MigrationsAssembly(typeof(Program).Assembly.FullName))
        .EnableDetailedErrors()
        .EnableSensitiveDataLogging());
}