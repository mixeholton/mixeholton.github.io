using Komit.Base.Ui.Components.Services;
using Komit.Infrastructure.CqrsClient;
using Microsoft.AspNetCore.Components;

namespace Komit.Base.Ui.Components.Components.PageLayout;

public abstract class KomitPageBase : KomitComponentBase, IKomitPage, IDisposable
{
    private AccessService _access;
    [Inject]
    public AccessService Access
    {
        get => _access;
        set => _access = value.OpenPage(this);
    }
    [Inject] public CqrsClient Client { get; set; }
    [Inject] public DialogService Dialog { get; set; }
    public string Title { get; protected set; } = "KOMiT";
    public void Dispose()
    {
        OnDisposing();
        Access.ClosePage(this);
    }
    public virtual void OnDisposing() { }
}
