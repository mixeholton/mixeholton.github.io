using Komit.Base.Module.Handlers.Context;

namespace Komit.Sandbox.App.Context;
public interface ISandboxQueryContext : IReadContext
{
    IQueryable<UserState> Users { get; }
    IQueryable<WineState> Wine { get; }
    IQueryable<BoardState> Boards { get; }
    IQueryable<CycleState> Cycle { get; }
}