using Microsoft.AspNetCore.Components;

namespace RTB.BlazorUI.Components;

public abstract class RTBComponent : ComponentBase
{
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> CapturedAttributes { get; set; } = new Dictionary<string, object>();
}
