using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Core;
using RTB.Blazor.Styled.Helper;
using System;
using System.Drawing;
using static RTB.Blazor.Styled.Components.Border;

namespace RTB.Blazor.Styled.Components;

/// <summary>
/// Contributes CSS <c>border</c> and <c>border-radius</c> declarations to the cascading <see cref="StyleBuilder"/>.
/// </summary>
/// <remarks>
/// - Use <see cref="Side"/>, <see cref="Style"/>, <see cref="Width"/>, and <see cref="Color"/> to render borders on one or more sides.<br/>
/// - Use <see cref="Corner"/> and <see cref="Radius"/> to control per-corner or all-corners radius.<br/>
/// - Defaults when unspecified:
///   <list type="bullet">
///     <item><description>When a border is requested (<see cref="Side"/> not null), <see cref="Width"/> defaults to <c>1px</c>.</description></item>
///     <item><description>When a border is requested, <see cref="Color"/> defaults to black (<c>#000</c>).</description></item>
///     <item><description>When a radius is requested (<see cref="Corner"/> not null), <see cref="Radius"/> defaults to <c>1px</c>.</description></item>
///   </list>
/// - Special cases:
///   <list type="bullet">
///     <item><description><see cref="BorderSide.None"/> results in <c>border: unset</c>.</description></item>
///     <item><description><see cref="BorderSide.All"/> uses the shorthand <c>border: [width] [style] [color]</c>.</description></item>
///   </list>
/// </remarks>
/// <example>
/// Typical usage inside a style scope (component providing a cascading StyleBuilder):
/// <code>
/// &lt;Border Side="Border.BorderSide.All"
///         Style="Border.BorderStyle.Solid"
///         Width="SizeUnit.Px(2)"
///         Color="RTBColors.RoyalBlue" /&gt;
///
/// &lt;Border Corner="Border.BorderCorner.Top"
///         Radius="SizeUnit.Rem(0.5)" /&gt;
/// </code>
/// </example>
public class Border : RTBStyleBase
{
    /// <summary>
    /// Which side(s) to apply a border to. When set, a border is emitted using
    /// <see cref="Width"/> (defaults to 1px when null), <see cref="Style"/>, and <see cref="Color"/> (defaults to black when null).
    /// </summary>
    [Parameter] public BorderSide? Side { get; set; }

    /// <summary>
    /// The CSS border style keyword to use (e.g., <c>solid</c>, <c>dashed</c>). Default is <see cref="BorderStyle.Solid"/>.
    /// </summary>
    [Parameter] public BorderStyle Style { get; set; } = BorderStyle.Solid;

    /// <summary>
    /// The border width to use. When <see cref="Side"/> is provided and this is null, a default of <c>1px</c> is used.
    /// </summary>
    [Parameter] public SizeUnit? Width { get; set; }

    /// <summary>
    /// The border color to use. When <see cref="Side"/> is provided and this is null, black is used.
    /// </summary>
    [Parameter] public RTBColor? Color { get; set; }

    /// <summary>
    /// The border radius to apply. When <see cref="Corner"/> is provided and this is null, a default of <c>1px</c> is used.
    /// </summary>
    [Parameter] public SizeUnit? Radius { get; set; }

    /// <summary>
    /// Which corner(s) should receive the border radius. Supports individual corners and grouped flags (e.g., <see cref="BorderCorner.Top"/> or <see cref="BorderCorner.All"/>).
    /// </summary>
    [Parameter] public BorderCorner? Corner { get; set; }

    /// <summary>
    /// Contributes declarations to the builder:
    /// - Emits <c>border-radius</c> declarations when <see cref="Corner"/> is not null (with <see cref="Radius"/> or default 1px).<br/>
    /// - Emits <c>border</c> declarations when <see cref="Side"/> is not null (with <see cref="Width"/> default 1px and <see cref="Color"/> default black if omitted).
    /// </summary>
    /// <param name="builder">The style builder to receive the declarations.</param>
    protected override void BuildStyle(StyleBuilder builder)
    {
        if (Corner is not null) builder.BorderRadius(Radius, Corner);
        if (Side is not null) builder.Border(Width, Style, Color, Side);
    }

