using Komit.Base.Module.Handlers.Context.Markers;
namespace Komit.Base.Module.Handlers;
public interface IQueryHandler<TRequest, TValue> : IRequestMarker<TRequest>
   where TRequest : QueryBase<TValue>
{
    Task<TValue> Perform(TRequest query);
}