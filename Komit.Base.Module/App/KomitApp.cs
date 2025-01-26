using Komit.Base.Module.Handlers.Context.Markers;
using Komit.Base.Module.Handlers.Context.Transaction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Komit.Base.Module.App;
public class KomitApp: IKomitApp
{
    private class RequestResultBuilder
    {
        public bool Success { get; set; }
        public dynamic Value { get; set; }
        public string Description { get; set; }
        public IEnumerable<Message> Messages { get; set; }
        public void SetFrom<T>(RequestResult<T> request)
        {
            Success = request.Success;
            Value = request.Result;
            Description = request.Description;
            Messages = request.Messages;
        }
        public void SetSuccessFor<T>(RequestBase request, T value)
        {
            Success = true;
            Value = value;
            Description = request.GetDescription();
            Messages = Array.Empty<Message>();
        }
        public void SetFailureFor(RequestBase request, IEnumerable<Message> messages)
        {
            Success = false;
            Value = null;
            Description = request.GetDescription();
            Messages = messages;
        }
        public CommandResult CommandResult => new(Success, Value, Description, Messages);
        public QueryResult<T> QueryResult<T>() => new(Success, Value, Description, Messages);
        public RequestResult<T> RequestResult<T>() => new(Success, Value, Description, Messages);
        public EventResult EventResult => new(Success, Value, Description, Messages);
    }
    protected IServiceProvider Services { get; }
    public KomitApp(IServiceProvider services)
    {
        Services = services;
    }
    public async Task<CommandResult> Perform<TCommand>(TCommand command)
        where TCommand : CommandBase
    {
        var handler = Services.GetHandler<TCommand>();
        var result = await Write(handler, command);
        return result.CommandResult;
    }
    public async Task<QueryResult<TValue>> Perform<TQuery, TValue>(TQuery request)
        where TQuery : QueryBase<TValue>
    {
        var handler = Services.GetHandler<TQuery>();
        var result = await Read(handler, request);
        return result.QueryResult<TValue>();
    }
    public async Task<RequestResult<TValue>> Request<TRequest, TValue>(TRequest request)
        where TRequest : RequestBase
    {
        var handler = Services.GetHandler<TRequest>();
        var result = await (request is QueryBase ? Read(handler, request) : Write(handler, request));
        return result.RequestResult<TValue>();
    }
    public async Task<RequestResult<object>> Perform(string moduleName, string requestName, string requestJson, bool useCamelCase = false)
    {
        var (request, handler) = Services.GetRequestHandling(moduleName, requestName, requestJson, useCamelCase);
        var result = await (request is QueryBase ? Read(handler, request) : Write(handler, request));
        return result.RequestResult<object>();
    }
    /// <summary>
    /// For integrations events from other modules 
    /// </summary>
    public async Task<EventResult> Handle<TEvent>(TEvent @event)
       where TEvent : EventBase
    {
        var eventType = @event.GetType();
        var handler = Services.GetService(typeof(IRequestMarker<>).MakeGenericType(eventType));
        var result = await Write(handler, @event);
        return result.EventResult;
    }
    private async Task HandleEvents(ITransactionService transaction)
    {
        var events = (await transaction.PublishEvents()).ToList();
        var integrationEvents = new List<EventBase>();
        while (events.Any())
        {
            // Ensure integration events are handled after all domain events
            events.Where(x => x.IsIntegrationEvent).ToList().ForEach(x =>
            {
                events.Remove(x);
                integrationEvents.Add(x);
            });
            // Handle batch events
            foreach (var batch in events.GroupBy(x => x.GetType().AssemblyQualifiedName).Where(x => x.Count() > 1))
            {
                var eventType = batch.First().GetType();
                var handlers = Services.GetServices(typeof(IRequestMarker<>).MakeGenericType(eventType));
                if (handlers.Any() == false)
                    continue; // will be handled as single events
                var batchHandlerType = typeof(IEventBatchHandler<>).MakeGenericType(eventType);
                dynamic list = null;
                foreach (dynamic handler in handlers)
                {
                    if (batchHandlerType.IsAssignableFrom(handler.GetType()))
                    {
                        if(list == null)
                        {
                            list = Activator.CreateInstance(typeof(List<>).MakeGenericType(eventType)); 
                            foreach (dynamic @event in batch)
                            {
                                list.Add(@event);
                            }
                        }
                        await handler.Handle(list);
                    }
                    else
                        foreach (dynamic @event in batch)
                        {
                            await handler.Handle(@event);
                        }

                }
            }
            events = (await transaction.PublishEvents()).ToList();
        }
        // Handle Integration events
        foreach (var @event in integrationEvents)
        {
            var eventType = @event.GetType();
            var handlers = Services.GetServices(typeof(IRequestMarker<>).MakeGenericType(eventType));
            if (handlers.Any())
                foreach (dynamic handler in handlers)
                {
                    await handler.Handle(@event as dynamic);
                }
            else // Unhandled event
            {
                //ToDo log unhandled event
            }
        }
    }
    private async Task<RequestResultBuilder> Read(dynamic queryHandler, dynamic query)
    {
        var result = new RequestResultBuilder();
        try
        {
            result.SetSuccessFor(query, await queryHandler.Perform(query));
        }
        catch (Exception exception)
        {
            var domainException = exception as DomainException;
            var inspector = Services.GetService<IExceptionInspector>();
            if (inspector != null)
            {
                inspector.Thrown(exception, true);
                inspector.UsedForErrorMessage(domainException ?? exception, true);
            }
            var messages = new Message[]
            {
            domainException == default
                ? new Message(exception.GetType().Name, exception.Message)
                : domainException.GetMessage
            };
            result.SetFailureFor(query, messages);
        }
        finally
        {
            // ToDo query result logging
        }
        return result;
    }
    private async Task<RequestResultBuilder> Write(dynamic handler, dynamic request)
    {
        // !!! Important ToDo handle Distinct TransactionServices for event handlers
        // To Ensure All dbContext transactions are committed for cross context event handling 
        ITransactionService transaction = handler.TransactionService;
        var result = new RequestResultBuilder();
        var isCommand = request is CommandBase;
        var isEvent = request is EventBase;
        try
        {
            dynamic requestValue = default;
            if (isCommand)
                await handler.Perform(request);
            else if (isEvent)
                await handler.Handle(request);
            else
                requestValue = await handler.Perform(request);
            await HandleEvents(transaction);
            result.SetFrom(await transaction.Commit(request));
            if (!isCommand && !isEvent)
                result.Value = requestValue;
        }
        catch (Exception handledOuterException)
        {
            var handledException = handledOuterException.GetDeepestDomainException();
            var inspector = Services.GetService<IExceptionInspector>();
            if (inspector != null)
            {
                inspector.Thrown(handledOuterException, true);
                inspector.UsedForErrorMessage(handledException, true);
            }
            var errorMessages = new List<Message>();
            try
            {
                result.SetFrom(await transaction.Rollback(request));
                var domainException = handledException as DomainException;
                errorMessages.Add(domainException == default
                    ? new Message(handledException.GetType().Name, handledException.Message)
                    : domainException.GetMessage);
            }
            catch (Exception unhandledOuterException)
            {
                var unhandledException = unhandledOuterException.GetDeepestDomainException();
                if (inspector != null)
                {
                    inspector.Thrown(unhandledOuterException, false);
                    inspector.UsedForErrorMessage(unhandledException, false);
                }
                result.SetFailureFor(request, new[]
                {
                    new Message(unhandledException.GetType().Name + " rollback failed!", unhandledException.Message)
                });
                // ToDo critical unhandled exception logging
            }
            finally
            {
                // ToDo consider handled exception logging
                // ToDo consider if exceptions should be added as command result messages 
                if (result == default || result.Messages == default || !result.Messages.Any())
                    result.SetFailureFor(request, errorMessages);
                else if (errorMessages.Any())
                {
                    var messages = result.Messages.ToList();
                    messages.AddRange(errorMessages);
                    result.SetFailureFor(request, messages);
                }
            }
        }
        finally
        {
            // ToDo command result logging
        }
        return result;
    }
}
