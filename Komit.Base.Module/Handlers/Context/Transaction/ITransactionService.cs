namespace Komit.Base.Module.Handlers.Context.Transaction;
public interface ITransactionService : IWriteContext
{
    bool InSubScope { get; set; }
    Task<IEnumerable<EventBase>> PublishEvents();
    Task EnsureTransactionScope();
    Task<CommandResult> SaveCurrentChanges(RequestBase command);
    Task<CommandResult> Commit(RequestBase command);
    Task<CommandResult> Rollback(RequestBase command);
}