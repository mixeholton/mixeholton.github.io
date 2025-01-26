namespace Komit.Sandbox.App.Domain;
public class User : AggregateBase<UserState>
{
    protected override string _creationCodeName => State.Name;

    protected override Func<IQueryable<UserState>, IQueryable<UserState>>? IncludedRelations => null;

    public User New(string Name)
    {
        EnsureIsNew();
        State.Name = Name;
        State.CreatedDateTime = DateTime.UtcNow;
        State.CreatedDate = DateOnly.FromDateTime(DateTime.UtcNow);
        State.CreatedTime = TimeOnly.FromDateTime(DateTime.UtcNow);
        return this;
    }
}
