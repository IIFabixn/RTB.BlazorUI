using Microsoft.AspNetCore.Components;

namespace RTB.Blazor.Styled.Components;

public class GridPlacement : RTBStyleBase
{
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;
    [Parameter] public int Column { get; set; } = 0;
    [Parameter] public int ColumnSpan { get; set; } = 1;
    [Parameter] public int Row { get; set; } = 0;
    [Parameter] public int RowSpan { get; set; } = 1;

    protected override StyleBuilder BuildStyle(StyleBuilder builder)
    {
        return builder
            .Append("grid-column", $"{(Column > 0 ? Column : "auto")} / span {ColumnSpan}")
            .Append("grid-row", $"{(Row > 0 ? Row : "auto")} / span {RowSpan}");
    }
}
