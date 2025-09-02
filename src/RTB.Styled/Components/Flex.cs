using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Core;
using RTB.Blazor.Styled.Helper;

namespace RTB.Blazor.Styled.Components;

/// <summary>
/// Contributes CSS declarations for a flex container (display: flex) to the current <see cref="StyleBuilder"/> scope.
/// </summary>
/// <remarks>
/// - This component writes only the properties you specify; when a parameter is null, the corresponding declaration is omitted
///   and the browser default applies.
/// - Typical browser defaults (when not explicitly set):<br/>
///   flex-direction: row; flex-wrap: nowrap; justify-content: flex-start; align-items: stretch; align-content: normal;<br/>
///   flex-grow: 0; flex-shrink: 1.
/// - Place this component within a style scope that supplies the cascading <see cref="StyleBuilder"/>.
/// </remarks>
/// <example>
/// <code>
/// // Equivalent via the extensions API:
/// var css = StyleBuilder.Start
///     .Flex(direction: Flex.AxisDirection.Row,
///           wrap: Flex.WrapMode.NoWrap,
///           justifyContent: Flex.Justify.SpaceBetween,
///           alignItems: Flex.Align.Center,
///           gap: Spacing.Rem(1),
///           grow: 1)
///     .BuildScoped(".container");
/// </code>
/// </example>
public class Flex : RTBStyleBase
{
    /// <summary>
    /// Flexbox main-axis direction. When omitted, the browser default is <c>row</c>.
    /// </summary>
    public enum AxisDirection
    {
        /// <summary>Maps to <c>flex-direction: row;</c></summary>
        Row,
        /// <summary>Maps to <c>flex-direction: row-reverse;</c></summary>
        RowReverse,
        /// <summary>Maps to <c>flex-direction: column;</c></summary>
        Column,
        /// <summary>Maps to <c>flex-direction: column-reverse;</c></summary>
        ColumnReverse
    }

    /// <summary>
    /// Flexbox wrapping behavior. When omitted, the browser default is <c>nowrap</c>.
    /// </summary>
    public enum WrapMode
    {
        /// <summary>Maps to <c>flex-wrap: nowrap;</c></summary>
        NoWrap,
        /// <summary>Maps to <c>flex-wrap: wrap;</c></summary>
        Wrap,
        /// <summary>Maps to <c>flex-wrap: wrap-reverse;</c></summary>
        WrapReverse
    }

    /// <summary>
    /// Distribution along the main axis.
    /// </summary>
    public enum Justify
    {
        /// <summary>Maps to <c>justify-content: flex-start;</c></summary>
        Start,
        /// <summary>Maps to <c>justify-content: flex-end;</c></summary>
        End,
        /// <summary>Maps to <c>justify-content: center;</c></summary>
        Center,
        /// <summary>Maps to <c>justify-content: space-between;</c></summary>
        SpaceBetween,
        /// <summary>Maps to <c>justify-content: space-around;</c></summary>
        SpaceAround,
        /// <summary>Maps to <c>justify-content: space-evenly;</c></summary>
        SpaceEvenly
    }

    /// <summary>
    /// Alignment in the cross axis (or multi-line distribution for <see cref="AlignContent"/>).
    /// </summary>
    /// <remarks>
    /// Notes:
    /// - <c>Baseline</c> is valid for <c>align-items</c>/<c>align-self</c>, but not for <c>align-content</c>. Avoid using
    ///   <see cref="Baseline"/> with <see cref="AlignContent"/>; many browsers will ignore it.
    /// </remarks>
    public enum Align
    {
        /// <summary>Maps to <c>align-*: flex-start;</c></summary>
        Start,
        /// <summary>Maps to <c>align-*: flex-end;</c></summary>
        End,
        /// <summary>Maps to <c>align-*: center;</c></summary>
        Center,
        /// <summary>Maps to <c>align-*: stretch;</c></summary>
        Stretch,
        /// <summary>Maps to <c>align-*: baseline;</c></summary>
        Baseline
    }

    /// <summary>
    /// Sets <c>flex-direction</c>. When null, no declaration is written (default: <c>row</c>).
    /// </summary>
    [Parameter] public AxisDirection? Direction { get; set; }

    /// <summary>
    /// Sets <c>flex-wrap</c>. When null, no declaration is written (default: <c>nowrap</c>).
    /// </summary>
    [Parameter] public WrapMode? Wrap { get; set; }

    /// <summary>
    /// Sets <c>justify-content</c> (distribution along the main axis). When null, browser default applies.
    /// </summary>
    [Parameter] public Justify? JustifyContent { get; set; }

    /// <summary>
    /// Sets <c>align-items</c> (alignment of items on the cross axis). When null, browser default applies.
    /// </summary>
    [Parameter] public Align? AlignItems { get; set; }

