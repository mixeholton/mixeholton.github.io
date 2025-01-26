using Microsoft.AspNetCore.Components;

namespace Komit.Base.Ui.Components.Components.PageLayout;

public interface IKomitPage
{
    string Title { get; }
    // Set Shortcuts
    bool IsBusy { get; }
    Task Refresh();

}
