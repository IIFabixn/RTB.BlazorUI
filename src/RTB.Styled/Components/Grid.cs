using System;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Core;
using RTB.Blazor.Styled.Helper;

namespace RTB.Blazor.Styled.Components;

/// <summary>
///  Contributes CSS for a grid container to the current <see cref="StyleBuilder"/> scope.
///  When placed inside a style scope, this component sets:
///  - display: grid
///  - grid-template-columns
///  - grid-template-rows
///  - gap (optional)
///  - place-items (optional)
/// </summary>
/// <remarks>
/// Usage (as a Blazor style-contributor component):
/// <code>
/// &lt;StyleScope&gt;
///   &lt;Grid TemplateColumns="repeat(3, 1fr)"
///         TemplateRows="auto 1fr"
///         Gap="@Spacing.Rem(1)"
///         ItemPlacement="Grid.Placement.Center" /&gt;
/// &lt;/StyleScope&gt;
/// </code>
/// Usage (via <see cref="GridExtensions.Grid(StyleBuilder, string, string, Spacing?, Grid.Placement?)"/> on a <see cref="StyleBuilder"/>):
/// <code>
/// var css = StyleBuilder.Start
///     .Grid("repeat(12, 1fr)", "auto", Spacing.Rem(1), Grid.Placement.Start)
///     .BuildScoped(".my-grid");
/// </code>
/// </remarks>
public class Grid : RTBStyleBase
{
    /// <summary>
    /// Alignment values mapped to the CSS <c>place-items</c> property.
    /// </summary>
    /// <remarks>
    /// Mappings:
    /// - <see cref="Normal"/> -> <c>normal</c>
    /// - <see cref="Start"/> -> <c>start</c>
    /// - <see cref="End"/> -> <c>end</c>
    /// - <see cref="Center"/> -> <c>center</c>
    /// - <see cref="Stretch"/> -> <c>stretch</c>
    /// - <see cref="FlexStart"/> -> <c>flex-start</c>
    /// - <see cref="FlexEnd"/> -> <c>flex-end</c>
    /// </remarks>
    public enum Placement
    {
        /// <summary>Emits <c>normal</c>.</summary>
        Normal,
        /// <summary>Emits <c>start</c>.</summary>
        Start,
        /// <summary>Emits <c>end</c>.</summary>
        End,
        /// <summary>Emits <c>center</c>.</summary>
        Center,
        /// <summary>Emits <c>stretch</c>.</summary>
        Stretch,
        /// <summary>Emits <c>flex-end</c>.</summary>
        FlexEnd,
        /// <summary>Emits <c>flex-start</c>.</summary>
        FlexStart
    }

    /// <summary>
    /// The value for CSS <c>grid-template-columns</c>.
    /// </summary>
    /// <remarks>
    /// Accepts any valid CSS value (e.g., <c>1fr</c>, <c>repeat(3, 1fr)</c>, <c>200px auto 1fr</c>).
    /// Defaults to <c>1fr</c>.
    /// </remarks>
    [Parameter] public string TemplateColumns { get; set; } = "1fr";

    /// <summary>
    /// The value for CSS <c>grid-template-rows</c>.
    /// </summary>
    /// <remarks>
    /// Accepts any valid CSS value (e.g., <c>auto</c>, <c>repeat(2, minmax(0, 1fr))</c>, <c>auto 1fr auto</c>).
    /// Defaults to <c>1fr</c>.
    /// </remarks>
    [Parameter] public string TemplateRows { get; set; } = "1fr";

    /// <summary>
    /// The CSS <c>gap</c> between grid rows and columns.
    /// </summary>
    /// <remarks>
    /// When provided, emits <c>gap: {value}</c>. When <c>null</c>, no <c>gap</c> is set.
    /// Use <see cref="Spacing"/> helpers (e.g., <see cref="Spacing.Rem(double)"/>, <see cref="Spacing.Px(double)"/>).
    /// </remarks>
    [Parameter] public Spacing? Gap { get; set; }

    /// <summary>
    /// The CSS <c>place-items</c> value to align items inside the grid container.
    /// </summary>
    /// <remarks>
    /// When provided, emits <c>place-items: {value}</c>. When <c>null</c>, no <c>place-items</c> is set.
    /// See <see cref="Placement"/> for available values and their CSS mapping.
    /// </remarks>
    [Parameter] public Placement? ItemPlacement { get; set; }

    /// <summary>
    /// Contributes CSS declarations to the provided <paramref name="builder"/>.
    /// </summary>
    /// <inheritdoc cref="RTBStyleBase.BuildStyle(StyleBuilder)"/>
    protected override void BuildStyle(StyleBuilder builder)
    {
        builder
            .Set("display", "grid")
            .Set("grid-template-columns", TemplateColumns)
            .Set("grid-template-rows", TemplateRows)
            .SetIfNotNull("gap", Gap)
            .SetIfNotNull("place-items", ItemPlacement?.ToCss());
    }
}

/// <summary>
/// Extension methods for configuring grid-related CSS on a <see cref="StyleBuilder"/>.
/// </summary>
/// <remarks>
/// Use when composing styles fluently without the component.
/// </remarks>
public static class GridExtensions
{
    /// <summary>
    /// Configures the current <see cref="StyleBuilder"/> as a CSS Grid container.
    /// </summary>
    /// <param name="builder">The style builder to mutate.</param>
    /// <param name="templateColumns">CSS <c>grid-template-columns</c> value. Defaults to <c>1fr</c>.</param>
    /// <param name="templateRows">CSS <c>grid-template-rows</c> value. Defaults to <c>1fr</c>.</param>
    /// <param name="gap">Optional CSS <c>gap</c> value. When <c>null</c>, no gap is emitted.</param>
    /// <param name="itemPlacement">Optional CSS <c>place-items</c> value via <see cref="Grid.Placement"/>.</param>
    /// <returns>The same <see cref="StyleBuilder"/> instance for chaining.</returns>
    /// <example>
    /// <code>
    /// StyleBuilder.Start
    ///     .Grid("repeat(4, 1fr)", "auto 1fr auto", Spacing.Rem(1), Grid.Placement.Center)
    ///     .BuildScoped(".grid");
    /// </code>
    /// </example>
    public static StyleBuilder Grid(this StyleBuilder builder,
        string templateColumns = "1fr",
        string templateRows = "1fr",
        Spacing? gap = null,
        Grid.Placement? itemPlacement = null)
    {
        return builder
            .Set("display", "grid")
            .Set("grid-template-columns", templateColumns)
            .Set("grid-template-rows", templateRows)
            .SetIfNotNull("gap", gap)
            .SetIfNotNull("place-items", itemPlacement?.ToCss());
    }
}
