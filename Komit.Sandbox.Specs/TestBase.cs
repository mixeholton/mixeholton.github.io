using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright.NUnit;
namespace Komit.Sandbox.Specs;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class TestBase : PageTest
{
    public SandboxAppInstance App { get; private set; }
    private AsyncServiceScope Scope { get; set; }
    [SetUp]
    public async Task Setup()
    {
        App = new SandboxAppInstance();
        Scope = SetupFixture.RootServiceProvider.CreateAsyncScope();
        App.InitializeFrom(Scope.ServiceProvider);
        await Page.GotoAsync(SetupFixture.Path);
    }

    [TearDown]
    public async Task TearDown()
    {
        await Scope.DisposeAsync();
    }
}
