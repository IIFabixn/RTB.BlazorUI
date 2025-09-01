using System;
using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Core;

namespace RTB.Blazor.Styled.Components;

/// <summary>
/// Represents a custom CSS style component that allows you to set any CSS property and value.
/// </summary>
public class Other : RTBStyleBase
{
    [Parameter, EditorRequired] public string Property { get; set; } = string.Empty;
    [Parameter] public string? Value { get; set; }

    protected override void BuildStyle(StyleBuilder builder)
    {
        builder.Other(Property, Value);
    }
}

public static class OtherExtensions
{
    /// <summary>
    /// Adds a custom CSS property and value to the style builder.
    /// </summary>
    public static StyleBuilder Other(this StyleBuilder builder, string property, string? value)
    {
        return builder.SetIfNotNull(property, value);
    }
}
