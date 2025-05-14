using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using RTB.BlazorUI.Services.BusyTracker;
using System.Runtime.CompilerServices;

namespace RTB.BlazorUI.Components;

public abstract class RTBComponent : ComponentBase
{
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> CapturedAttributes { get; set; } = [];

    /// <summary>
    /// SetProperty is a helper method to call StateHasChanged.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetProperty<TValue>(ref TValue key, TValue value)
    {
        key = value;
        StateHasChanged();
    }

    /// <summary>
    /// StatefulAction is a helper method to call an action and then call StateHasChanged.
    /// </summary>
    /// <param name="action"></param>
    public void StatefulAction(Action action)
    {
        action();
        StateHasChanged();
    }
}

public abstract class BusyComponent : RTBComponent
{
    [Inject]
    private IBusyTracker BusyTracker { get; set; } = default!;

    /// <summary>
    /// TrackBusy is a helper method to track the busy state of the component.
    /// Uppon disposing the IDisposable, the component will call StateHasChanged.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public IDisposable TrackBusy([CallerMemberName] string key = "")
    {
        return BusyTracker.Track(key,
            onDispose: () =>
            {
                StateHasChanged();
            });
    }

    /// <summary>
    /// IsBusy is a helper method to check if the component is busy.
    /// </summary>
    /// <param name="key">
    /// The key to track the busy state of the component.
    /// </param>
    /// <returns></returns>
    public bool IsBusy([CallerMemberName] string? key = "")
    {
        return BusyTracker.IsBusy(key);
    }
}