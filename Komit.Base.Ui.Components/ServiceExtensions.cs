// Register MudBlazor services in a shared method.
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;

namespace Komit.Base.Ui.Components;
public static class ServiceExtensions
{
    public static IServiceCollection AddBaseUiComponents(this IServiceCollection services)
    {
        services.AddMudServices();
        services.AddMudBlazorDialog();
        services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
            config.SnackbarConfiguration.PreventDuplicates = false;
            config.SnackbarConfiguration.NewestOnTop = false;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
        });
        return services;
    }
}
