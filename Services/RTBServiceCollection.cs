using Microsoft.Extensions.DependencyInjection;
using RTB.BlazorUI.Services.Dialog;
using RTB.BlazorUI.Services.BusyTracker;
using RTB.BlazorUI.Services.DragDrop;

namespace RTB.BlazorUI.Services
{
    public static class RTBServiceCollection
    {
        public static IServiceCollection UseRTBServices(this IServiceCollection collection)
        {
            return collection
                .AddScoped<IDialogService, DialogService>()
                .AddScoped<IBusyTracker, BusyTracker.BusyTracker>()
                .AddScoped<IDragDropService, DragDropService>();
        }
    }
}
