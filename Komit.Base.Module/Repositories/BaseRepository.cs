using Komit.Base.Module.Domain.Context;
using Komit.Base.Module.Repositories.Context;

namespace Komit.Base.Module.Repositories;
public class BaseRepository<TAggregate, TState> : BaseReadRepository<TAggregate, TState>
    where TAggregate : AggregateBase<TState>, IExposeState<TState>, new()
    where TState : StateBase, new()
{
    private readonly IRepositoryContext _commandContext;
    public BaseRepository(IRepositoryContext context, Func<IQueryable<TState>, IQueryable<TState>>? filterChildEntities = null) 
        : base(context, filterChildEntities)
    {
        _commandContext = context;
        
    }
    public TAggregate Create()
    {
        var aggregate = new TAggregate();
        aggregate.InitializeNew(_aggregateContext);
        Add(aggregate);
        return aggregate;
    }
    private TAggregate Add(TAggregate aggregate)
    {
        _commandContext.RegisterCreatedState(aggregate);
        _context.Set<TState>().Add(aggregate._state);
        return aggregate;
    }
    public void Delete(TAggregate aggregate)
    {
        aggregate.PrepareForDeletionFromRepository();
        _context.Set<TState>().Remove(aggregate._state);
    }
    public void Delete(IEnumerable<TAggregate> aggregates)
    {
        aggregates.ToList().ForEach(Delete);
    }
}