namespace Komit.Base.Module.Domain.Context;
public interface IAggregateCollectionContext : IAggregateContext
{
    void AddCreatedState<TState>(TState state) where TState : StateBase;
    void AddCreatedState<TState>(IExposeState<TState> entity) where TState : StateBase;
    void RemoveEntity<TState>(TState entity) where TState : StateBase;
}