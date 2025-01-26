namespace Komit.Base.Values.Cqrs;
public interface IKomitApp
{
    Task<EventResult> Handle<TEvent>(TEvent @event) where TEvent : EventBase;
    Task<RequestResult<object>> Perform(string moduleName, string requestName, string requestJson, bool useCamelCase = false);
    Task<CommandResult> Perform<TCommand>(TCommand command) where TCommand : CommandBase;
    Task<QueryResult<TValue>> Perform<TQuery, TValue>(TQuery request) where TQuery : QueryBase<TValue>;
    Task<RequestResult<TValue>> Request<TRequest, TValue>(TRequest request) where TRequest : RequestBase;
}
