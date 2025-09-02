using System;
using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Core;
using RTB.Blazor.Styled.Helper;

namespace RTB.Blazor.Styled.Components;

/// <summary>
/// A Blazor style contributor that emits CSS positioning declarations.
/// </summary>
/// <remarks>
/// <para>
/// This component contributes the following declarations into the current <see cref="StyleBuilder"/> scope:
/// - Always emits <c>position: {mode}</c>.
/// - Emits <c>top</c>, <c>right</c>, <c>bottom</c>, <c>left</c> only when their values are not null.
/// </para>
/// <para>
/// Place this inside a style scope that provides a cascading <see cref="StyleBuilder"/> (via <see cref="RTBStyleBase.StyleBuilder"/>).
/// </para>
/// <example>
/// Razor usage:
/// <code>
/// <StyledRoot>
///   <Positioned Position="Positioned.PositionMode.Relative"
///               Top="@Size.Px(8)"
///               Left="@Size.Rem(1)" />
/// </StyledRoot>
/// </code>
/// </example>
/// <example>
/// Fluent usage:
/// <code>
/// var css = StyleBuilder.Start
///     .Positioned(Positioned.PositionMode.Fixed, top: Size.Percent(10), right: Size.Px(16))
///     .BuildScoped("my-class");
/// </code>
/// </example>
/// </remarks>
public class Positioned : RTBStyleBase
{
    /// <summary>
    /// The CSS positioning mode to emit.
    /// </summary>
    public enum PositionMode
    {
        /// <summary>Emits <c>position: absolute</c>.</summary>
        Absolute,
        /// <summary>Emits <c>position: relative</c>.</summary>
        Relative,
        /// <summary>Emits <c>position: fixed</c>.</summary>
        Fixed,
        /// <summary>Emits <c>position: sticky</c>.</summary>
        Sticky
    }

    /// <summary>
    /// CSS <c>position</c> mode. Defaults to <see cref="PositionMode.Absolute"/>.
    /// </summary>
    /// <remarks>
    /// The chosen mode is always emitted; offsets are included only when provided.
    /// </remarks>
    [Parameter] public PositionMode Position { get; set; } = PositionMode.Absolute;

    /// <summary>
    /// CSS <c>top</c> offset. Emitted only when not null.
    /// </summary>
    /// <remarks>Use <see cref="SizeExpression"/> helpers to create values (e.g., pixels, rem, percentages).</remarks>
    [Parameter] public SizeExpression? Top { get; set; }

    /// <summary>
    /// CSS <c>right</c> offset. Emitted only when not null.
    /// </summary>
    /// <remarks>Use <see cref="SizeExpression"/> helpers to create values (e.g., pixels, rem, percentages).</remarks>
    [Parameter] public SizeExpression? Right { get; set; }

    /// <summary>
    /// CSS <c>bottom</c> offset. Emitted only when not null.
    /// </summary>
    /// <remarks>Use <see cref="SizeExpression"/> helpers to create values (e.g., pixels, rem, percentages).</remarks>
    [Parameter] public SizeExpression? Bottom { get; set; }

    /// <summary>
    /// CSS <c>left</c> offset. Emitted only when not null.
    /// </summary>
    /// <remarks>Use <see cref="SizeExpression"/> helpers to create values (e.g., pixels, rem, percentages).</remarks>
    [Parameter] public SizeExpression? Left { get; set; }

    /// <summary>
    /// Builds the component's style contribution by setting the positioning and optional offsets.
    /// <inheritdoc cref="RTBStyleBase.BuildStyle(StyleBuilder)"/>
    /// </summary>
    /// <param name="builder">The style builder receiving declarations.</param>
    protected override void BuildStyle(StyleBuilder builder)
    {
        builder.Positioned(Position, Top, Right, Bottom, Left);
    }
}

/// <summary>
/// Extension methods for <see cref="StyleBuilder"/> related to CSS positioning.
/// </summary>
/// <remarks>
/// Designed for fluent usage. Null offsets are ignored. The <c>position</c> declaration is always emitted.
/// </remarks>
/// <example>
/// <code>
/// var css = StyleBuilder.Start
///     .Positioned(Positioned.PositionMode.Relative, top: Size.Px(8))
///     .BuildScoped("my-class");
/// </code>
/// </example>
public static class PositionedExtensions
{
    /// <summary>
    /// Adds CSS <c>position</c> and optional offset declarations to the base declaration set.
    /// </summary>
    /// <param name="builder">The style builder to mutate.</param>
    /// <param name="position">The <see cref="Positioned.PositionMode"/> to emit; defaults to <c>absolute</c>.</param>
    /// <param name="top">Optional CSS <c>top</c> offset.</param>
    /// <param name="right">Optional CSS <c>right</c> offset.</param>
    /// <param name="bottom">Optional CSS <c>bottom</c> offset.</param>
    /// <param name="left">Optional CSS <c>left</c> offset.</param>
    /// <returns>The same <paramref name="builder"/> to allow fluent chaining.</returns>
    /// <example>
    /// <code>
    /// StyleBuilder.Start
    ///     .Positioned(Positioned.PositionMode.Sticky, top: Size.Rem(2))
    ///     .Selector("&amp; &gt; .badge", b =&gt; b.Set("z-index", "10"));
    /// </code>
    /// </example>
    public static StyleBuilder Positioned(this StyleBuilder builder,
        Positioned.PositionMode position = Components.Positioned.PositionMode.Absolute,
        SizeExpression? top = null,
        SizeExpression? right = null,
        SizeExpression? bottom = null,
        SizeExpression? left = null)
    {
        return builder
            .Set("position", position.ToCss())
            .SetIfNotNull("top", top)
            .SetIfNotNull("right", right)
            .SetIfNotNull("bottom", bottom)
            .SetIfNotNull("left", left);
    }
}
