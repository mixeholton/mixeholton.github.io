namespace Komit.Sandbox.App.Domain;
public class Cycle : AggregateBase<CycleState>
{
    protected override string _creationCodeName => State.Brand;
    protected override Func<IQueryable<CycleState>, IQueryable<CycleState>> IncludedRelations => null;
    public Cycle New(string brand, string color, int size)
    {
        State.Brand = brand;
        State.Color = color;
        State.Size = size;
        return this;
    }
}
