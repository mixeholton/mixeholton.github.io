using Komit.Base.Module.Handlers.Context;
using Komit.BoxOfSand.App.State;

namespace Komit.BoxOfSand.App.Context;
public class BoxOfSandDbContext : KomitDbContext, IBoxOfSandQueryContext
{
    IQueryable<BoxState> IBoxOfSandQueryContext.Boxes => Boxes.AsNoTracking();
    public DbSet<BoxState> Boxes { get; set; }

    IQueryable<BooksState> IBoxOfSandQueryContext.Books => Books.AsNoTracking();
    public DbSet<BooksState> Books { get; set; }

    //protected override bool UseArchivingForModule => true;

    public BoxOfSandDbContext(DbContextOptions<BoxOfSandDbContext> options, ISessionService session) : base(options, session)
    {
        //ShowOnlyArchived = true;
    }
    protected override void MapState(ModelBuilder builder)
    {
        builder.HasAnnotation("Relational:Collation", "Danish_Norwegian_CI_AS");
        builder.HasDefaultSchema("BoxOfSand");
    }
}
