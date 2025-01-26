namespace Komit.Base.Module.Domain.Context;
public interface IExposeCollectionState<TState>: IAggregateCollection
    where TState : StateBase, new()
{
    IReadOnlyCollection<TState> _state { get; }
    void InitializeExisting(List<TState> state, IAggregateCollectionContext context);
    Func<IQueryable<TState>, IQueryable<TState>>? _includedRelations { get; }
}
