using Komit.Base.Module.Domain.Context;
using Komit.Base.Module.Handlers;
using Komit.Base.Module.Handlers.Context;
using Komit.Base.Module.Handlers.Context.Transaction;
using Komit.Base.Module.Repositories.Context;
using Komit.Base.Module.Session;

namespace Komit.Base.Specs.CommandSpecs;
public abstract class CommandSpecs<TDbContext, TModelContext, TReadContext> : BaseSpecs, IDisposable
    where TDbContext : KomitDbContext, TReadContext
    where TModelContext : ModelContext<TDbContext, TReadContext>, ITransactionService, IRepositoryContext
    where TReadContext : IReadContext
{
    protected TModelContext Context { get; }
    public ISessionService Session { get; }
    public CommandSpecs(TModelContext context, ISessionService session)
    {
        Context = context;
        Session = session;
    }
    public void Dispose()
    {
        Context.DbContext.Dispose();
    }
    public async Task SeedAggregates<T>(IEnumerable<IExposeState<T>> aggregates) where T : StateBase
    {
        foreach (var state in aggregates.Select(x => x._state))
            Context.DbContext.Add(state);
        await Context.DbContext.SaveChangesAsync();
    }
    public async Task<Guid> SeedAggregate<T>(IExposeState<T> aggregate) where T : StateBase
    {
        Context.DbContext.Add(aggregate._state);
        await Context.DbContext.SaveChangesAsync();
        return aggregate._state.Id;
    }
    public async Task<CommandResult> Perform<TCommandHandler, TCommand>(TCommandHandler handler, TCommand command)
        where TCommand : CommandBase
        where TCommandHandler : ICommandHandler<TCommand>
    {
        await handler.Perform(command);
        return await Context.Commit(command);
    }
}