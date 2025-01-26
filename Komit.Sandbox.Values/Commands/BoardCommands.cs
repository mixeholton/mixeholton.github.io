namespace Komit.Sandbox.Values.Commands;
public record AddWorkItemCommand(Guid BoardId, string Title) : CommandBase();
