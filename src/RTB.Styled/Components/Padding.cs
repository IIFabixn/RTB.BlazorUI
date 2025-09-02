using System;
using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Core;
using RTB.Blazor.Styled.Helper;

namespace RTB.Blazor.Styled.Components;

/// <summary>
/// Contributes CSS padding declarations to a cascading <see cref="StyleBuilder"/>.
/// Supports:
/// - A single value for all sides (<see cref="All"/>).
/// - Axis shorthands (<see cref="Vertical"/> for top/bottom and <see cref="Horizontal"/> for left/right).
/// - Individual sides (<see cref="Top"/>, <see cref="Right"/>, <see cref="Bottom"/>, <see cref="Left"/>).
/// <para>
/// Precedence (from lowest to highest):
/// 1) <see cref="All"/> (padding)
/// 2) Axis shorthand via <see cref="Vertical"/>/<see cref="Horizontal"/> (padding: vertical horizontal)
/// 3) Individual sides (padding-top/right/bottom/left)
/// Later declarations override earlier ones when present.
/// </para>
/// <para>
/// Notes:
/// - When only one axis is provided, the missing axis defaults to 0 (CSS zero).
/// - Two-value padding shorthand uses the CSS form "padding: &lt;vertical&gt; &lt;horizontal&gt;",
///   i.e., "top/bottom left/right".
/// </para>
/// </summary>
public class Padding : RTBStyleBase
{
    /// <summary>
    /// Shorthand padding for all sides (emits "padding: {value}").
    /// Overridden by axis shorthand and any side-specific values when present.
    /// </summary>
    [Parameter] public Spacing? All { get; set; }

    /// <summary>
    /// Sets "padding-top: {value}". Overrides <see cref="All"/> and <see cref="Vertical"/> for the top side.
    /// </summary>
    [Parameter] public Spacing? Top { get; set; }

    /// <summary>
    /// Sets "padding-right: {value}". Overrides <see cref="All"/> and <see cref="Horizontal"/> for the right side.
    /// </summary>
    [Parameter] public Spacing? Right { get; set; }

    /// <summary>
    /// Sets "padding-bottom: {value}". Overrides <see cref="All"/> and <see cref="Vertical"/> for the bottom side.
    /// </summary>
    [Parameter] public Spacing? Bottom { get; set; }

    /// <summary>
    /// Sets "padding-left: {value}". Overrides <see cref="All"/> and <see cref="Horizontal"/> for the left side.
    /// </summary>
    [Parameter] public Spacing? Left { get; set; }

    /// <summary>
    /// Axis shorthand for left and right.
    /// When used (alone or with <see cref="Vertical"/>), emits a two-value padding shorthand:
    /// "padding: &lt;vertical&gt; &lt;horizontal&gt;" where &lt;horizontal&gt; sets left/right.
    /// Missing axis defaults to 0.
    /// </summary>
    [Parameter] public Spacing? Horizontal { get; set; }

    /// <summary>
    /// Axis shorthand for top and bottom.
    /// When used (alone or with <see cref="Horizontal"/>), emits a two-value padding shorthand:
    /// "padding: &lt;vertical&gt; &lt;horizontal&gt;" where &lt;vertical&gt; sets top/bottom.
    /// Missing axis defaults to 0.
    /// </summary>
    [Parameter] public Spacing? Vertical { get; set; }

    /// <summary>
    /// <inheritdoc cref="RTBStyleBase.BuildStyle(StyleBuilder)"/>
    /// <remarks>
    /// Emission order establishes precedence:
    /// 1) "padding" via <see cref="All"/>
    /// 2) Two-value "padding: vertical horizontal" via <see cref="Vertical"/>/<see cref="Horizontal"/>
    /// 3) Side-specific properties via <see cref="Top"/>, <see cref="Right"/>, <see cref="Bottom"/>, <see cref="Left"/>
    /// </remarks>
    /// </summary>
    /// <param name="builder">The style builder receiving padding declarations.</param>
    protected override void BuildStyle(StyleBuilder builder)
    {
        // Shorthand for all sides
        builder.PaddingAll(All);

        // Two-value shorthand: "padding: <vertical> <horizontal>" => (top/bottom) (left/right)
        builder.SetIf("padding", $"{Vertical ?? 0} {Horizontal ?? 0}", Horizontal.HasValue || Vertical.HasValue);

        // Per-side overrides
        builder.PaddingTop(Top);
        builder.PaddingRight(Right);
        builder.PaddingBottom(Bottom);
        builder.PaddingLeft(Left);
    }
}

/// <summary>
/// Extension methods on <see cref="StyleBuilder"/> for emitting padding declarations.
/// </summary>
public static class PaddingExtensions
{
    /// <summary>
    /// Emits "padding: {value}" when <paramref name="value"/> is not null.
    /// </summary>
    /// <param name="builder">The target style builder.</param>
    /// <param name="value">Spacing for all sides.</param>
    /// <returns>The same builder for chaining.</returns>
    public static StyleBuilder PaddingAll(this StyleBuilder builder, Spacing? value)
    {
        return builder.SetIfNotNull("padding", value);
    }

    /// <summary>
    /// Emits "padding-top: {value}" when <paramref name="value"/> is not null.
    /// </summary>
    /// <param name="builder">The target style builder.</param>
    /// <param name="value">Top spacing.</param>
    /// <returns>The same builder for chaining.</returns>
    public static StyleBuilder PaddingTop(this StyleBuilder builder, Spacing? value)
    {
        return builder.SetIfNotNull("padding-top", value);
    }

    /// <summary>
    /// Emits "padding-right: {value}" when <paramref name="value"/> is not null.
    /// </summary>
    /// <param name="builder">The target style builder.</param>
    /// <param name="value">Right spacing.</param>
    /// <returns>The same builder for chaining.</returns>
    public static StyleBuilder PaddingRight(this StyleBuilder builder, Spacing? value)
    {
        return builder.SetIfNotNull("padding-right", value);
    }

    /// <summary>
    /// Emits "padding-bottom: {value}" when <paramref name="value"/> is not null.
    /// </summary>
    /// <param name="builder">The target style builder.</param>
    /// <param name="value">Bottom spacing.</param>
    /// <returns>The same builder for chaining.</returns>
    public static StyleBuilder PaddingBottom(this StyleBuilder builder, Spacing? value)
    {
        return builder.SetIfNotNull("padding-bottom", value);
    }

    /// <summary>
    /// Emits "padding-left: {value}" when <paramref name="value"/> is not null.
    /// </summary>
    /// <param name="builder">The target style builder.</param>
    /// <param name="value">Left spacing.</param>
    /// <returns>The same builder for chaining.</returns>
    public static StyleBuilder PaddingLeft(this StyleBuilder builder, Spacing? value)
    {
        return builder.SetIfNotNull("padding-left", value);
    }
}
