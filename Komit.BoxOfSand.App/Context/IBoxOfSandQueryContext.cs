using Komit.Base.Module.Handlers.Context;
using Komit.BoxOfSand.App.State;

namespace Komit.BoxOfSand.App.Context;
public interface IBoxOfSandQueryContext : IReadContext
{
    IQueryable<BoxState> Boxes { get; }

    IQueryable<BooksState> Books { get; }
}