using Microsoft.Extensions.DependencyInjection;
using RTB.Blazor.Styled.Extensions;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RTB.Blazor.Services.BusyIndicator;
using RTB.Blazor.Services.Dialog;
using RTB.Blazor.Services.DragDrop;
using RTB.Blazor.Services.DataNavigation;
using RTB.Blazor.Services.Input;
using RTB.Blazor.Services.Theme;
using System.Threading.Channels;

namespace RTB.Blazor.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection UseRTBBlazor(this IServiceCollection services, Action<RTBConfig> config)
        {
            services.UseRTBStyled();

            var cfg = new RTBConfig();
            config(cfg);

            if (cfg.ThemeType != null)
                services.TryAddScoped(typeof(IThemeService<>).MakeGenericType(cfg.ThemeType), typeof(RTBThemeService<>).MakeGenericType(cfg.ThemeType));

            if (cfg.BusyTracker)
                services.TryAddScoped<IBusyTracker, BusyTracker>();
            if (cfg.DialogService)
                services.TryAddScoped<IDialogService, DialogService>();
            if (cfg.DragDropService)
                services.TryAddScoped<IDragDropService, DragDropService>();
            if (cfg.DataNavigationService)
                services.TryAddScoped<IDataNavigationService, DataNavigationService>();
            if (cfg.InputService)
                services.TryAddScoped<IInputService, InputService>();

            return services;
        }

        public class RTBConfig
        {
            internal Type ThemeType { get; private set; } = typeof(ITheme);
            public RTBConfig UseTheme<T>() where T : ITheme
            {
                ThemeType = typeof(T);
                return this;
            }

            public RTBConfig UseServices()
            {
                return UseBusyTracker()
                    .UseDialogService()
                    .UseDragDropService()
                    .UseDataNavigationService()
                    .UseInputService();
            }

            internal bool BusyTracker { get; private set; } = false;
            public RTBConfig UseBusyTracker()
            {
                BusyTracker = true;
                return this;
            }

            internal bool DialogService { get; private set; } = false;
            public RTBConfig UseDialogService()
            {
                DialogService = true;
                return this;
            }

            internal bool DragDropService { get; private set; } = false;
            public RTBConfig UseDragDropService()
            {
                DragDropService = true;
                return this;
            }

            internal bool DataNavigationService { get; private set; } = false;
            public RTBConfig UseDataNavigationService()
            {
                DataNavigationService = true;
                return this;
            }

            internal bool InputService { get; private set; } = false;
            public RTBConfig UseInputService()
            {
                InputService = true;
                return this;
            }
        }
    }
}
