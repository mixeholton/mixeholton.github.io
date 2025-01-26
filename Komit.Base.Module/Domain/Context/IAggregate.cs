namespace Komit.Base.Module.Domain.Context;
public interface IAggregate
{
    /// <summary>
    /// Supplies a string to identify a newly created aggregate id in a command result
    /// </summary>
    string CreationCodeName { get; }
    void InitializeNew(IAggregateContext context);
}