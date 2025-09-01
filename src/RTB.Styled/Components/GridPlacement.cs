using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Core;

namespace RTB.Blazor.Styled.Components;

public class GridPlacement : RTBStyleBase
{
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;
    [Parameter] public int Column { get; set; } = 0;
    [Parameter] public int ColumnSpan { get; set; } = 1;
    [Parameter] public int Row { get; set; } = 0;
    [Parameter] public int RowSpan { get; set; } = 1;

    protected override void BuildStyle(StyleBuilder builder)
    {
        builder
            .Set("grid-column", $"{(Column > 0 ? Column : "auto")} / span {ColumnSpan}")
            .Set("grid-row", $"{(Row > 0 ? Row : "auto")} / span {RowSpan}");
    }
}

public static class GridPlacementExtensions
{
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
