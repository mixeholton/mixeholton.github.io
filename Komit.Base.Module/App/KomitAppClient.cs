using Komit.Base.Module.Handlers.Context.Transaction;
using Microsoft.Extensions.DependencyInjection;
namespace Komit.Base.Module.App;
public class KomitAppClient(IServiceProvider services)
{
    /* Consider:
     * Making CqrsClient available
     * Detecting wether to use KomitApp or CqrsClient 
     * based on if request module is added to the app
     */
    protected class KomitAppThrowsOnError(IServiceProvider services) : IKomitApp
    {
        protected IKomitApp App { get; } = new KomitApp(services);
        protected async Task<T> Result<T>(Task<T> task) where T: IRequestResult
        {
            var result = await task;
            return result.Success ? result : throw new DomainException("Del handling fejlede", result.Description);
        }
        public Task<EventResult> Handle<TEvent>(TEvent @event) where TEvent : EventBase => Result(App.Handle(@event));
        public Task<RequestResult<object>> Perform(string moduleName, string requestName, string requestJson, bool useCamelCase = false) => Result(App.Perform(moduleName, requestName, requestJson, useCamelCase));
        public Task<CommandResult> Perform<TCommand>(TCommand command) where TCommand : CommandBase => Result(App.Perform(command));
        public Task<QueryResult<TValue>> Perform<TQuery, TValue>(TQuery request) where TQuery : QueryBase<TValue> => Result(App.Perform<TQuery, TValue>(request));
        public Task<RequestResult<TValue>> Request<TRequest, TValue>(TRequest request) where TRequest : RequestBase => Result(App.Request<TRequest, TValue>(request));
    }
    protected IServiceProvider Services { get; } = services;
    /// <summary>
    /// !!! Notice save any pending changes before
    /// Returns KomitApp for same TransactionScope.
    /// </summary>
    protected IKomitApp App(IServiceProvider services, bool throwErrors) => throwErrors ? new KomitAppThrowsOnError(services) : new KomitApp(services);
    public async Task CurrentScope(Func<IKomitApp, Task> scopedAction, bool throwErrors = true)
    {
        var transactionService = Services.GetRequiredService<ITransactionService>();
        var originalSubScopeValue = transactionService.InSubScope;
        try
        {
            transactionService.InSubScope = true;
            await scopedAction(App(Services, throwErrors));
        }
        finally
        {
            transactionService.InSubScope = originalSubScopeValue;
        }
    }
    public async Task<T> CurrentScope<T>(Func<IKomitApp, Task<T>> scopedAction, bool throwErrors = true)
    {
        var transactionService = Services.GetRequiredService<ITransactionService>();
        var originalSubScopeValue = transactionService.InSubScope;
        try
        {
            transactionService.InSubScope = true;
            return await scopedAction(App(Services, throwErrors));
        }
        finally
        {
            transactionService.InSubScope = originalSubScopeValue;
        }
    }
    /// <summary>
    /// Performs an action on a KomitApp in a new scope with same session.
    /// !!! Notice changes performed here is not part of the parent transaction
    /// and will not be rolled back if other scopes fails
    /// will require compensating actions to rollback
    /// </summary>
    public async Task<RequestResult<TValue>> NewScope<TRequest, TValue>(TRequest request, bool throwErrors = true) where TRequest : RequestBase 
    {
        using var scope = Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<ISetSession>().Set(Services.GetRequiredService<ISessionService>().Value);
        return await App(scope.ServiceProvider, throwErrors).Request<TRequest, TValue>(request);
    }
}
