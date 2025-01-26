namespace Komit.Base.Specs.Configuration;

public record PlaywrightSettings
{
    public static string Name => "Playwright";
    public string Browser { get; init; }
    public int SlowMo { get; init; }
    public bool Devtools { get; init; }
    public bool Headless { get; init; }
}
