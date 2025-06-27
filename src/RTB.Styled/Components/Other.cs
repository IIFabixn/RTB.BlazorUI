using System;
using Microsoft.AspNetCore.Components;

namespace RTB.Blazor.Styled.Components;

/// <summary>
/// Represents a custom CSS style component that allows you to set any CSS property and value.
/// </summary>
public class Other : RTBStyleBase
{
    [Parameter, EditorRequired] public string Property { get; set; } = string.Empty;
    [Parameter] public string? Value { get; set; }

    protected override StyleBuilder BuildStyle(StyleBuilder builder)
    {
        return builder.AppendIfNotNull(Property, Value);
    }
}
