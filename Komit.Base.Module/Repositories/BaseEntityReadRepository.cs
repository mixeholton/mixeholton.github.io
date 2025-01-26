using Komit.Base.Module.Handlers.Context;
using Komit.Base.Module.Repositories.Context;

namespace Komit.Base.Module.Repositories;
public class BaseEntityReadRepository<TEntity>
    where TEntity : EntityBase, new()
{
    protected readonly KomitDbContext _context;
    private readonly Func<IQueryable<TEntity>, IQueryable<TEntity>> _loadChildEntities;

    public BaseEntityReadRepository(IRepositoryContext context, Func<IQueryable<TEntity>, IQueryable<TEntity>> filterChildEntities = null)
    {
        _context = context.DbContext;
        _loadChildEntities = filterChildEntities;
    }
    private IQueryable<TEntity> QueryEntity() => _loadChildEntities == default ? _context.Set<TEntity>() : _loadChildEntities(_context.Set<TEntity>());
    public async Task<TEntity> Get(Guid id) => await QueryEntity().FirstOrDefaultAsync(x => x.Id == id);
    public async Task<TEntity> Get(Func<IQueryable<TEntity>, IQueryable<TEntity>> query) => await query(QueryEntity().AsQueryable()).SingleOrDefaultAsync();
    public async Task<TEntity> Get(Func<IQueryable<TEntity>, Task<TEntity>> query) => await query(QueryEntity());
    public async Task<List<TEntity>> GetList(IEnumerable<Guid> ids) => await QueryEntity().Where(x => ids.Contains(x.Id)).ToListAsync();
    public async Task<List<TEntity>> GetList(Func<IQueryable<TEntity>, IQueryable<TEntity>> query) => await query(QueryEntity()).ToListAsync();
    public async Task<List<TEntity>> GetAll() => await QueryEntity().ToListAsync();
}