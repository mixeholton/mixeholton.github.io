namespace Komit.Base.Values.Cqrs;
public record CommandBase: RequestBase
{
    public CommandBase(string? description = null) : base(description) { }
}
public record CommandResult(bool Success, IEnumerable<IdNamePair> Result, string Description, IEnumerable<Message> Messages) : 
    RequestResult<IEnumerable<IdNamePair>>(Success, Result, Description, Messages)
{
    public IdNamePair GetIdentityFor(string name) => Result?.SingleOrDefault(x => x.Name == name);
    public bool TryGetIdentityFor(string name, out IdNamePair id)
    {
        var ids = Result?.Where(x => x.Name == name)?.ToArray();
        if(ids != null && ids.Length == 1)
        {
            id = ids.First();
            return true;
        }
        id = default;
        return false;
    }
};