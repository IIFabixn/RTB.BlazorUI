using System;
using Microsoft.AspNetCore.Components;

namespace RTB.BlazorUI.Styles.Components;

/// <summary>
/// Represents a custom CSS style component that allows you to set any CSS property and value.
/// </summary>
public class Other : RTBStyleBase
{
    [Parameter] public string Property { get; set; } = string.Empty;
    [Parameter] public string? Value { get; set; }
    [Parameter] public string? Raw { get; set; }

    protected override void OnParametersSet()
    {
        if (!Condition) return;

        StyleBuilder.Append(Property, Value);

        base.OnParametersSet();
    }
}
