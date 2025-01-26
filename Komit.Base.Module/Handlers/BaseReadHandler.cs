using Komit.Base.Module.Handlers.Context;
using Komit.Base.Module.Handlers.Context.Markers;

namespace Komit.Base.Module.Handlers;
public abstract class BaseReadHandler<TContext>: IHandlerMarker
    where TContext : IReadContext
{
    protected TContext Context { get; }
    protected ISessionService Session { get; }
    protected BaseReadHandler(TContext context, ISessionService session)
    {
        Context = context;
        //Context.ShowOnlyArchived = false;
        Session = session;
    }
}