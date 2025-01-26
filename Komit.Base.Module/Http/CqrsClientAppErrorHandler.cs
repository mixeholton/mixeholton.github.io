using Komit.Base.Values;
using Komit.Infrastructure.CqrsClient;

namespace Komit.Base.Module.Http;
public class CqrsClientAppErrorHandler : ICqrsClientErrorHandler
{
    public void HandleUnknownError(HttpResponseMessage response)
    {
        Console.WriteLine($"Unknown error(HTTP {(int)response.StatusCode} - {response.StatusCode})");
    }

    public void ShowMessages(string title, bool success, Message[] messages)
    {
        Console.WriteLine($"{title}{Environment.NewLine}{string.Join(Environment.NewLine, messages.Select(x => x.Title + ". " + x.Details))}");
    }
}
