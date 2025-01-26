using Komit.Base.Module.Handlers.Context.Markers;
namespace Komit.Base.Module.Handlers;
public interface IRequestHandler<TRequest, TValue> : IRequestMarker<TRequest>
  where TRequest : RequestBase<TValue>
{
    /// <summary>
    /// Requests should only be used for performance optimization purposes
    /// When its nescessary to combine commands & queries in a single call
    /// </summary>
    Task<TValue> Perform(TRequest request);
}
public interface IRequestDepricatedHandler<TRequest, TValue> : IRequestMarker<TRequest>
  where TRequest : RequestBase
{
    Task<TValue> Perform(TRequest request);
}