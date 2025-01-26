namespace Komit.Sandbox.App.Domain;
public class WorkItem : AggregateBase<WorkItemState>
{
    protected override string _creationCodeName => State.Title;
    protected override Func<IQueryable<WorkItemState>, IQueryable<WorkItemState>> IncludedRelations => null;

    public WorkItem New(string title)
    {
        EnsureIsNew();
        State.Title = title;
        return this;
    }
}
