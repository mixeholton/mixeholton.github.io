using Komit.Base.Values;
using Komit.Infrastructure.CqrsClient;

namespace Komit.Base.Specs.Stubs;
public class CqrsClientErrorIgnorer : ICqrsClientErrorHandler
{
    public void HandleUnknownError(HttpResponseMessage response)
    {
    }

    public void ShowMessages(string title, bool success, Message[] messages)
    {
    }
}