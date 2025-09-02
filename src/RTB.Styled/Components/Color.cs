using System;
using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Core;
using RTB.Blazor.Styled.Helper;

namespace RTB.Blazor.Styled.Components;

/// <summary>
/// Contributes a CSS <c>color</c> declaration to the current <see cref="StyleBuilder"/> scope.
/// </summary>
/// <remarks>
/// - When <see cref="RTBStyleBase.Condition"/> is true, this component registers itself with the cascading
///   <see cref="StyleBuilder"/> and emits a <c>color</c> declaration during composition.
/// - When <see cref="Value"/> is <c>null</c>, no declaration is written (the property is omitted).
/// - The value is serialized using <see cref="RTBColor"/>'s formatting (e.g., hex RGBA).
/// </remarks>
/// <example>
/// <code>
/// <Color Value="RTBColor.FromRgb(255, 0, 0)" />
/// </code>
/// </example>
public class Color : RTBStyleBase
{
    /// <summary>
    /// The color to apply to the CSS <c>color</c> property.
    /// </summary>
    /// <remarks>
    /// - <c>null</c> omits the declaration.<br/>
    /// - Use <see cref="RTBColor.Parse(string)"/> to create a value from a CSS string if needed.
    /// </remarks>
    [Parameter] public RTBColor? Value { get; set; }

    /// <summary>
    /// Emits the CSS <c>color</c> declaration when <see cref="Value"/> is not <c>null</c>.
    /// </summary>
    /// <param name="builder">The style builder receiving the declaration.</param>
    /// <remarks>
    /// See <see cref="RTBStyleBase.BuildStyle(StyleBuilder)"/> for lifecycle details.
    /// </remarks>
    protected override void BuildStyle(StyleBuilder builder)
    {
        builder.Color(Value);
    }
}

/// <summary>
/// <see cref="StyleBuilder"/> extensions for the CSS <c>color</c> property.
/// </summary>
public static class ColorExtensions
{
    /// <summary>
    /// Sets the CSS <c>color</c> property when <paramref name="color"/> is not <c>null</c>.
    /// </summary>
    /// <param name="builder">The target <see cref="StyleBuilder"/>.</param>
    /// <param name="color">The color value to serialize and assign; <c>null</c> omits the declaration.</param>
    /// <returns>The same <see cref="StyleBuilder"/> for chaining.</returns>
    /// <remarks>
    /// If the property is set multiple times, the last value wins (last-write-wins).
    /// </remarks>
    public static StyleBuilder Color(this StyleBuilder builder, RTBColor? color)
    {
        return builder.SetIfNotNull("color", color);
    }
}
