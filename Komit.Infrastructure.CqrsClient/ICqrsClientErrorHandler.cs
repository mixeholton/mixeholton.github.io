using Komit.Base.Values;
namespace Komit.Infrastructure.CqrsClient;
public interface ICqrsClientErrorHandler
{
    void ShowMessages(string title, bool success, Message[] messages);
    void HandleUnknownError(HttpResponseMessage response);
}
