using Microsoft.AspNetCore.Components;

namespace RTB.Blazor.Components;

/// <summary>
/// Base Component for all RTB components.<br/>
/// RTB Components are aware of the RTB theme and can use it.<br/>
/// RTB Components provide a way to capture unmatched attributes.<br/>
/// RTB Components provide a way to call StateHasChanged easily.<br/>
/// </summary>
public abstract class RTBComponent : ComponentBase
{
    [Parameter] public string? Id { get; set; }
    [Parameter] public string? Class { get; set; }

    protected static string CombineClass(params string?[] parts)
    {
        return string.Join(" ", parts.Where(p => !string.IsNullOrWhiteSpace(p))).Trim();
    }

    /// <summary>
    /// SetProperty is a helper method to call StateHasChanged.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="field"></param>
    /// <param name="value"></param>
    public void SetProperty<TValue>(ref TValue field, TValue value)
    {
        if (EqualityComparer<TValue>.Default.Equals(field, value)) return;

        field = value;
        InvokeAsync(StateHasChanged);
    }

    /// <summary>
    /// StatefulAction is a helper method to call an action and then call StateHasChanged.
    /// </summary>
    /// <param name="action"></param>
    public void StatefulAction(Action action)
    {
        action();
        InvokeAsync(StateHasChanged);
    }
}