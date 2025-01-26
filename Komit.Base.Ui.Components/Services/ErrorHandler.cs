using Komit.Base.Values;
using Komit.Infrastructure.CqrsClient;
using Microsoft.JSInterop;

namespace Komit.Base.Ui.Components.Services;
public class ErrorHandler : ICqrsClientErrorHandler
{
    protected IJSRuntime JsRuntime { get; }
    public ErrorHandler(IJSRuntime jsRuntime)
    {
        JsRuntime = jsRuntime;
    }
    public void ShowMessages(string title, bool success, Message[] messages)
    {
        // ToDo show proper error message dialog component here 
        foreach (var message in messages)
        {
            JsRuntime.InvokeVoidAsync("alert",
                title
                + (success ? string.Empty : " fejlede")
                + Environment.NewLine
                + message.Title
                + Environment.NewLine
                + message.Details);
        }
    }
    public void HandleUnknownError(HttpResponseMessage response)
    {
        switch ((int)response.StatusCode)
        {
            case 401:
                var authMsg = new Message[] { new Message("Not Authorized!", "You will now be redirected to the login page...") };
                ShowMessages("Not Authorized!", false, authMsg);
                break;
            default:
                var msg = new Message[] { new Message("An unknown error occurred!", "Please try again...") };
                ShowMessages("Error!", false, msg);
                break;
        }
    }
}
