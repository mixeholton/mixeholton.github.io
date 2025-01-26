using Komit.Base.Module.Domain.Context;
using Komit.Base.Module.Repositories.Context;

namespace Komit.Base.Module.Repositories;
public class BaseCollectionRepository<TAggregate, TState>
    where TAggregate : AggregateCollectionBase<TState>, IExposeCollectionState<TState>, new()
    where TState : StateBase, new()
{
    protected readonly DbContext _context;
    protected readonly IAggregateCollectionContext _aggregateContext;
    private readonly Func<IQueryable<TState>, IQueryable<TState>> _loadChildEntities = x => x;

    public BaseCollectionRepository(IRepositoryContext context, Func<IQueryable<TState>, IQueryable<TState>>? filterChildEntities = null)
    {
        _context = context.DbContext;
        _aggregateContext = context;
        var includedRelations = new TAggregate()._includedRelations ?? _loadChildEntities;
        _loadChildEntities = filterChildEntities == null ? includedRelations : x => filterChildEntities(includedRelations(x));
    }
    private IQueryable<TState> QueryState() => _loadChildEntities(_context.Set<TState>());
    protected TAggregate BuildAggregate(List<TState> state)
    {
        var aggregate = new TAggregate();
        aggregate.InitializeExisting(state, _aggregateContext);
        return aggregate;
    }
    public async Task<TAggregate> Get()
    {
        var states = await QueryState().ToListAsync();
        return BuildAggregate(states);
    }
}