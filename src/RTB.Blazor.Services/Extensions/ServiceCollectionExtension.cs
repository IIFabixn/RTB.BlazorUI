using Microsoft.Extensions.DependencyInjection;
using RTB.Blazor.Styled.Helper;
using RTB.Blazor.Styled;
using RTB.Blazor.Styled.Extensions;
using RTB.Blazor.Styled.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RTB.Blazor.Services.Services;

namespace RTB.Blazor.Services.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection UseRTBServices(this IServiceCollection collection)
        {
            collection.UseRTBStyled();

            collection.UseRTBDialog();
            collection.UseRTBBusyTracker();
            collection.UseRTBDragDrop();
            collection.UseRTBDataNavigation();
            collection.UseRTBInputService();

            return collection;
        }

        public static IServiceCollection UseRTBDialog(this IServiceCollection collection)
        {
            collection.TryAddSingleton<IDialogService, DialogService>();
            return collection;
        }

        public static IServiceCollection UseRTBBusyTracker(this IServiceCollection collection)
        {
            collection.TryAddSingleton<IBusyTracker, BusyTracker>();
            return collection;
        }

        public static IServiceCollection UseRTBDragDrop(this IServiceCollection collection)
        {
            collection.TryAddSingleton<IDragDropService, DragDropService>();
            return collection;
        }

        public static IServiceCollection UseRTBDataNavigation(this IServiceCollection collection)
        {
            collection.TryAddSingleton<DataNavigationService>();
            return collection;
        }

        public static IServiceCollection UseRTBInputService(this IServiceCollection collection)
        {
            collection.TryAddSingleton<IInputService, InputService>();
            return collection;
        }
    }
}
