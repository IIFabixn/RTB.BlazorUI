using Microsoft.AspNetCore.Components;

namespace RTB.Blazor.Components;

/// <summary>
/// Base component for RTB Blazor components.
/// </summary>
/// <remarks>
/// <para>
/// Provides common conveniences for derived components:
/// </para>
/// <list type="bullet">
///   <item>
///     <description>
///       Optional <see cref="Id"/> and <see cref="Class"/> parameters for markup.
///     </description>
///   </item>
///   <item>
///     <description>
///       <see cref="CombineClass(string?[])"/> to safely compose CSS class strings.
///     </description>
///   </item>
///   <item>
///     <description>
///       <see cref="SetProperty{TValue}(ref TValue, TValue)"/> to update a backing field and schedule a re-render only when the value changes.
///     </description>
///   </item>
///   <item>
///     <description>
///       <see cref="StatefulAction(System.Action)"/> to run arbitrary logic and then schedule a re-render.
///     </description>
///   </item>
/// </list>
/// <para>
/// Re-rendering is scheduled via <see cref="ComponentBase.InvokeAsync(System.Func{System.Threading.Tasks.Task})"/>
/// to ensure it occurs on the correct synchronization context for Blazor.
/// </para>
/// </remarks>
/// <example>
/// Example: efficiently update a bound value and re-render only when it changes.
/// <code>
/// private string? _text;
/// [Parameter] public string? Text
/// {
///     get => _text;
///     set => SetProperty(ref _text, value);
/// }
/// </code>
/// Example: build a class attribute from optional parts.
/// <code>
/// var classes = CombineClass("rtb-btn", isPrimary ? "rtb-btn--primary" : null, Class);
/// </code>
/// </example>
public abstract class RTBComponent : ComponentBase
{
    /// <summary>
    /// Optional id attribute value that derived components may apply to their root element.
    /// </summary>
    [Parameter] public string? Id { get; set; }

    /// <summary>
    /// Optional CSS class(es) that derived components may append to their root element.
    /// </summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>
    /// Combines CSS class name fragments into a single space-delimited string,
    /// ignoring null, empty, and whitespace-only entries.
    /// </summary>
    /// <param name="parts">The class name fragments to combine.</param>
    /// <returns>A space-delimited string containing only non-empty fragments.</returns>
    protected static string CombineClass(params string?[] parts)
    {
        return string.Join(" ", parts.Where(p => !string.IsNullOrWhiteSpace(p))).Trim();
    }

    /// <summary>
    /// Sets a backing field to a new value and, if the value changed, schedules a component re-render.
    /// </summary>
    /// <typeparam name="TValue">The type of the backing field.</typeparam>
    /// <param name="field">A reference to the backing field to update.</param>
    /// <param name="value">The new value to assign to the field.</param>
    /// <remarks>
    /// If the new value equals the current one (by <see cref="EqualityComparer{T}.Default"/>), no re-render is scheduled.
    /// </remarks>
    public void SetProperty<TValue>(ref TValue field, TValue value)
    {
        if (EqualityComparer<TValue>.Default.Equals(field, value)) return;

        field = value;
        InvokeAsync(StateHasChanged);
    }

    /// <summary>
    /// Executes the provided action and then schedules a component re-render.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <remarks>
    /// Exceptions thrown by <paramref name="action"/> will propagate to the caller.
    /// </remarks>
    public void StatefulAction(Action action)
    {
        action();
        InvokeAsync(StateHasChanged);
    }
}