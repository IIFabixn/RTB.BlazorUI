using BlazorStyled;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using RTB.BlazorUI.Helper;
using RTB.BlazorUI.Services.BusyTracker;
using RTB.BlazorUI.Services.Theme;
using RTB.BlazorUI.Services.Theme.Themes;
using System.Diagnostics;
using System.Reflection;
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
    public Dictionary<string, object> CapturedAttributes { get; set; } = [];

    protected string? ComponentClass { get; set; }

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