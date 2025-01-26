namespace Komit.Base.Values.Cqrs;
public abstract record EventBase : RequestBase
{
    public Guid SourceId { get; private set; }
    public bool IsIntegrationEvent { get; private set; }
    public EventBase(string? description = null, bool isIntegrationEvent = false) : base(description) 
    { 
        IsIntegrationEvent = isIntegrationEvent;
    }
    public void SetSourceId(Guid sourceId)
    {
        if (SourceId != default)
            throw new InvalidOperationException(nameof(SourceId));
        SourceId = sourceId;
    }
}
public record EventResult(bool Success, IEnumerable<IdNamePair> Result, string Description, IEnumerable<Message> Messages) :
    RequestResult<IEnumerable<IdNamePair>>(Success, Result, Description, Messages);