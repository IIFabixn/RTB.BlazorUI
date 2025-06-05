using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using RTB.BlazorUI.Helper;
using RTB.BlazorUI.Services.Style;
using System.Runtime.CompilerServices;

namespace RTB.BlazorUI.Components;

/// <summary>
/// Base Component for all RTB components.<br/>
/// RTB Components are aware of the RTB theme and can use it.<br/>
/// RTB Components provide a way to capture unmatched attributes.<br/>
/// RTB Components provide a way to call StateHasChanged easily.<br/>
/// </summary>
public abstract class RTBComponent : ComponentBase
{
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? CapturedAttributes { get; set; }

    [Parameter] public string? Class { get; set; }

    /// <summary>
    /// SetProperty is a helper method to call StateHasChanged.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="field"></param>
    /// <param name="value"></param>
    public void SetProperty<TValue>(ref TValue field, TValue value, [CallerMemberName] string member = "")
    {
        if (EqualityComparer<TValue>.Default.Equals(field, value)) return;

        field = value;
        StateHasChanged();
    }

    /// <summary>
    /// StatefulAction is a helper method to call an action and then call StateHasChanged.
    /// </summary>
    /// <param name="action"></param>
    public void StatefulAction(Action action, [CallerMemberName] string callee = "")
    {
        action();
        StateHasChanged();
    }
}