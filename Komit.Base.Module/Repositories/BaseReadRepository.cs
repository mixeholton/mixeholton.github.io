using Komit.Base.Module.Domain.Context;
using Komit.Base.Module.Repositories.Context;

namespace Komit.Base.Module.Repositories;
public class BaseReadRepository<TAggregate, TState>
    where TAggregate : AggregateBase<TState>, IExposeState<TState>, new()
    where TState : StateBase, new()
{
    protected readonly DbContext _context;
    protected readonly IAggregateContext _aggregateContext;
    private readonly Func<IQueryable<TState>, IQueryable<TState>> _loadChildEntities = x => x;

    public BaseReadRepository(IRepositoryContext context, Func<IQueryable<TState>, IQueryable<TState>>? filterChildEntities = null)
    {
        _context = context.DbContext;
        _aggregateContext = context;
        var includedRelations = new TAggregate()._includedRelations ?? _loadChildEntities;
        _loadChildEntities = filterChildEntities == null ? includedRelations : x => filterChildEntities(includedRelations(x));
    }
    internal protected IQueryable<TState> QueryState() => _loadChildEntities(_context.Set<TState>());
    internal protected TAggregate BuildAggregate(TState state)
    {
        var aggregate = new TAggregate();
        aggregate.InitializeExisting(state, _aggregateContext);
        return aggregate;
    }
    public async Task<TAggregate> Get(Guid id)
    {
        var state = await QueryState().FirstOrDefaultAsync(x => x.Id == id);
        return BuildAggregate(state);
    }

    public async Task<TAggregate> Get(Func<IQueryable<TState>, IQueryable<TState>> query)
    {
        var state = await query(QueryState().AsQueryable()).SingleOrDefaultAsync();
        return BuildAggregate(state);
    }
    public async Task<TAggregate> Get(Func<IQueryable<TState>, Task<TState>> query)
    {
        var state = await query(QueryState().AsQueryable());
        return BuildAggregate(state);
    }
    public async Task<List<TAggregate>> GetList(IEnumerable<Guid> ids)
    {
        var states = await QueryState().Where(x => ids.Contains(x.Id)).ToListAsync();
        return states.Select(x => BuildAggregate(x)).ToList();
    }
    public async Task<List<TAggregate>> GetList(Func<IQueryable<TState>, IQueryable<TState>> query)
    {
        var states = await query(QueryState().AsQueryable()).ToListAsync();
        return states.Select(x => BuildAggregate(x)).ToList();
    }
    public async Task<List<TAggregate>> GetAll()
    {
        var states = await QueryState().ToListAsync();
        return states.Select(x => BuildAggregate(x)).ToList();
    }
}
