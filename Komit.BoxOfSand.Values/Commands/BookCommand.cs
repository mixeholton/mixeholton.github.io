using Komit.Base.Values.Cqrs;

namespace Komit.BoxOfSand.Values.Commands;

public record CreateBookCommand(string Name, string Description, string? Collection):CommandBase(nameof(CreateBookCommand));
