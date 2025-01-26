using Komit.Base.Module.Domain.Context;

namespace Komit.Base.Module.Domain;
public abstract class AggregateBase<TState> : IExposeState<TState>
    where TState : StateBase, new()
{
    private IAggregateContext _context;
    private bool _isNew = false;
    protected bool IsNew => _isNew;
    public Guid Id => State.Id;
    protected TState State { get; private set; }
    protected abstract string _creationCodeName { get; }
    TState IExposeState<TState>._state => State;
    string IAggregate.CreationCodeName => _creationCodeName;
    protected abstract Func<IQueryable<TState>, IQueryable<TState>> IncludedRelations { get; }
    Func<IQueryable<TState>, IQueryable<TState>>? IExposeState<TState>._includedRelations => IncludedRelations;
    void IAggregate.InitializeNew(IAggregateContext context)
    {
        _isNew = true;
        (this as IExposeState<TState>).InitializeExisting(new TState(), context);
    }
    void IExposeState<TState>.InitializeExisting(TState state, IAggregateContext context)
    {
        if (State != default || _context != default)
            throw new InvalidOperationException();
        State = state;
        _context = context;
    }
    protected void EnsureIsInitialized()
    {
        if (State == default || _context == default)
            throw new InvalidOperationException("Aggregate is not initialized");
    }
    protected void EnsureIsNew()
    {
        EnsureIsInitialized();
        if (!_isNew)
            throw new InvalidOperationException("Can only be called on newly initialized aggregates");
    }
    internal void PrepareForDeletionFromRepository() => PrepareForDeletion();
    /// <summary>
    /// Override this method to publish events on deletion
    /// Or to throw exception if deletion is not allowed
    /// </summary>
    protected virtual void PrepareForDeletion() { }
    protected void PublishEvent(Func<EventBase> @event) => _context.PublishEvent(this, @event);
    protected TChildState AddNewChild<TChildState>(TChildState child, ICollection<TChildState> collection, Func<TChildState, string>? creationNameFunction = null)
        where TChildState : StateBase, new()
    {
        collection.Add(child);
        return AddNewChild(child, creationNameFunction);
    }
    protected TChildState AddNewChild<TChildState>(TChildState child, Func<TChildState, string>? creationNameFunction = null)
        where TChildState : StateBase, new()
    {
        _context.RegisterCreatedState(child, creationNameFunction);
        return child;
    }
    protected TEntity AddNewChild<TEntity, TEntityState>(TEntity child, ICollection<TEntityState> collection)
        where TEntity : AggregateBase<TEntityState>, IExposeState<TEntityState>, new()
        where TEntityState : StateBase, new()
    {
        child.InitializeNew(_context);
        _context.RegisterCreatedState(child);
        return AddReference(child, collection);
    }
    protected void SetNewChild<TEntityState>(AggregateBase<TEntityState> child, Action<TState, TEntityState> referenceAction)
       where TEntityState : StateBase, new()
    {
        (child as IAggregate).InitializeNew(_context);
        _context.RegisterCreatedState(child);
        SetReference(child, referenceAction);
    }
    protected TEntity AddReference<TEntity, TEntityState>(TEntity child, ICollection<TEntityState> collection)
        where TEntity : IExposeState<TEntityState>
        where TEntityState : StateBase, new()
    {
        collection.Add(child._state);
        return child;
    }
    protected void SetReference<TEntityState>(IExposeState<TEntityState> child, Action<TState, TEntityState> referenceAction)
        where TEntityState : StateBase, new()
    {
        referenceAction(State, child._state);
    }
    protected T StateOf<T>(IExposeState<T> aggregate)
        where T : StateBase
        => aggregate._state;
    protected IEnumerable<T> StateOf<T>(IExposeCollectionState<T> aggregate)
        where T : StateBase, new()
        => aggregate._state;
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
        => child.Select(x => GetChild<TEntity, TEntityState>(x)).ToArray();
    /// <summary>
    /// Must ensure that only valid state can happen
    /// Throw exception for invalid state change
    /// </summary>
    protected virtual bool ValidateStateOverride(TState oldState, TState newState)
    {
        throw new InvalidOperationException("State change is not allowed, enable by overriding " + nameof(ValidateStateOverride));
    }
    protected void OverrideState(TState newState)
    {
        if(!ValidateStateOverride(State, newState))
            throw new ArgumentException("Invalid", nameof(newState));
        State = newState;
    }
}