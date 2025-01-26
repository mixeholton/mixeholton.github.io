namespace Komit.Sandbox.Values.Commands;

public record AddWineCommand(string Name) : CommandBase(nameof(AddWineCommand));
