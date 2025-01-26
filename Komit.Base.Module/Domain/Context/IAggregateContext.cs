namespace Komit.Base.Module.Domain.Context;
public interface IAggregateContext
{
    void AddMessage(Message message);
    void RegisterCreatedState<TState>(IExposeState<TState> entity) where TState : StateBase, new();
    void RegisterCreatedState<TState>(TState state, Func<TState, string>? creationNameFunction) where TState : StateBase, new();
    void PublishEvent<TState>(IExposeState<TState> source, Func<EventBase> @event) where TState : StateBase, new();
}