using Komit.Base.Ui.Components.Services;
using Microsoft.AspNetCore.Components;

namespace Komit.Base.Ui.Components.Components.PageLayout;

public class KomitLayoutBase : LayoutComponentBase, IKomitLayout, IDisposable
{
    private AccessService _access;
    [Inject]
    public AccessService Access
    {
        get => _access;
        set => _access = value.OpenLayout(this);
    }
    public void Render() => StateHasChanged();
    public void Dispose()
    {
        OnDisposing();
        Access.CloseLayout(this);
    }
    public virtual void OnDisposing() { }
}
