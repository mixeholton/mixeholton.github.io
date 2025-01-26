using Komit.Base.Module.Repositories.Context;

namespace Komit.Base.Module.Repositories;
public class BaseEntityRepository<TEntity> : BaseEntityReadRepository<TEntity>
    where TEntity : EntityBase, new()
{
    protected IRepositoryContext _commandContext { get; }
    protected Func<TEntity, string> CreationNameFunction { get; }
    public BaseEntityRepository(IRepositoryContext context, Func<TEntity, string> creationNameFunction, Func<IQueryable<TEntity>, IQueryable<TEntity>> filterChildEntities = null) : base(context, filterChildEntities)
    {
        _commandContext = context;
        CreationNameFunction = creationNameFunction;
    }
    public TEntity Create() => Add(new TEntity());
    private TEntity Add(TEntity entity)
    {
        _commandContext.RegisterCreatedState(entity, CreationNameFunction);
        _context.Set<TEntity>().Add(entity);
        return entity;
    }
    public void Delete(Guid id)
    {
        // ToDo This method does not currently work with the [TimeStamp] optimistic concurrency handling, if .Local does not hold an entity instance, consider opdating or removing it
        _context.Set<TEntity>().Remove(_context.Set<TEntity>().Local.FirstOrDefault(x => x.Id == id)
            ?? new TEntity() { Id = id });
    }
    public void Delete(IEnumerable<Guid> ids)
    {
        // ToDo This method does not currently work with the [TimeStamp] optimistic concurrency handling, if .Local does not hold an entity instance, consider opdating or removing it
        _context.Set<TEntity>().RemoveRange(ids.Select(id => _context.Set<TEntity>().Local.FirstOrDefault(x => x.Id == id)
            ?? new TEntity() { Id = id }));
    }
    public void Delete(TEntity entity) => _context.Set<TEntity>().Remove(entity);
    public void Delete(IEnumerable<TEntity> entity) => _context.Set<TEntity>().RemoveRange(entity);
}