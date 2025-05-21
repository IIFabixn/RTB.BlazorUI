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
        public static IServiceCollection UseRTBServices(this IServiceCollection collection, Action<RTBConfig>? configAction = null)
        {
            collection.AddBlazorStyled();
            var config = new RTBConfig();
            configAction?.Invoke(config);

            collection.AddScoped(typeof(IThemeService<>).MakeGenericType(config.ThemeType), typeof(RTBThemeService<>).MakeGenericType(config.ThemeType));

            return collection
                .AddScoped<IDialogService, DialogService>()
                .AddScoped<IBusyTracker, BusyTracker.BusyTracker>()
                .AddScoped<IDragDropService, DragDropService>()
                .AddScoped<DataNavigationService.DataNavigationService>();
        }
    }

    public class RTBConfig
    {
        public Type ThemeType { get; set; } = typeof(ITheme);
    }
}
