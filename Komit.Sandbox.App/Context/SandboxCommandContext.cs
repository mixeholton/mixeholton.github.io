using Komit.Base.Module.Handlers.Context;
using Komit.Base.Module.Repositories;
namespace Komit.Sandbox.App.Context;
public class SandboxCommandContext : ModelContext<SandboxDbContext, ISandboxQueryContext>
{
    public BaseRepository<User, UserState> Users => new(this);
    public BaseEntityRepository<UserState> UserEntities => new(this, x => x.Name);
    public BaseRepository<Test, TestState> Test => new(this, x => x.Where(t => t.Test == nameof(Test)));

    public BaseRepository<Wine, WineState> Wine => new(this);
    public BaseRepository<Board, BoardState> Boards => new(this);
    public BaseReadRepository<WorkItem, WorkItemState> WorkItems => new(this);
    public BaseRepository<Cycle, CycleState> Cycle => new(this);

    public SandboxCommandContext(SandboxDbContext context) : base(context)
    {
    }
}
