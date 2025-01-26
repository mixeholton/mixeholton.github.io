using Komit.Base.Module.Handlers.Context;
using Komit.Base.Module.Repositories;
using Komit.BoxOfSand.App.Domain;
using Komit.BoxOfSand.App.State;

namespace Komit.BoxOfSand.App.Context;
public class BoxOfSandCommandContext : ModelContext<BoxOfSandDbContext, IBoxOfSandQueryContext>
{
    public BaseRepository<Box, BoxState> Boxes => new(this);
    public BaseRepository<Book, BooksState> Books => new(this);
    public BoxOfSandCommandContext(BoxOfSandDbContext context) : base(context)
    {
    }
}
