using Komit.Base.Module.Handlers.Context;
namespace Komit.Sandbox.App.Context;
public class SandboxDbContext : KomitDbContext, ISandboxQueryContext
{
    IQueryable<UserState> ISandboxQueryContext.Users => Users.AsNoTracking();
    IQueryable<WineState> ISandboxQueryContext.Wine => Wine.AsNoTracking();
    IQueryable<BoardState> ISandboxQueryContext.Boards => Boards.AsNoTracking();
    IQueryable<CycleState> ISandboxQueryContext.Cycle => Cycle.AsNoTracking();
    public DbSet<UserState> Users { get; set; }
    public DbSet<TestState> Tests { get; set; }
    public DbSet<WineState> Wine { get; set; }
    public DbSet<BoardState> Boards { get; set; }
    public DbSet<CycleState> Cycle { get; set; }

    //protected override bool UseArchivingForModule => true;


    public SandboxDbContext(DbContextOptions<SandboxDbContext> options, ISessionService session) : base(options, session)
    {
        //ShowOnlyArchived = true;
    }
    protected override void MapState(ModelBuilder builder)
    {
        builder.HasAnnotation("Relational:Collation", "Danish_Norwegian_CI_AS");
        builder.HasDefaultSchema("Sandbox");
    }
}
