using Microsoft.Extensions.DependencyInjection;
using RTB.BlazorUI.Services.Dialog;
using RTB.BlazorUI.Services.BusyTracker;
using RTB.BlazorUI.Services.DragDrop;
using RTB.BlazorUI.Services.Theme;
using RTB.BlazorUI.Services.Theme.Themes;
using BlazorStyled;

namespace RTB.BlazorUI.Services
{
    public static class RTBServiceCollection
    {
        public static IServiceCollection UseRTBServices(this IServiceCollection collection)
        {
            collection.AddBlazorStyled();

            return collection
                .AddScoped<IDialogService, DialogService>()
                .AddScoped<IBusyTracker, BusyTracker.BusyTracker>()
                .AddScoped<IDragDropService, DragDropService>()
                .AddScoped<IRTBTheme, RTBLightTheme>()
                .AddScoped<IRTBTheme, RTBDarkTheme>()
                .AddScoped<IThemeService<IRTBTheme>, RTBThemeService<IRTBTheme>>();
        }
    }
}
