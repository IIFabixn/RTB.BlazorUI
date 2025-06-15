using Microsoft.Extensions.DependencyInjection;
using RTB.BlazorUI.Services.Dialog;
using RTB.BlazorUI.Services.BusyTracker;
using RTB.BlazorUI.Services.DragDrop;
using RTB.BlazorUI.Services.Theme;
using RTB.BlazorUI.Services.Theme.Themes;
using RTB.BlazorUI.Services.Style;

namespace RTB.BlazorUI.Services
{
    public static class RTBServiceCollection
    {
        public static IServiceCollection UseRTBServices(this IServiceCollection collection, Action<RTBConfig>? configAction = null)
        {
            var config = new RTBConfig();
            configAction?.Invoke(config);

            if (config.UseThemeService && config.ThemeType is not null)
            {
                var themeServiceType = typeof(RTBThemeService<>).MakeGenericType(config.ThemeType);
                collection.AddScoped(typeof(IThemeService<>).MakeGenericType(config.ThemeType), themeServiceType);
            }

            if (config.UseDialogService)
            {
                collection.AddScoped<IDialogService, DialogService>();
            }

            if (config.UseBusyTracker)
            {
                collection.AddScoped<IBusyTracker, BusyTracker.BusyTracker>();
            }

            if (config.UseDragDropService)
            {
                collection.AddScoped<IDragDropService, DragDropService>();
            }

            if (config.UseDataNavigationService)
            {
                collection.AddScoped<DataNavigationService.DataNavigationService>();
            }

            collection.AddScoped<IStyleRegistry, StyleRegistry>();

            return collection;
        }
    }

    public class RTBConfig
    {
        public bool UseThemeService { get; set; } = true;
        /// <summary>
        /// If defined, <see cref="RTBThemeService{TThemeBase}"/> will be added as a Scoped services.
        /// </summary>
        public Type ThemeType { get; set; } = typeof(ITheme);

        public bool UseDialogService { get; set; } = true;
        public bool UseBusyTracker { get; set; } = true;
        public bool UseDragDropService { get; set; } = true;
        public bool UseDataNavigationService { get; set; } = true;
    }
}
