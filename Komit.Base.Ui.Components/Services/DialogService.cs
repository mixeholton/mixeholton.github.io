using Komit.Base.Ui.Components.Components.Dialog;
using Komit.Base.Ui.Components.Components.PageLayout;
using Microsoft.AspNetCore.Components;
namespace Komit.Base.Ui.Components.Services;
public class DialogService
{
    public record DialogInstance(Type Type, Dictionary<string, object> Parameters);
    public DialogInstance? Current { get; set; }
    public List<DialogInstance> Dialogs { get; set; } = new();
    public Func<bool, Task> OnOpenNext { get; set; }
    public async void OpenMessage(string title, string message, Action<bool> result, object reciever)
    {
        Dialogs.Add(new DialogInstance(typeof(DialogTemplate), new()
        {
            { nameof(DialogTemplate.Title), title },
            { nameof(DialogTemplate.Message), message },
            {
                nameof(DialogTemplate.OnClose),
                EventCallback.Factory.Create(reciever, result)
            }
        }));
        await OpenNext();
    }
    public async Task OpenQuestionDialog(string title, string message, string yes, string no, Func<bool, Task> OnClose)
    {
        OnClose += async (b) => CloseCurrent();
        Dialogs.Add(new DialogInstance(typeof(EnsureActionDialog), new()
            {
                { nameof(EnsureActionDialog.Title), title },
                { nameof(EnsureActionDialog.Message), message },
                { nameof(EnsureActionDialog.YesText), yes },
                { nameof(EnsureActionDialog.NoText), no },
                {
                    nameof(EnsureActionDialog.OnClose),
                    OnClose
                }
            }));
        await OpenNext();
    }
    public async Task OpenConfirmationDialog(string title, string message, string CloseText)
    {
        Dialogs.Add(new DialogInstance(typeof(ConfirmationDialog), new()
            {
                { nameof(ConfirmationDialog.Title), title },
                { nameof(ConfirmationDialog.Message), message },
                { nameof(ConfirmationDialog.CloseText), CloseText },
                {
                    nameof(EnsureActionDialog.OnClose),
                    CloseCurrent
                }
            }));
        await OpenNext();
    }
    public async Task OpenConfirmationDialog(string title, string message, string CloseText, Func<Task> OnClose)
    {
        OnClose += CloseCurrent;
        Dialogs.Add(new DialogInstance(typeof(ConfirmationDialog), new()
            {
                { nameof(ConfirmationDialog.Title), title },
                { nameof(ConfirmationDialog.Message), message },
                { nameof(ConfirmationDialog.CloseText), CloseText },
                {
                    nameof(EnsureActionDialog.OnClose),
                    OnClose
                }
            }));
        await OpenNext();
    }
    protected async Task OpenNext()
    {
        Current = Dialogs.LastOrDefault();
        await OnOpenNext.Invoke(true);
    }
    public async Task CloseCurrent()
    {
        if (Dialogs.Any() && Current != null)
            Dialogs.Remove(Current);
        await OpenNext();
    }
}
