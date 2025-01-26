using Komit.Base.Specs.Fixtures.New;
using Komit.BoxOfSand.App.Context;
using Komit.Sandbox.App.Context;
using Microsoft.Extensions.DependencyInjection;
namespace Komit.Sandbox.Specs;
[SetUpFixture]
public class SetupFixture
{
    protected SandboxHostFixture HostFactory { get; set; }
    public static IServiceProvider RootServiceProvider { get; private set; }
    public static string Path { get; private set; }
    [OneTimeSetUp]
    public async Task Setup()
    {
        HostFactory = new();
        HostFactory.CreateDefaultClient();
        RootServiceProvider = HostFactory.Services;
        Path = HostFactory.Url.ToString();
    }
    [OneTimeTearDown]
    public async Task TearDown()
    {
        if (HostFactory != default)
            HostFactory.Dispose();
    }
}
public class SandboxHostFixture : HostFixtureBase<Web.Program>
{
    protected override IServiceCollection ServiceModifications(IServiceCollection services)
    {
        return default;
    }
    protected override int Port => 8080;
    protected override string Environment => "Development";
}
public class SandboxAppInstance : AppInstance<SandboxDbContext, SandboxCommandContext, ISandboxQueryContext>
{

}
public class BoxOfSandAppInstance : AppInstance<BoxOfSandDbContext, BoxOfSandCommandContext, IBoxOfSandQueryContext>
{

}
public class SandboxAppFactory : AppFactoryBase<SandboxDbContext, SandboxCommandContext, ISandboxQueryContext>
{
    public override ServiceCollection ModuleServices(ServiceCollection services)
    {
        return services;
    }
}