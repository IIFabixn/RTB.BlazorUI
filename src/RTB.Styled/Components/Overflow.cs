using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Core;
using RTB.Blazor.Styled.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Components;

/// <summary>
/// A style contributor that sets CSS overflow properties.
/// Behavior:
///  - When all parameters (<see cref="X"/>, <see cref="Y"/>, <see cref="Value"/>) are null, this emits 'overflow: auto'.
///  - Otherwise, it emits any provided axis-specific values (overflow-x, overflow-y) and/or the shorthand (overflow).
///    Note: If both axis-specific values and the shorthand are supplied, the emitted order is:
///    overflow-x, overflow-y, overflow (last). In CSS, the shorthand can reset axis-specific values.
///    Prefer setting either axis-specific values or the shorthand to avoid unintended overrides.
/// </summary>
public class Overflow : RTBStyleBase
{
    /// <summary>
    /// Represents allowed values for CSS overflow properties.
    /// </summary>
    public enum OverflowMode
    {
        /// <summary>Maps to 'visible'. Content is not clipped, may render outside the element's box.</summary>
        Visible,
        /// <summary>Maps to 'hidden'. Content is clipped, no scrollbars.</summary>
        Hidden,
        /// <summary>Maps to 'scroll'. Content is clipped and scrollbars are always shown.</summary>
        Scroll,
        /// <summary>Maps to 'auto'. Content is clipped and scrollbars appear when needed.</summary>
        Auto
    }

    /// <summary>
    /// Sets 'overflow-x' for the horizontal axis.
    /// </summary>
    /// <remarks>When null, no 'overflow-x' declaration is emitted.</remarks>
    [Parameter] public OverflowMode? X { get; set; }

    /// <summary>
    /// Sets 'overflow-y' for the vertical axis.
    /// </summary>
    /// <remarks>When null, no 'overflow-y' declaration is emitted.</remarks>
    [Parameter] public OverflowMode? Y { get; set; }

    /// <summary>
    /// Sets the 'overflow' shorthand (both axes).
    /// </summary>
    /// <remarks>
    /// When null, no 'overflow' shorthand is emitted unless both <see cref="X"/> and <see cref="Y"/> are also null,
    /// in which case the component defaults to 'overflow: auto'.
    /// </remarks>
    [Parameter] public OverflowMode? Value { get; set; }

    /// <summary>
    /// Contributes CSS declarations for overflow properties.
    /// </summary>
    /// <param name="builder">The style builder that receives declarations.</param>
    /// <remarks>
    /// - If all properties are null, emits 'overflow: auto'.
    /// - Otherwise emits 'overflow-x' and/or 'overflow-y' for provided axes, and 'overflow' for <see cref="Value"/>.
    ///   The shorthand is emitted last and may override axis-specific values per CSS rules.
    /// </remarks>
    protected override void BuildStyle(StyleBuilder builder)
    {
        if (X == null && Y == null && Value == null)
        {
            builder.Set("overflow", OverflowMode.Auto.ToCss());
            return;
        }

        builder.OverflowX(X).OverflowY(Y).Overflow(Value);
    }
}

/// <summary>
/// Extension methods on <see cref="StyleBuilder"/> for setting overflow-related CSS.
/// </summary>
public static class OverflowExtensions
{
    /// <summary>
    /// Sets the 'overflow' CSS shorthand.
    /// </summary>
    /// <param name="builder">The style builder.</param>
    /// <param name="value">The overflow mode; when null, no declaration is emitted.</param>
    /// <returns>The same <see cref="StyleBuilder"/> for chaining.</returns>
    public static StyleBuilder Overflow(this StyleBuilder builder,
        Overflow.OverflowMode? value = null)
    {
        return builder.Other("overflow", value?.ToCss());
    }

    /// <summary>
    /// Sets the 'overflow-x' CSS property (horizontal axis).
    /// </summary>
    /// <param name="builder">The style builder.</param>
    /// <param name="value">The overflow mode; when null, no declaration is emitted.</param>
    /// <returns>The same <see cref="StyleBuilder"/> for chaining.</returns>
    public static StyleBuilder OverflowX(this StyleBuilder builder,
        Overflow.OverflowMode? value = null)
    {
        return builder.Other("overflow-x", value?.ToCss());
    }

    /// <summary>
    /// Sets the 'overflow-y' CSS property (vertical axis).
    /// </summary>
    /// <param name="builder">The style builder.</param>
    /// <param name="value">The overflow mode; when null, no declaration is emitted.</param>
    /// <returns>The same <see cref="StyleBuilder"/> for chaining.</returns>
    public static StyleBuilder OverflowY(this StyleBuilder builder,
        Overflow.OverflowMode? value = null)
    {
        return builder.Other("overflow-y", value?.ToCss());
    }
}