    /// <summary>
    /// Flags enum describing which side(s) of a box should receive border declarations.
    /// </summary>
    [Flags]
    public enum BorderSide
    {
        /// <summary>No sides.</summary>
        None = 0,
        /// <summary>Top side.</summary>
        Top = 1,
        /// <summary>Right side.</summary>
        Right = 2,
        /// <summary>Bottom side.</summary>
        Bottom = 4,
        /// <summary>Left side.</summary>
        Left = 8,

        /// <summary>All four sides.</summary>
        All = Top | Right | Bottom | Left,

        /// <summary>Left and right sides.</summary>
        Horizontal = Left | Right,
        /// <summary>Top and bottom sides.</summary>
        Vertical = Top | Bottom,
    }

    /// <summary>
    /// Flags enum describing which corner(s) should receive a radius.
    /// </summary>
    [Flags]
    public enum BorderCorner
    {
        /// <summary>No corners.</summary>
        None = 0,
        /// <summary>Top-left corner.</summary>
        TopLeft = 1,
        /// <summary>Top-right corner.</summary>
        TopRight = 2,
        /// <summary>Bottom-left corner.</summary>
        BottomLeft = 4,
        /// <summary>Bottom-right corner.</summary>
        BottomRight = 8,

        /// <summary>Both top corners.</summary>
        Top = TopLeft | TopRight,
        /// <summary>Both right corners.</summary>
        Right = TopRight | BottomRight,
        /// <summary>Both bottom corners.</summary>
        Bottom = BottomRight | BottomLeft,
        /// <summary>Both left corners.</summary>
        Left = BottomLeft | TopRight,

        /// <summary>All four corners.</summary>
        All = TopLeft | TopRight | BottomLeft | BottomRight
    }

    /// <summary>
    /// CSS <c>border-style</c> values supported by this component.
    /// </summary>
    public enum BorderStyle
    {
        /// <summary>No border.</summary>
        None,
        /// <summary>Solid border.</summary>
        Solid,
        /// <summary>Dotted border.</summary>
        Dotted,
        /// <summary>Dashed border.</summary>
        Dashed,
        /// <summary>Double-line border.</summary>
        Double,
        /// <summary>Groove border.</summary>
        Groove,
        /// <summary>Ridge border.</summary>
        Ridge,
        /// <summary>Inset border.</summary>
        Inset,
        /// <summary>Outset border.</summary>
        Outset
    }
}

/// <summary>
/// Extension methods on <see cref="StyleBuilder"/> for composing border and radius declarations.
/// </summary>
public static class BorderStyleExtensions
{
    /// <summary>
    /// Sets <c>border</c> shorthand for all sides: <c>[width] [style] [color]</c>.
    /// </summary>
    /// <param name="builder">The target style builder.</param>
    /// <param name="width">The border width (e.g., 1px).</param>
    /// <param name="color">The border color.</param>
    /// <param name="style">The border style keyword.</param>
    /// <returns>The builder for chaining.</returns>
    public static StyleBuilder BorderAll(this StyleBuilder builder, SizeUnit? width, RTBColor color, BorderStyle style)
    {
        string value = $"{width} {style.ToCss()} {color}";
        return builder.Set("border", value);
    }

    /// <summary>
    /// Unsets the border: <c>border: unset</c>.
    /// </summary>
    /// <param name="builder">The target style builder.</param>
    /// <returns>The builder for chaining.</returns>
    public static StyleBuilder BorderNone(this StyleBuilder builder)
    {
        return builder.Set("border", "unset");
    }

