namespace Komit.Base.Ui.Helpers;

using Komit.Base.Ui.Components;
using Komit.Base.Ui.Components.Components.Dialog;
using Microsoft.AspNetCore.Components;
using MudBlazor;

public static class DialogHelper
{

    public static (DialogParameters<InfoDialog> Parameters, DialogOptions Options) DefaultInfoDialogSettings(
        string ContentText,
        string OkButtonText = "Ok",
        string CancelButtonText = "Fortryd",
        Color ButtonColor = Color.Primary,
        bool CancelButtonVisible = true,
        DefaultFocusButton DefaultFocusButton = DefaultFocusButton.Cancel,
        Color TextColor = Color.Default,
        DialogPosition DialogPosition = DialogPosition.Center,
        bool CloseTopRightButton = false)
    {

        DialogOptions options = new DialogOptions()
        {
            BackdropClick = false,
            NoHeader = false,
            CloseOnEscapeKey = true,
            Position = DialogPosition,
            CloseButton = CloseTopRightButton
        };
        var parameters = new DialogParameters<InfoDialog>
        {
            { x => x.ContentText, ContentText },
            { x => x.ButtonText, OkButtonText },
            { x => x.ButtonColor, ButtonColor },
            { x => x.CancelButtonVisible, CancelButtonVisible },
            { x => x.CancelButtonText, CancelButtonText },
            { x => x.DefaultFocusButton, DefaultFocusButton},
            { x => x.TextColor, TextColor },

        };
        return (parameters, options);
    }
    public static (DialogParameters<CustomDialog> Parameters, DialogOptions Options) DefaultCustomDialogSettings(
        RenderFragment Page,
        Func<Task>? OnSubmit,
        bool IsReadOnly = false,
        MaxWidth MaxWidth = MaxWidth.Medium,
        bool FullWidth = false,
        string OkButtonText = "Ok",
        string CancelButtonText = "Fortryd",
        Color ButtonColor = Color.Primary,
        bool CancelButtonVisible = true,
        DefaultFocusButton DefaultFocusButton = DefaultFocusButton.Cancel,
        Color TextColor = Color.Default,
        DialogPosition DialogPosition = DialogPosition.Center,
        bool CloseTopRightButton = false)
    {

        DialogOptions options = new DialogOptions()
        {
            BackdropClick = false,
            NoHeader = false,
            CloseOnEscapeKey = true,
            Position = DialogPosition,
            CloseButton = CloseTopRightButton,
            BackgroundClass = "",
            FullScreen = false,
            MaxWidth = MaxWidth,
            FullWidth = FullWidth
        };
        var parameters = new DialogParameters<CustomDialog>
        {
            { x => x.Page, Page },
            { x => x.OnSubmit, OnSubmit },
            { x => x.IsReadOnly, IsReadOnly },
            { x => x.ButtonText, OkButtonText },
            { x => x.ButtonColor, ButtonColor },
            { x => x.CancelButtonVisible, CancelButtonVisible },
            { x => x.CancelButtonText, CancelButtonText },
            { x => x.DefaultFocusButton, DefaultFocusButton},
            { x => x.TextColor, TextColor }
        };
        return (parameters, options);
    }

    public static async Task Submit(IMudDialogInstance? mudDialog, Func<Task>? onSubmit = null)
    {
        if (onSubmit != null)
        {
            await onSubmit.Invoke();
        }
        mudDialog.Close();
    }
    public static async Task Cancel(IMudDialogInstance? mudDialog) => mudDialog.Close(DialogResult.Cancel());
}

