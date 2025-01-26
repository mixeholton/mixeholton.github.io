using Komit.Base.Values.Cqrs;

namespace Komit.BoxOfSand.Values.Commands;
public record CreateBoxCommand(string Name) : CommandBase(nameof(CreateBoxCommand));

public record ArchiveBoxCommand(Guid BoxId) : CommandBase(nameof(CreateBoxCommand));

public record UndoLastBoxArchivationCommand() : CommandBase(nameof(CreateBoxCommand));
