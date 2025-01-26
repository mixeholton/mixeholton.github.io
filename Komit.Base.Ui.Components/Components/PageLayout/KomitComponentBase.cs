using Microsoft.AspNetCore.Components;

namespace Komit.Base.Ui.Components.Components.PageLayout;

public class KomitComponentBase : ComponentBase
{
    public bool IsBusy { get; protected set; } = false;
    protected Func<Task>? RefreshAction { get; set; }
    public virtual async Task Refresh()
    {
        if (RefreshAction != null)
            await BusyWhile(RefreshAction);
        StateHasChanged();
    }
    protected async Task BusyWhile(Func<Task> action)
    {
        try
        {
            IsBusy = true;
            await action();
        }
        finally
        {
            IsBusy = false;
        }
    }
}
