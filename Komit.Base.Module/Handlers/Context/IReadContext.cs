namespace Komit.Base.Module.Handlers.Context;
public interface IReadContext
{
    IQueryable<T> Query<T>() where T : StateBase;
    //bool ShowOnlyArchived { get; set; }
}
