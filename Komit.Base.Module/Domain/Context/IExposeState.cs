namespace Komit.Base.Module.Domain.Context;
public interface IExposeState<TState> : IAggregate 
    where TState : StateBase
{
    TState _state { get; }
    void InitializeExisting(TState state, IAggregateContext context);
    Func<IQueryable<TState>, IQueryable<TState>>? _includedRelations { get; }
}