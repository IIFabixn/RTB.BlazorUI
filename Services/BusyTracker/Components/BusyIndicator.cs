using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using RTB.BlazorUI.Components;
using RTB.BlazorUI.Services.BusyTracker;

namespace RTB.BlazorUI.Services.BusyTracker.Components
{
    public class BusyIndicator(BusyTracker tracker) : RTBComponent, IDisposable
    {
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public string? TrackId { get; set; }

        protected override void OnInitialized()
        {
            tracker.OnBusyChanged += StateHasChanged;
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (!tracker.IsAnyBusy) return; // No busy state, do not render anything

            if (!string.IsNullOrEmpty(TrackId) && !tracker.IsBusy(TrackId)) return; // No busy state for this TrackId, do not render anything

            if (ChildContent is null)
            {
                // Render the default busy tracker if no ChildContent is provided
                builder.OpenComponent<DefaultBusyTracker>(0);
                builder.CloseComponent();
            }
            else
            {
                // Render the provided ChildContent
                builder.AddContent(0, ChildContent);
            }
        }

        public void Dispose()
        {
            tracker.OnBusyChanged -= StateHasChanged;
        }
    }
}