    /// <summary>
    /// Sets a uniform <c>border-radius</c> value for all corners.
    /// </summary>
    /// <param name="builder">The target style builder.</param>
    /// <param name="radius">The radius to apply. If null, nothing is emitted.</param>
    /// <returns>The builder for chaining.</returns>
    public static StyleBuilder BorderRadiusAll(this StyleBuilder builder, SizeUnit? radius)
    {
        if (radius is null) return builder;
        return builder.Set("border-radius", radius);
    }

    /// <summary>
    /// Unsets any corner radius: <c>border-radius: unset</c>.
    /// </summary>
    /// <param name="builder">The target style builder.</param>
    /// <returns>The builder for chaining.</returns>
    public static StyleBuilder BorderRadiusNone(this StyleBuilder builder)
    {
        return builder.Set("border-radius", "unset");
    }

    /// <summary>
    /// Emits border declarations for the specified side(s).
    /// </summary>
    /// <param name="builder">The target style builder.</param>
    /// <param name="width">Width to use; when null, defaults to <c>1px</c>.</param>
    /// <param name="style">Border style keyword.</param>
    /// <param name="color">Color to use; when null, defaults to black.</param>
    /// <param name="side">
    /// Side(s) to apply:
    /// - <see cref="Border.BorderSide.None"/>: emits <c>border: unset</c><br/>
    /// - <see cref="Border.BorderSide.All"/>: emits shorthand <c>border</c><br/>
    /// - Mixed flags: emits per-side properties (<c>border-top</c>, <c>border-right</c>, etc.)
    /// </param>
    /// <returns>The builder for chaining.</returns>
    public static StyleBuilder Border(this StyleBuilder builder, SizeUnit? width, BorderStyle style, RTBColor? color, BorderSide? side)
    {
        if (side is null) return builder;

        width ??= SizeUnit.Px(1);
        color ??= RTBColors.Black;

        var s = (BorderSide)side;
        var c = (RTBColor)color;

        if (side == BorderSide.None) return builder.BorderNone();
        if (side == BorderSide.All) return builder.BorderAll(width, c, style);

        string value = $"{width} {style.ToCss()} {color}";
        if (s.HasFlag(BorderSide.Top))
            builder.Set("border-top", value);
        if (s.HasFlag(BorderSide.Right))
            builder.Set("border-right", value);
        if (s.HasFlag(BorderSide.Bottom))
            builder.Set("border-bottom", value);
        if (s.HasFlag(BorderSide.Left))
            builder.Set("border-left", value);

        return builder;
    }

    /// <summary>
    /// Emits <c>border-radius</c> declarations for the specified corner(s).
    /// </summary>
    /// <param name="builder">The target style builder.</param>
    /// <param name="radius">Radius to use; when null, defaults to <c>1px</c> if any corner is specified.</param>
    /// <param name="corner">
    /// Corner(s) to apply:
    /// - <see cref="Border.BorderCorner.None"/>: emits <c>border-radius: unset</c><br/>
    /// - <see cref="Border.BorderCorner.All"/>: emits uniform <c>border-radius</c><br/>
    /// - Mixed flags: emits the corresponding per-corner properties.
    /// </param>
    /// <returns>The builder for chaining.</returns>
    public static StyleBuilder BorderRadius(this StyleBuilder builder, SizeUnit? radius, BorderCorner? corner)
    {
        if (corner is null) return builder;

        if (corner == BorderCorner.None) return builder.BorderRadiusNone();

        radius ??= SizeUnit.Px(1);
        if (corner == BorderCorner.All) return builder.BorderRadiusAll(radius);

        var c = (BorderCorner)corner;
        if (c.HasFlag(BorderCorner.TopLeft))
            builder.Set("border-top-left-radius", radius);
        if (c.HasFlag(BorderCorner.TopRight))
            builder.Set("border-top-right-radius", radius);
        if (c.HasFlag(BorderCorner.BottomLeft))
            builder.Set("border-bottom-left-radius", radius);
        if (c.HasFlag(BorderCorner.BottomRight))
            builder.Set("border-bottom-right-radius", radius);

        return builder;
    }
}
