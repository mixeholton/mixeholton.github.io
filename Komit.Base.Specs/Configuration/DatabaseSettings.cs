namespace Komit.Base.Specs.Configuration;

public record DatabaseSettings()
{
    public static string Name => "Database";
    public bool DropDatabaseBefore { get; init; } = true;
    public bool DropDatabaseAfter { get; init; } = true;
    public string ConnectionString { get; init; }
    public bool IgnoreMigrations { get; init; } = false;
}
