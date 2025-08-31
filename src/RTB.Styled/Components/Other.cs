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

    public override IStyleBuilder BuildStyle(IStyleBuilder builder)
    {
        if (!Condition) return builder;

        return builder.AppendIfNotNull(Property, Value);
    }
}

public static class OtherExtensions
{
    /// <summary>
    /// Adds a custom CSS property and value to the style builder.
    /// </summary>
    public static IStyleBuilder Other(this IStyleBuilder builder, string property, string? value)
    {
        return builder.AppendIfNotNull(property, value);
    }
}
