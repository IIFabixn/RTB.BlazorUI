using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using RTB.BlazorUI.Components;

namespace RTB.BlazorUI.Services.BusyTracker.Components
{
    public class BusyIndicator(IBusyTracker tracker) : RTBComponent, IDisposable
    {
        [Parameter] public RenderFragment? BusyContent { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public string? TrackId { get; set; }

        protected override void OnInitialized()
        {
            tracker.OnBusyChanged += StateHasChanged;
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (!tracker.IsBusy(TrackId)) 
            {
                // Render the child content if not busy for the specified TrackId
                ChildContent?.Invoke(builder);
                return;
            }

            if (BusyContent is null)
            {
                // Render the default busy tracker if no BusyContent is provided
                builder.OpenComponent<DefaultBusyTracker>(0);
                builder.CloseComponent();
            }
            else
            {
                // Render the provided BusyContent
                BusyContent.Invoke(builder);
            }
        }

        public void Dispose()
        {
            tracker.OnBusyChanged -= StateHasChanged;
            GC.SuppressFinalize(this);
        }
    }
}
