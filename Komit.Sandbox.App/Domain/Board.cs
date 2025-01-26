namespace Komit.Sandbox.App.Domain;
public class Board : AggregateBase<BoardState>
{
    protected override string _creationCodeName => nameof(Board);
    protected override Func<IQueryable<BoardState>, IQueryable<BoardState>> IncludedRelations => x => x.Include(b => b.WorkItems);

    public Board AddWorkItem(string title)
    {
        if (State.WorkItems.Any(x => x.Title == title))
            throw new DomainException(nameof(title) + ": " + title + " already exists", "");
        AddNewChild(new WorkItem(), State.WorkItems).New(title);
        return this;
    }
}