    /// <summary>
    /// Sets <c>align-content</c> (distribution of lines in a multi-line flex container). When null, browser default applies.
    /// </summary>
    /// <remarks>
    /// Do not use <see cref="Align.Baseline"/> with <c>align-content</c>; it is not a valid value for that property.
    /// </remarks>
    [Parameter] public Align? AlignContent { get; set; }

    /// <summary>
    /// Sets the <c>gap</c> between items (length or percentage, e.g., <c>1rem</c>, <c>8px</c>, <c>4%</c>).
    /// </summary>
    /// <remarks>
    /// Note: The CSS keyword <c>auto</c> is not valid for <c>gap</c>. Avoid using <see cref="Spacing.Auto"/> here.
    /// </remarks>
    [Parameter] public Spacing? Gap { get; set; }

    /// <summary>
    /// Sets <c>flex-shrink</c> (non-negative integer). When null, no declaration is written (default: <c>1</c>).
    /// </summary>
    [Parameter] public int? Shrink { get; set; }

    /// <summary>
    /// Sets <c>flex-grow</c> (non-negative integer). When null, no declaration is written (default: <c>0</c>).
    /// </summary>
    [Parameter] public int? Grow { get; set; }

    /// <summary>
    /// Contributes the flex container declarations to the provided <paramref name="builder"/>.
    /// </summary>
    /// <param name="builder">The style builder receiving the flex container declarations.</param>
    protected override void BuildStyle(StyleBuilder builder)
    {
        builder
            .Set("display", "flex")
            .SetIfNotNull("flex-direction", Direction?.ToCss())
            .SetIfNotNull("flex-wrap", Wrap?.ToCss())
            .SetIfNotNull("justify-content", JustifyContent?.ToCss())
            .SetIfNotNull("align-items", AlignItems?.ToCss())
            .SetIfNotNull("align-content", AlignContent?.ToCss())
            .SetIfNotNull("gap", Gap)
            .SetIfNotNull("flex-shrink", Shrink?.ToString())
            .SetIfNotNull("flex-grow", Grow?.ToString());
    }
}

/// <summary>
/// Extension helpers for composing flex container styles fluently.
/// </summary>
/// <remarks>
/// Applies the same CSS as the <see cref="Flex"/> component but directly on a <see cref="StyleBuilder"/>.
/// Only the provided arguments are emitted; omitted arguments leave browser defaults in effect.
/// </remarks>
public static class FlexExtensions
{
    /// <summary>
    /// Adds flex container declarations to the builder.
    /// </summary>
    /// <param name="builder">The <see cref="StyleBuilder"/> to mutate.</param>
    /// <param name="direction">Maps to <c>flex-direction</c>. Null omits the declaration.</param>
    /// <param name="wrap">Maps to <c>flex-wrap</c>. Null omits the declaration.</param>
    /// <param name="justifyContent">Maps to <c>justify-content</c>. Null omits the declaration.</param>
    /// <param name="alignItems">Maps to <c>align-items</c>. Null omits the declaration.</param>
    /// <param name="alignContent">
    /// Maps to <c>align-content</c>. Null omits the declaration.
    /// Avoid <see cref="Flex.Align.Baseline"/>; it is not valid for this property.
    /// </param>
    /// <param name="gap">
    /// Maps to <c>gap</c>. Use lengths or percentages (e.g., <c>Spacing.Px(8)</c>, <c>Spacing.Rem(1)</c>).
    /// Do not use <see cref="Spacing.Auto"/>.
    /// </param>
    /// <param name="shrink">Maps to <c>flex-shrink</c> (non-negative integer). Null omits the declaration.</param>
    /// <param name="grow">Maps to <c>flex-grow</c> (non-negative integer). Null omits the declaration.</param>
    /// <returns>The same <see cref="StyleBuilder"/> for chaining.</returns>
    /// <example>
    /// <code>
    /// var css = StyleBuilder.Start
    ///     .Flex(direction: Flex.AxisDirection.Row, gap: Spacing.Em(0.5))
    ///     .BuildScoped(".container");
    /// </code>
    /// </example>
    public static StyleBuilder Flex(this StyleBuilder builder,
        Flex.AxisDirection? direction = null,
        Flex.WrapMode? wrap = null,
        Flex.Justify? justifyContent = null,
        Flex.Align? alignItems = null,
        Flex.Align? alignContent = null,
        Spacing? gap = null,
        int? shrink = null,
        int? grow = null)
    {
        return builder
            .Set("display", "flex")
            .SetIfNotNull("flex-direction", direction?.ToCss())
            .SetIfNotNull("flex-wrap", wrap?.ToCss())
            .SetIfNotNull("justify-content", justifyContent?.ToCss())
            .SetIfNotNull("align-items", alignItems?.ToCss())
            .SetIfNotNull("align-content", alignContent?.ToCss())
            .SetIfNotNull("gap", gap)
            .SetIfNotNull("flex-shrink", shrink?.ToString())
            .SetIfNotNull("flex-grow", grow?.ToString());
    }
}
