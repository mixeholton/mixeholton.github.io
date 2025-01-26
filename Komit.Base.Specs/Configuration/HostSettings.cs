namespace Komit.Base.Specs.Configuration;

public record HostSettings()
{
    public string Address { get; init; }
    public bool InProcess { get; init; } = true;
}
