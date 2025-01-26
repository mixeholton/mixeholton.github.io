using Komit.Base.Module.Handlers.Context.Markers;
namespace Komit.Base.Module.Handlers;
public interface IEventHandler<TEvent> : IRequestMarker<TEvent>
     where TEvent : EventBase
{
    public Task Handle(TEvent @event);
}
public interface IEventBatchHandler<TEvent> : IRequestMarker<TEvent>
     where TEvent : EventBase
{
    public Task Handle(IEnumerable<TEvent> @event);
}