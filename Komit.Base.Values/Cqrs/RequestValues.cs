namespace Komit.Base.Values.Cqrs;
public record RequestBase
{
    private readonly string _description;
    public RequestBase(string? description = null)
    {
        _description = description ?? GetType().Name;
    }
    public string GetDescription() => _description;
}
public record RequestBase<T>: RequestBase
{
    public RequestBase(string description): base(description) { }
}
public interface IRequestResult
{
    bool Success { get; }
    string Description { get; }
    IEnumerable<Message> Messages { get; }
}
public record RequestResult<T>(bool Success, T Result, string Description, IEnumerable<Message> Messages): IRequestResult;