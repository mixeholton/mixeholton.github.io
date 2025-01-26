using Komit.Base.Module.Domain.Context;
using System.Collections.Concurrent;

namespace Komit.Base.Specs;
public static class ContextFixture
{
    public static bool TryGetEvents<TEvent>(Guid sourceId, out IEnumerable<TEvent> @event)
        where TEvent : EventBase 
    {
        @event = new List<TEvent>();
        if (AggregateContextStub._events.TryGetValue(sourceId, out var publishedEvents))
            @event = publishedEvents.Where(x => x is TEvent).Cast<TEvent>();
        return @event.Any();
    }

    public static TEvent? GetSingleOrDefaultEvent<TEvent>(Guid sourceId)
        where TEvent : EventBase
        => TryGetEvents<TEvent>(sourceId, out var events) ? events.SingleOrDefault() : default;
    private class AggregateContextStub : IAggregateContext, IAggregateCollectionContext
    {
        public static ConcurrentDictionary<Guid, List<EventBase>> _events = new();
        public void AddMessage(Message message) { }
        public void RegisterCreatedState<TState>(IExposeState<TState> entity)
            where TState : StateBase, new()
            => AddCreatedState(entity._state);
        public void AddCreatedState<TState>(IExposeState<TState> entity)
            where TState : StateBase
            => AddCreatedState(entity._state);
        public void AddCreatedState<TState>(TState state) 
            where TState : StateBase
        {
            state.Id = Guid.NewGuid();
            state.TenantId = 1;
        }
        public void RegisterCreatedState<TState>(TState state, Func<TState, string> creationNameFunction)
            where TState : StateBase, new()
            => AddCreatedState(state);
        public void RemoveEntity<TState>(TState entity)
            where TState : StateBase
        { }
        public void PublishEvent<TState>(IExposeState<TState> source, EventBase @event) where TState : StateBase, new()
        {
            @event.SetSourceId(source._state.Id);
            if (_events.ContainsKey(@event.SourceId))
            {
                _events.TryGetValue(@event.SourceId, out var events);
                events.Add(@event);
            }
            else
                _events.TryAdd(@event.SourceId, new List<EventBase>() { @event });
        }

        public void PublishEvent<TState>(IExposeState<TState> source, Func<EventBase> @event) where TState : StateBase, new()
        {
            throw new NotImplementedException();
        }
    }
    private static int _id = int.MinValue;
    private static AggregateContextStub _context = new AggregateContextStub();
    
    public static T Create<T>()
        where T : IAggregate, new()
    {
        var aggregate = new T();
        InitializeSimple((dynamic)aggregate);
        return aggregate;
    }
    public static T Create<T, TState>()
        where T : AggregateCollectionBase<TState>, IExposeCollectionState<TState>, new()
        where TState : StateBase, new()
    {
        var aggregate = new T();
        aggregate.InitializeExisting(new List<TState>(), _context);
        return aggregate;
    }
    public static T Create<T, TState>(Action<TState> include)
        where T : IAggregate, IExposeState<TState>, new()
        where TState : StateBase, new()
    {
        var aggregate = new T();
        Initialize<T, TState>(aggregate);
        include(aggregate._state);
        return aggregate;
    }
    public static T GetState<T>(this IExposeState<T> aggregate)
        where T : StateBase, new()
        => aggregate._state;
    public static IReadOnlyCollection<T> GetState<T>(this IExposeCollectionState<T> aggregate)
        where T : StateBase, new()
        => aggregate._state;
    public static T Initialize<T, TState>(T aggregate, TState state = default)
        where T : IAggregate, IExposeState<TState>, new()
        where TState : StateBase, new()
    {
        if (state == default)
            aggregate.InitializeNew(_context);
        else
            aggregate.InitializeExisting(state, _context);
        if(aggregate._state.Id == default)
            aggregate._state.Id = Guid.NewGuid();
        return aggregate;
    }
    public static IEnumerable<T> InitializeList<T, TState>(IEnumerable<(T aggregate, TState state)> values)
        where T : IAggregate, IExposeState<TState>, new()
        where TState : StateBase, new()
        => values.Select(x => Initialize(x.aggregate, x.state));
    public static T Initialize<T, TState>(T aggregate, List<TState> state = default)
        where T : IAggregateCollection, IExposeCollectionState<TState>, new()
        where TState : StateBase, new()
    {
        if (state == default)
            aggregate.InitializeNew(_context);
        else
            aggregate.InitializeExisting(state, _context);
        return aggregate;
    }
    private static void InitializeSimple<T>(IExposeState<T> aggregate, T state = default)
        where T : StateBase, new()
    {
        if (state == default)
            aggregate.InitializeNew(_context);
        else
            aggregate.InitializeExisting(state, _context);
        if (aggregate._state.Id == default)
            aggregate._state.Id = Guid.NewGuid();
        aggregate._state.Id = Guid.NewGuid();
    }
}