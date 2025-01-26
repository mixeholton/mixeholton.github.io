using Komit.BoxOfSand.App.State;

namespace Komit.BoxOfSand.App.Domain;
public class Box : AggregateBase<BoxState>
{
    protected override string _creationCodeName => State.Name;
    protected override Func<IQueryable<BoxState>, IQueryable<BoxState>> IncludedRelations => x => x.Include(b => b.Content);

    public Box New(string Name)
    {
        EnsureIsNew();
        State.Name = Name;
        State.Description = nameof(State.Description);
        AddNewChild(new ContentState() { Name = Name + "Content" }, State.Content);
        return this;
    }
}
