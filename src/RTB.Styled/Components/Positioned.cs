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
        return builder.Positioned(Position, Top, Right, Bottom, Left);
    }
}

public static class PositionedExtensions
{
    public static StyleBuilder Positioned(this StyleBuilder builder,
        Positioned.PositionMode position = Components.Positioned.PositionMode.Absolute,
        SizeUnit? top = null,
        SizeUnit? right = null,
        SizeUnit? bottom = null,
        SizeUnit? left = null)
    {
        return builder
            .Append("position", position.ToCss())
            .AppendIfNotNull("top", top)
            .AppendIfNotNull("right", right)
            .AppendIfNotNull("bottom", bottom)
            .AppendIfNotNull("left", left);
    }
}
