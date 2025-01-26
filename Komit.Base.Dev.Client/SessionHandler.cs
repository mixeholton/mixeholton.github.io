using Microsoft.JSInterop;

namespace Komit.Base.Dev.Client;

public class SessionHandler
{
    protected IJSRuntime JSRuntime { get; }
    public string Token { get; private set; }
    public SessionHandler(IJSRuntime iJSRuntime)
    {
        JSRuntime = iJSRuntime;
    }
    public async Task Initialize()
    {
        if (Token != default)
            return;
        Token = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "x_komit_token");
    }
    public async Task SetToken(string token)
    {
        await JSRuntime.InvokeVoidAsync("localStorage.setItem", "x_komit_token", token);
        Token = token;
    }
}