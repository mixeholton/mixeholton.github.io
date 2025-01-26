
namespace Komit.Sandbox.App.Domain;
public class Test : AggregateBase<TestState>
{
    protected override string _creationCodeName => State.Name;
    protected override Func<IQueryable<TestState>, IQueryable<TestState>> IncludedRelations => x => x.Include(t => t.SubItems);

    public Test New(string name)
    {
        State.Name = name;
        State.Test = nameof(Test);
        State.CreatedDateTime = DateTime.UtcNow;
        State.CreatedDate = DateOnly.FromDateTime(DateTime.UtcNow);
        State.CreatedTime = TimeOnly.FromDateTime(DateTime.UtcNow);
        AddNewChild(new TestSubItemState() { Name = name + "SubItem"}, State.SubItems);
        return this;
    }
}
