using Komit.Base.Specs.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;

namespace Komit.Base.Specs.Fixtures;
public class BrowserFixture : IDisposable
{
    private readonly IDictionary<Guid, string> _storedSessions = new Dictionary<Guid, string>();
    public bool CanRetrieveSession(Guid sessionKey) => _storedSessions.ContainsKey(sessionKey);
    public (string Name, string Password) DefaultCredentials { get; set; }
    protected PlaywrightSettings Settings { get; }
    public string Address { get; }
    public IPlaywright PlaywrightInstance { get; private set; }
    public IBrowser BrowserInstance { get; private set; }
    public BrowserFixture(string hostSettingPath, ServiceFixture services)
        : this(services.StoryConfiguration.GetRequiredSection(PlaywrightSettings.Name).Get<PlaywrightSettings>(),
              services.StoryConfiguration.GetRequiredSection(hostSettingPath).Get<HostSettings>().Address)
    {
    }
    public BrowserFixture(PlaywrightSettings settings, string address)
    {
        Settings = settings;
        Address = address;
    }
    public async Task InitializeBrowser()
    {
        PlaywrightInstance = await Playwright.CreateAsync();
        BrowserInstance = await PlaywrightInstance[Settings.Browser].LaunchAsync(new BrowserTypeLaunchOptions
        {
            SlowMo = Settings.SlowMo,
            Headless = Settings.Headless,
            Devtools = Settings.Devtools
        });
    }
    protected virtual async Task InitialSetup() { }
    public async Task<IPage> NewPage(Guid sessionKey, string? address = default)
    {
        var page = await (CanRetrieveSession(sessionKey) ? BrowserInstance.NewPageAsync(new BrowserNewPageOptions
        {
            StorageState = _storedSessions[sessionKey]

        }) : BrowserInstance.NewPageAsync());
        await page.GotoAsync(address ?? Address);
        return page;
    }
    public async Task StorePageSession(IPage page, Guid sessionKey)
    {
        _storedSessions.Add(sessionKey, new(await page.Context.StorageStateAsync()));
    }
    public virtual void Dispose()
        => PlaywrightInstance?.Dispose();
}
