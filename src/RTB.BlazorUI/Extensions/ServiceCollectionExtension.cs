using Microsoft.Extensions.DependencyInjection;
using RTB.Blazor.UI.Services.Dialog;
using RTB.Blazor.UI.Services.BusyTracker;
using RTB.Blazor.UI.Services.DragDrop;
using RTB.Blazor.Styled.Helper;
using RTB.Blazor.Styled;
using RTB.Blazor.Styled.Extensions;
using RTB.Blazor.UI.Services.DataNavigationService;
using RTB.Blazor.Styled.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace RTB.Blazor.UI.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection UseRTBUI(this IServiceCollection collection)
        {
            collection.UseRTBStyled();

            collection.TryAddScoped<IDialogService, DialogService>();
            collection.TryAddScoped<IBusyTracker, BusyTracker>();
            collection.TryAddScoped<IDragDropService, DragDropService>();
            collection.TryAddScoped<DataNavigationService>();

            return collection;
        }
    }
}
