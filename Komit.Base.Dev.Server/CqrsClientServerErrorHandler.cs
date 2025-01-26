using Komit.Base.Values;
using Komit.Infrastructure.CqrsClient;

namespace Komit.Base.Dev.Server;
public class CqrsClientServerErrorHandler : ICqrsClientErrorHandler
{
    public void HandleUnknownError(HttpResponseMessage response)
    {

    }

    public void ShowMessages(string title, bool success, Message[] messages)
    {
        Console.WriteLine($"{title}{Environment.NewLine}{string.Join(Environment.NewLine, messages.Select(x => x.Title + ". " + x.Details))}");
    }
}
