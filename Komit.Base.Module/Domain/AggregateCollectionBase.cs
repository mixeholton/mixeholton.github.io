using Komit.Base.Module.Domain.Context;

namespace Komit.Base.Module.Domain;
public abstract class AggregateCollectionBase<TState> : IExposeCollectionState<TState>
    where TState : StateBase, new()
{
    protected abstract Func<IQueryable<TState>, IQueryable<TState>> IncludedRelations { get; }
    Func<IQueryable<TState>, IQueryable<TState>>? IExposeCollectionState<TState>._includedRelations => IncludedRelations;
    private IAggregateCollectionContext _context;
    protected List<TState> State { get; private set; } // Notice! the State list is not changetracked, But the state list items are
    IReadOnlyCollection<TState> IExposeCollectionState<TState>._state => State;
    void IAggregateCollection.InitializeNew(IAggregateCollectionContext context)
        => (this as IExposeCollectionState<TState>).InitializeExisting(new List<TState>(), context);
    void IExposeCollectionState<TState>.InitializeExisting(List<TState> state, IAggregateCollectionContext context)
    {
        if (State != default || _context != default)
            throw new InvalidOperationException();
        State = state;
        _context = context;
    }
    protected void PublishEvent(IExposeState<TState> source, Func<EventBase> @event) => _context.PublishEvent(source, @event);
    protected TState AddNewChild(TState child, Func<TState, string> creationNameFunction)
    {
        State.Add(child); 
        _context.AddCreatedState(child);
        _context.RegisterCreatedState(child, creationNameFunction);
        return child;
    }
    protected TEntity AddNewChild<TEntity>(TEntity child)
       where TEntity : AggregateBase<TState>, IExposeState<TState>, new()
    {
        child.InitializeNew(_context);
        State.Add(child._state);
        _context.AddCreatedState(child); // Because the State list is not changetracked
        _context.RegisterCreatedState(child);
        return child;
    }
    protected bool RemoveChild(TState child)
    {
        if (!State.Remove(child))
            return false;
        _context.RemoveEntity(child); // Because  the State list is not changetracked
        return true;
    }
    protected TEntity GetChild<TEntity, TEntityState>(TEntityState child)
       where TEntity : AggregateBase<TEntityState>, IExposeState<TEntityState>, new()
       where TEntityState : StateBase, new()
    {
        var childEntity = new TEntity();
        childEntity.InitializeExisting(child, _context);
        return childEntity;
    }
    protected IEnumerable<TEntity> GetChildren<TEntity, TEntityState>(IEnumerable<TEntityState> child)
        where TEntity : AggregateBase<TEntityState>, IExposeState<TEntityState>, new()
        where TEntityState : StateBase, new()
        => child.Select(GetChild<TEntity, TEntityState>).ToArray();
    protected T StateOf<T>(IExposeState<T> aggregate)
        where T : StateBase
        => aggregate._state;
    protected IEnumerable<T> StateOf<T>(IExposeCollectionState<T> aggregate)
        where T : StateBase, new()
        => aggregate._state;
}