using Microsoft.Extensions.DependencyInjection;
using RTB.Blazor.Styled.Helper;
using RTB.Blazor.Styled;
using RTB.Blazor.Styled.Extensions;
using RTB.Blazor.Styled.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RTB.Blazor.Services.Services;

namespace RTB.Blazor.UI.Extensions
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

            return collection;
        }

        public static IServiceCollection UseRTBDialog(this IServiceCollection collection)
        {
            collection.TryAddScoped<IDialogService, DialogService>();
            return collection;
        }

        public static IServiceCollection UseRTBBusyTracker(this IServiceCollection collection)
        {
            collection.TryAddScoped<IBusyTracker, BusyTracker>();
            return collection;
        }

        public static IServiceCollection UseRTBDragDrop(this IServiceCollection collection)
        {
            collection.TryAddScoped<IDragDropService, DragDropService>();
            return collection;
        }

        public static IServiceCollection UseRTBDataNavigation(this IServiceCollection collection)
        {
            collection.TryAddScoped<DataNavigationService>();
            return collection;
        }
    }
}
