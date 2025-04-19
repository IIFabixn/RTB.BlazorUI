using Microsoft.AspNetCore.Components;
using RTB.BlazorUI.Services.BusyTracker;

namespace RTB.BlazorUI.Components;

public abstract class RTBComponent : ComponentBase
{
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> CapturedAttributes { get; set; } = [];

    /// <summary>
    /// Executes an action and calls <c>StateHasChanged()</c> after, triggering a UI update.<br/>
    /// Use wisely, preferably only in simple lambda expressions.<br/>
    /// </summary>
    /// <param name="action">The action to perform before refreshing UI.</param>
    public void StateWillChange(Action action)
    {
        action();
        StateHasChanged();
    }
}
