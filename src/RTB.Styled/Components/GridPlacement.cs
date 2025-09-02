using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Core;

namespace RTB.Blazor.Styled.Components;

/// <summary>
/// Contributes CSS Grid placement declarations to a <see cref="StyleBuilder"/>.
/// </summary>
/// <remarks>
/// - Emits <c>grid-column</c> and <c>grid-row</c> declarations using explicit start positions (when &gt; 0) or <c>auto</c>,
///   combined with their respective spans.<br/>
/// - This component renders no visual markup; it participates in style composition only.
/// - When <see cref="Column"/> or <see cref="Row"/> is less than or equal to 0, <c>auto</c> is used for the start.
/// - No validation is performed on spans; callers should pass values greater than or equal to 1.
/// </remarks>
/// <example>
/// Usage in a style scope:
/// <code>
/// <GridPlacement Column="2" ColumnSpan="3" Row="1" RowSpan="2" />
/// </code>
/// Produces:
/// <code>
/// grid-column: 2 / span 3;
/// grid-row: 1 / span 2;
/// </code>
/// </example>
public class GridPlacement : RTBStyleBase
{
    /// <summary>
    /// Optional child content. Captured but not rendered by this component.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;

    /// <summary>
    /// The 1-based starting grid column track. When less than or equal to 0, the start resolves to <c>auto</c>.
    /// </summary>
    [Parameter] public int Column { get; set; } = 0;

    /// <summary>
    /// The number of columns to span. Should be greater than or equal to 1. Defaults to 1.
    /// </summary>
    [Parameter] public int ColumnSpan { get; set; } = 1;

    /// <summary>
    /// The 1-based starting grid row track. When less than or equal to 0, the start resolves to <c>auto</c>.
    /// </summary>
    [Parameter] public int Row { get; set; } = 0;

    /// <summary>
    /// The number of rows to span. Should be greater than or equal to 1. Defaults to 1.
    /// </summary>
    [Parameter] public int RowSpan { get; set; } = 1;

    /// <summary>
    /// <inheritdoc cref="RTBStyleBase.BuildStyle(StyleBuilder)"/>
    /// </summary>
    /// <param name="builder">The style builder receiving the grid placement declarations.</param>
    /// <remarks>
    /// Writes:
    /// <list type="bullet">
    ///   <item><description><c>grid-column: [Column|auto] / span [ColumnSpan]</c></description></item>
    ///   <item><description><c>grid-row: [Row|auto] / span [RowSpan]</c></description></item>
    /// </list>
    /// </remarks>
    protected override void BuildStyle(StyleBuilder builder)
    {
        builder
            .Set("grid-column", $"{(Column > 0 ? Column : "auto")} / span {ColumnSpan}")
            .Set("grid-row", $"{(Row > 0 ? Row : "auto")} / span {RowSpan}");
    }
}

/// <summary>
/// Extension methods for composing CSS Grid placement with a <see cref="StyleBuilder"/>.
/// </summary>
public static class GridPlacementExtensions
{
    /// <summary>
    /// Adds <c>grid-column</c> and <c>grid-row</c> declarations to the builder using the provided placement options.
    /// </summary>
    /// <param name="builder">The target style builder.</param>
    /// <param name="column">1-based starting column; when less than or equal to 0, uses <c>auto</c>.</param>
    /// <param name="columnSpan">Number of columns to span; should be greater than or equal to 1.</param>
    /// <param name="row">1-based starting row; when less than or equal to 0, uses <c>auto</c>.</param>
    /// <param name="rowSpan">Number of rows to span; should be greater than or equal to 1.</param>
    /// <returns>The same <see cref="StyleBuilder"/> instance for fluent chaining.</returns>
    /// <example>
    /// <code>
    /// builder.GridPlacement(column: 2, columnSpan: 2, row: 1, rowSpan: 3);
    /// // grid-column: 2 / span 2; grid-row: 1 / span 3;
    /// </code>
    /// </example>
    public static StyleBuilder GridPlacement(this StyleBuilder builder,
        int column = 0,
        int columnSpan = 1,
        int row = 0,
        int rowSpan = 1)
    {
        return builder
            .Set("grid-column", $"{(column > 0 ? column : "auto")} / span {columnSpan}")
            .Set("grid-row", $"{(row > 0 ? row : "auto")} / span {rowSpan}");
    }
}
