using Komit.Base.Values;
using Komit.Base.Values.Cqrs;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Komit.Infrastructure.CqrsClient;
public class CqrsClient
{
    //ToDo create subclasses for each module with modulename string values
    public record QueryTest(): QueryBase<IdNamePair>(nameof(QueryTest)) { }
    public static string ControllerAction { get; set; } = "api/perform";
    protected IHttpClientFactoryAdaptor HttpFactory { get; }
    protected ICqrsClientErrorHandler ErrorHandler { get; }
    protected string DefaultClientName { get; init; } = Options.DefaultName;
    protected string DefaultPathPrefix { get; init; } = string.Empty;
    public CqrsClient(IHttpClientFactoryAdaptor http, ICqrsClientErrorHandler errorHandler)
    {
        HttpFactory = http;
        ErrorHandler = errorHandler;
    }
    public HttpClient Http(string? name = default)
        => HttpFactory.CreateClient(name ?? DefaultClientName)
        ?? throw new DomainException("Missing http configuration", "No IHttpClientFactory was set for CqrsClient, name: " + name);
    public CqrsClient ForClient(string name)
        => new(HttpFactory, ErrorHandler) { DefaultClientName = name, DefaultPathPrefix = DefaultPathPrefix};
    public CqrsClient ForModule(string name)
        => new(HttpFactory, ErrorHandler) { DefaultClientName = DefaultClientName, DefaultPathPrefix = name };
    public CqrsClient For(string client, string module)
        => new(HttpFactory, ErrorHandler) { DefaultClientName = client, DefaultPathPrefix = module };
    public RequestAction<CommandResult, IEnumerable<IdNamePair>> Command(CommandBase command)
        => new(Http(DefaultClientName), ErrorHandler, command, DefaultPathPrefix);
    public RequestAction<QueryResult<TResult>, TResult> Query<TResult>(QueryBase<TResult> query)
        => new(Http(DefaultClientName), ErrorHandler, query, DefaultPathPrefix);
    /// <summary>
    /// Requests should only be used for performance optimization purposes
    /// When its nescessary to combine commands & queries in a single call
    /// </summary>
    public RequestAction<RequestResult<TResult>, TResult> Request<TResult>(RequestBase<TResult> request)
        => new(Http(DefaultClientName), ErrorHandler, request, DefaultPathPrefix);
    public record RequestAction<TResult, TValue> : CqrsRequest<RequestBase, TResult>
        where TResult : RequestResult<TValue>
    {
        public RequestAction(HttpClient http, ICqrsClientErrorHandler errorHandler, RequestBase request, string? pathPrefix)
            : base(http, errorHandler, request, pathPrefix) { }
        public async Task<bool> Success(bool useErrorHandling = true)
        {
            return (await Response(useErrorHandling))?.Success ?? false;
        }
        public async Task<TValue> Result(bool useErrorHandling = true)
        {
            var response = await Response(useErrorHandling);
            return response == null ? default : response.Result;
        }
    }
    public abstract record CqrsRequest<TAction, TResult>
        where TAction : RequestBase
        where TResult : IRequestResult
    {
        protected HttpClient Http { get; }
        protected ICqrsClientErrorHandler ErrorHandler { get;}
        protected TAction Action { get; }
        protected string? PathPrefix { get; }
        protected CqrsRequest(HttpClient http, ICqrsClientErrorHandler errorHandler, TAction action, string? pathPrefix)
        {
            Http = http;
            ErrorHandler = errorHandler;
            Action = action;
            PathPrefix = pathPrefix;
        }
        protected HttpRequestMessage BuildRequest()
        {
            if (string.IsNullOrWhiteSpace(PathPrefix))
                throw new ArgumentException($"Module name not set for api client");
            // Consider allowing queries to us httpGet by sending the query instance as a header json object instead of using content
            var actionType = Action.GetType();
            return new(HttpMethod.Post, $"{PathPrefix}/{ControllerAction}/{actionType.Name}")
            {
                Content = new StringContent(JsonSerializer.Serialize(Action, actionType), Encoding.UTF8, "application/json")
            };
        }
        protected Message[] GetErrorMessages(Exception exception)
        {
            exception = exception.GetDeepestDomainException();
            return new Message[1]
            {
                exception is DomainException
                    ? (exception as DomainException)!.GetMessage
                    : new Message(exception.GetType().Name, exception.Message)
            };
        }
        public async Task<TResult?> Response(bool useErrorHandling = true) // ToDo replace useErrorHandling with a Error(Request)HandlingConfig object
        {
            var success = false;
            Message[] messages = Array.Empty<Message>();
            try
            {
                var response = await Http.SendAsync(BuildRequest());

                if (response.IsSuccessStatusCode || (int)response.StatusCode == 500)
                {
                    var result = await response.Content.ReadFromJsonAsync<TResult>();
                    messages = result?.Messages.ToArray() ?? Array.Empty<Message>();
                    success = response.IsSuccessStatusCode && (result?.Success ?? false);
                    return result;
                }
                else
                {
                    ErrorHandler.HandleUnknownError(response);
                }
            }
            catch (Exception exception)
            {
                messages = GetErrorMessages(exception);
            }
            finally
            {
                if (success || (!success && useErrorHandling) && messages.Any())
                    ErrorHandler.ShowMessages(Action.GetDescription(), success, messages);
            }
            return default;
        }
    }
}
