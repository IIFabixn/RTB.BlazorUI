using Microsoft.AspNetCore.Components;
using RTB.BlazorUI.Services.BusyTracker;
using static RTB.BlazorUI.Services.BusyTracker.BusyTracker;

namespace RTB.BlazorUI.Components;

public abstract class RTBComponent : ComponentBase
{
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> CapturedAttributes { get; set; } = [];
}
