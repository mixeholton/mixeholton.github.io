namespace Komit.Sandbox.App.Domain;
public class Wine : AggregateBase<WineState>
{
    protected override string _creationCodeName => State.Name;
    protected override Func<IQueryable<WineState>, IQueryable<WineState>> IncludedRelations => null;

    public Wine New(string name)
    {
        State.Name = name;
        return this;
    }
}
