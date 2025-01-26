namespace Komit.Base.Values.Cqrs;
public record QueryBase : RequestBase
{
    public QueryBase(string? description = null) : base(description) { }
}
public record QueryBase<T> : QueryBase
{
    public QueryBase(string? description = null) : base(description) { }
}
public record QueryResult<T>(bool Success, T Result, string Description, IEnumerable<Message> Messages) :
    RequestResult<T>(Success, Result, Description, Messages);