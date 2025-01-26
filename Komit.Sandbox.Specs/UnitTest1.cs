namespace Komit.Sandbox.Specs;

public class Tests: TestBase
{
    [Test]
    public async Task Test1()
    {
        await Page.PauseAsync();
        await Page.GotoAsync($"{SetupFixture.Path}dev/access/{Guid.NewGuid()}");

        await Page.PauseAsync();
    }
}