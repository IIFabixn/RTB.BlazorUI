using Microsoft.AspNetCore.Components;
using RTB.BlazorUI.Services.BusyTracker;
using static RTB.BlazorUI.Services.BusyTracker.BusyTracker;

namespace RTB.BlazorUI.Components;

public abstract class RTBComponent : ComponentBase
{
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> CapturedAttributes { get; set; } = [];

    /// <summary>
    /// Easyily notify the component that it needs to re-render.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetProperty<TValue>(ref TValue key, TValue value)
    {
        key = value;
        StateHasChanged();
    }
}
