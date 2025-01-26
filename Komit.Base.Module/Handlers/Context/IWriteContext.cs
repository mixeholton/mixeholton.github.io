namespace Komit.Base.Module.Handlers.Context;
public interface IWriteContext
{
    Task<CommandResult> SaveWithRollback(RequestBase command);
    //bool ShowOnlyArchived { get; set; }
}
