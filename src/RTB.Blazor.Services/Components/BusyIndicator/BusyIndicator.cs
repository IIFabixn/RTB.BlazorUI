using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using RTB.Blazor.Core;
using RTB.Blazor.Services.Services;

namespace RTB.Blazor.Services.Components.BusyIndicator
{
    public class BusyIndicator : RTBComponent, IDisposable
    {
        [Inject] public IBusyTracker Tracker { get; set; } = default!;
        [Parameter] public RenderFragment? BusyContent { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public string? TrackId { get; set; }

        protected override void OnInitialized()
        {
            Tracker.OnBusyChanged += OnBusyChange;
        }

        private void OnBusyChange(string? key)
        {
            if (string.IsNullOrEmpty(TrackId) || key == TrackId) StateHasChanged();
            else Console.WriteLine(string.Join("\n", Tracker.Tracks));
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender) OnBusyChange(TrackId);
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (!Tracker.IsBusy(TrackId))
            {
                // Render the child content if not busy for the specified TrackId
                ChildContent?.Invoke(builder);
                return;
            }

            if (BusyContent is not null)
            {
                // Render the provided BusyContent
                BusyContent.Invoke(builder);
            }
            else 
            {
                var seq = 0;
                builder.OpenElement(seq++, "div");
                builder.AddAttribute(seq++, "title", string.Join("\n", Tracker.Tracks));
                builder.AddContent(seq++, "Busy..");
                builder.CloseComponent();
            }
        }

        public void Dispose()
        {
            Tracker.OnBusyChanged -= OnBusyChange;
            GC.SuppressFinalize(this);
        }
    }
}
