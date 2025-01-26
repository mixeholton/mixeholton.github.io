using Komit.Base.Module.Handlers.Context;
using Komit.Base.Module.Handlers.Context.Markers;
using Komit.Base.Module.Handlers.Context.Transaction;

namespace Komit.Base.Module.Handlers;
public abstract class BaseWriteHandler<TContext> : IHandlerMarker
    where TContext : IWriteContext
{
    protected TContext Context { get; }
    protected ISessionService Session { get; }
    internal ITransactionService TransactionService => Context as ITransactionService;
    protected BaseWriteHandler(TContext context, ISessionService session)
    {
        Context = context;
        //Context.ShowOnlyArchived = false;
        Session = session;
    }
    protected Task<CommandResult> SaveWithRollback(RequestBase request) => Context.SaveWithRollback(request);
}