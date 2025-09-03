using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using RTB.Blazor.Components;

namespace RTB.Blazor.Services.BusyIndicator
{
    /// <summary>
    /// A Blazor component that conditionally renders a "busy" placeholder while work is in progress,
    /// as reported by an injected <see cref="IBusyTracker"/>.
    /// </summary>
    /// <remarks>
    /// Behavior:
    /// - When <see cref="IBusyTracker.IsBusy(string?)"/> returns true for <see cref="TrackId"/>, the component renders
    ///   <see cref="BusyContent"/> if provided; otherwise, a minimal fallback "Busy.." message.
    /// - When not busy, the component renders <see cref="ChildContent"/>.
    /// - If <see cref="TrackId"/> is null or empty, the component reflects the "any key" busy state.
    /// 
    /// Lifecycle:
    /// - Subscribes to <c>IBusyTracker.OnBusyChanged</c> in <see cref="OnInitialized"/> and requests a re-render
    ///   when the relevant key changes.
    /// - On first render, forces a state check to ensure the initial UI reflects the current busy state.
    /// 
    /// Typical usage:
    /// <code>
    /// // In a Razor component:
    /// &lt;BusyIndicator TrackId="LoadData"&gt;
    ///   &lt;BusyContent&gt;
    ///     &lt;div class="spinner"&gt;Loading…&lt;/div&gt;
    ///   &lt;/BusyContent&gt;
    ///   &lt;ChildContent&gt;
    ///     &lt;!-- Normal UI goes here --&gt;
    ///   &lt;/ChildContent&gt;
    /// &lt;/BusyIndicator&gt;
    ///
    /// @code {
    ///   [Inject] IBusyTracker Busy { get; set; } = default!;
    ///
    ///   private async Task LoadData()
    ///   {
    ///       using var _ = Busy.Track(nameof(LoadData)); // Matches TrackId="LoadData"
    ///       await Task.Delay(1000);
    ///   }
    /// }
    /// </code>
    /// </remarks>
    public class BusyIndicator : RTBComponent, IDisposable
    {
        /// <summary>
        /// The busy state tracker used to determine whether to display <see cref="BusyContent"/> or <see cref="ChildContent"/>.
        /// Provided via dependency injection.
        /// </summary>
        [Inject] public IBusyTracker Tracker { get; set; } = default!;

        /// <summary>
        /// Content to render when the component is busy for the specified <see cref="TrackId"/>.
        /// If not supplied, a minimal fallback is rendered.
        /// </summary>
        [Parameter] public RenderFragment? BusyContent { get; set; }

        /// <summary>
        /// Content to render when the component is not busy for the specified <see cref="TrackId"/>.
        /// </summary>
        [Parameter] public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Optional key that scopes the busy state for this component instance.
        /// When null or empty, the component reflects whether any key is busy.
        /// </summary>
        [Parameter] public string? TrackId { get; set; }

        /// <summary>
        /// Subscribes to the tracker's <c>OnBusyChanged</c> event to update the UI when busy state transitions occur.
        /// </summary>
        protected override void OnInitialized()
        {
            Tracker.OnBusyChanged += OnBusyChange;
        }

        /// <summary>
        /// Handles tracker busy state changes. Triggers a re-render when the change is relevant
        /// to this component's <see cref="TrackId"/> (or when <see cref="TrackId"/> is null/empty).
        /// </summary>
        /// <param name="key">The key that changed, as provided by <see cref="IBusyTracker"/>.</param>
        private void OnBusyChange(string? key)
        {
            if (string.IsNullOrEmpty(TrackId) || key == TrackId) StateHasChanged();
            else Console.WriteLine(string.Join("\n", Tracker.Tracks));
        }

        /// <summary>
        /// On first render, ensures the component reflects the current busy state immediately.
        /// </summary>
        /// <param name="firstRender">True when this is the first time the component has rendered.</param>
        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender) OnBusyChange(TrackId);
        }

        /// <summary>
        /// Builds the component's UI based on the current busy state for <see cref="TrackId"/>.
        /// Renders <see cref="ChildContent"/> when not busy; otherwise renders <see cref="BusyContent"/> or a fallback.
        /// </summary>
        /// <param name="builder">The render tree builder.</param>
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
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "title", string.Join("\n", Tracker.Tracks));
                builder.AddContent(2, "Busy..");
                builder.CloseComponent();
            }
        }

        /// <summary>
        /// Unsubscribes from tracker events and suppresses finalization.
        /// </summary>
        public void Dispose()
        {
            Tracker.OnBusyChanged -= OnBusyChange;
            GC.SuppressFinalize(this);
        }
    }
}
