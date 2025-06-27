using System;
using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Helper;

namespace RTB.Blazor.Styled.Components;

public class Positioned : RTBStyleBase
{
    public enum PositionMode {
        Absolute,
        Relative,
        Fixed,
        Sticky
    }

    [Parameter] public PositionMode Position { get; set; } = PositionMode.Absolute;
    [Parameter] public SizeUnit? Top { get; set; }
    [Parameter] public SizeUnit? Right { get; set; }
    [Parameter] public SizeUnit? Bottom { get; set; }
    [Parameter] public SizeUnit? Left { get; set; }

    protected override StyleBuilder BuildStyle(StyleBuilder builder)
    {
        return builder
            .Append("position", Position.ToCss())
            .AppendIfNotNull("top", Top)
            .AppendIfNotNull("right", Right)
            .AppendIfNotNull("bottom", Bottom)
            .AppendIfNotNull("left", Left);
    }
}
