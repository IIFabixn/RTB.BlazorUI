using BlazorStyled;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using RTB.BlazorUI.Extensions;
using RTB.BlazorUI.Helper;
using RTB.BlazorUI.Services.BusyTracker;
using RTB.BlazorUI.Services.Theme;
using RTB.BlazorUI.Services.Theme.Styles;
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
    public Dictionary<string, object>? CapturedAttributes { get; set; }

    private string? _className;
    public string? ComponentClass
    {
        get => _className;
        set => SetProperty(ref _className, value);
    }

    /// <summary>
    /// Helper Property to get the class attribute from the captured attributes.<br/>
    /// </summary>
    public string? Class => CapturedAttributes?.GetValueOrDefault<string>("class");

    /// <summary>
    /// Helper Property to get the style attribute from the captured attributes.<br/>
    /// </summary>
    public string? Style => CapturedAttributes?.GetValueOrDefault<string>("style");

    #region Base Style Properties

    // Helper properties for common CSS styles

    [Parameter] public RTBColor? Background { get; set; }
    [Parameter] public RTBBorder? Border { get; set; }
    [Parameter] public RTBColor? Color { get; set; }
    [Parameter] public RTBSpacing? Padding { get; set; }
    [Parameter] public RTBSpacing? Margin { get; set; }

    // Shorthand properties for common CSS styles
    [Parameter] public bool FullHeight { get; set; }
    [Parameter] public bool FullWidth { get; set; }

    protected StyleBuilder RTBStyle => StyleBuilder.Create()
        .AppendIfNotEmpty("background-color", Background?.Hex)
        .AppendStyle(Border?.ToStyle())
        .AppendIfNotEmpty("color", Color?.Hex)
        .AppendIfNotEmpty("padding", Padding)
        .AppendIfNotEmpty("margin", Margin)
        .AppendIf("width", "100%", FullWidth)
        .AppendIf("height", "100%", FullHeight)
        .AppendStyle(Style);

    #endregion

    /// <summary>
    /// SetProperty is a helper method to call StateHasChanged.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetProperty<TValue>(ref TValue key, TValue value, [CallerMemberName] string member = "")
    {
        key = value;
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