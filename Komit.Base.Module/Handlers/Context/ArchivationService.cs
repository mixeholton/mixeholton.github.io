using Komit.Base.Module.Handlers.Context.Transaction;
using Komit.Base.Module.Repositories.Context;

namespace Komit.Base.Module.Handlers.Context;

public class ArchivationService
{
    protected IRepositoryContext Context { get; }
    protected ITransactionService Transaction { get; }
    protected ISessionService Session { get; }
    public ArchivationService(IRepositoryContext context, ITransactionService transaction, ISessionService session)
    {
        throw new NotImplementedException();
        Context = context;
        Transaction = transaction;
        Session = session;
    }
    public async Task<ArchivationAction> Begin(RequestBase request, bool isManaged = false)
    {
        var record = new ArchivationRecord()
        {
            ArchiveAction = request.GetType().Name,
            AffiliationGuid = Session.AffiliationGuid,
            IsManaged = isManaged,
            Time = DateTime.UtcNow            
        };
        //Context.DbContext.ArchivationRecords.Add(record);
        Context.RegisterCreatedState(record, x => nameof(ArchivationRecord));
        await Transaction.SaveWithRollback(request);
        return new ArchivationAction(record);
    }
    public UndoArchivationAction UndoArchivation<T>(Guid archivationId) where T : StateBase, IStateArchivable
        => new UndoArchivationAction(Context, Transaction, archivationId).UndoArchivation<T>();
}
public class ArchivationAction
{
    public ArchivationRecord Record { get; }
    public ArchivationAction(ArchivationRecord record)
    {
        Record = record;
    }
    public ArchivationAction Archive<T>(T item) where T : StateBase, IStateArchivable
    {
        item.ArchivationId = Record.Id;
        item.IsArchived = true;
        return this;
    }
    public ArchivationAction Archive<T>(IEnumerable<T> items) where T : StateBase, IStateArchivable
    {
        foreach (var item in items)
        {
            Archive(item);
        }
        return this;
    }
}
public class UndoArchivationAction
{
    protected IRepositoryContext Context { get; }
    protected ITransactionService Transaction { get; }
    protected Guid ArchivationId { get; }
    protected List<Func<Task>> UndoTasks { get; }
    public UndoArchivationAction(IRepositoryContext context, ITransactionService transaction, Guid archivationId)
    {
        Context = context;
        Transaction = transaction;
        ArchivationId = archivationId;
        UndoTasks = new();
    }
    public UndoArchivationAction UndoArchivation<T>() where T : StateBase, IStateArchivable
    {
        UndoTasks.Add(UndoArchivationTask<T>);
        return this;
    }
    protected async Task UndoArchivationTask<T>() where T : StateBase, IStateArchivable
    {
        var archivedItems = await Context.DbContext.Set<T>().Where(x => x.ArchivationId == ArchivationId).ToListAsync();
        archivedItems.ForEach(x =>
        {
            x.ArchivationId = default;
            x.IsArchived = false;
        });
    }
    public async Task Perform(RequestBase request)
    {
        //var record = await Context.DbContext.ArchivationRecords.SingleAsync(x => x.Id == ArchivationId);
        //Context.DbContext.ShowOnlyArchived = true;
        foreach (var undoTask in UndoTasks)
            await undoTask();
        //Context.DbContext.ShowOnlyArchived = false;
        //Context.DbContext.ArchivationRecords.Remove(record);
        await Transaction.SaveWithRollback(request);
    }
}
