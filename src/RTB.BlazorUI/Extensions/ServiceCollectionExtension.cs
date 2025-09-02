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
    /// <summary>
    /// Provides extension methods for registering RTB Blazor services into the DI container.
    /// </summary>
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Registers RTB Blazor services and styling into the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The service collection to register services into.</param>
        /// <param name="config">
        /// A configuration action used to specify which RTB services to include and which theme to use.
        /// </param>
        /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
        /// <remarks>
        /// This method always registers the styled infrastructure via <c>UseRTBStyled()</c>.
        /// Optional services such as Busy Tracker, Dialog, Drag&amp;Drop, Data Navigation, and Input are
        /// registered based on the flags set via <see cref="RTBConfig"/>.
        /// </remarks>
        /// <example>
        /// <code>
        /// builder.Services.UseRTBBlazor(cfg => cfg
        ///     .UseTheme&lt;MyTheme&gt;()
        ///     .UseDefaultServices());
        /// </code>
        /// </example>
        public static IServiceCollection UseRTBBlazor(this IServiceCollection services, Action<RTBConfig> config)
        {
            // PSEUDOCODE PLAN:
            // 1) Ensure RTB styled infrastructure is registered.
            // 2) Create a new RTBConfig and invoke the provided configuration action.
            // 3) If a theme type is specified, register the IThemeService for that theme.
            // 4) Conditionally register optional services based on RTBConfig flags:
            //    - BusyTracker
            //    - DialogService
            //    - DragDropService
            //    - DataNavigationService
            //    - InputService
            // 5) Return the updated IServiceCollection for chaining.

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

        /// <summary>
        /// Configuration class for selecting theme and enabling optional RTB Blazor services.
        /// </summary>
        public class RTBConfig
        {
            /// <summary>
            /// The selected theme type implementing <see cref="ITheme"/> to be used for theming.
            /// Defaults to <see cref="ITheme"/> if not explicitly set via <see cref="UseTheme{T}"/>.
            /// </summary>
            internal Type ThemeType { get; private set; } = typeof(ITheme);

            /// <summary>
            /// Specifies the concrete theme type to be used for theming.
            /// </summary>
            /// <typeparam name="T">A type that implements <see cref="ITheme"/>.</typeparam>
            /// <returns>The same <see cref="RTBConfig"/> instance for chaining.</returns>
            /// <example>
            /// <code>
            /// cfg.UseTheme&lt;MyCustomTheme&gt;();
            /// </code>
            /// </example>
            public RTBConfig UseTheme<T>() where T : ITheme
            {
                ThemeType = typeof(T);
                return this;
            }

            /// <summary>
            /// Enables all optional RTB services:
            /// Busy Tracker, Dialog, Drag&amp;Drop, Data Navigation, and Input.
            /// </summary>
            /// <returns>The same <see cref="RTBConfig"/> instance for chaining.</returns>
            /// <example>
            /// <code>
            /// cfg.UseDefaultServices();
            /// </code>
            /// </example>
            public RTBConfig UseDefaultServices()
            {
                return UseBusyTracker()
                    .UseDialogService()
                    .UseDragDropService()
                    .UseDataNavigationService()
                    .UseInputService();
            }

            /// <summary>
            /// Indicates whether the Busy Tracker service should be registered.
            /// </summary>
            internal bool BusyTracker { get; private set; } = false;

            /// <summary>
            /// Enables the Busy Tracker service.
            /// </summary>
            /// <returns>The same <see cref="RTBConfig"/> instance for chaining.</returns>
            public RTBConfig UseBusyTracker()
            {
                BusyTracker = true;
                return this;
            }

            /// <summary>
            /// Indicates whether the Dialog service should be registered.
            /// </summary>
            internal bool DialogService { get; private set; } = false;

            /// <summary>
            /// Enables the Dialog service.
            /// </summary>
            /// <returns>The same <see cref="RTBConfig"/> instance for chaining.</returns>
            public RTBConfig UseDialogService()
            {
                DialogService = true;
                return this;
            }

            /// <summary>
            /// Indicates whether the Drag &amp; Drop service should be registered.
            /// </summary>
            internal bool DragDropService { get; private set; } = false;

            /// <summary>
            /// Enables the Drag &amp; Drop service.
            /// </summary>
            /// <returns>The same <see cref="RTBConfig"/> instance for chaining.</returns>
            public RTBConfig UseDragDropService()
            {
                DragDropService = true;
                return this;
            }

            /// <summary>
            /// Indicates whether the Data Navigation service should be registered.
            /// </summary>
            internal bool DataNavigationService { get; private set; } = false;

            /// <summary>
            /// Enables the Data Navigation service.
            /// </summary>
            /// <returns>The same <see cref="RTBConfig"/> instance for chaining.</returns>
            public RTBConfig UseDataNavigationService()
            {
                DataNavigationService = true;
                return this;
            }

            /// <summary>
            /// Indicates whether the Input service should be registered.
            /// </summary>
            internal bool InputService { get; private set; } = false;

            /// <summary>
            /// Enables the Input service.
            /// </summary>
            /// <returns>The same <see cref="RTBConfig"/> instance for chaining.</returns>
            public RTBConfig UseInputService()
            {
                InputService = true;
                return this;
            }
        }
    }
}
