using System;
using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Core;
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
    [Parameter] public SizeExpression? Top { get; set; }
    [Parameter] public SizeExpression? Right { get; set; }
    [Parameter] public SizeExpression? Bottom { get; set; }
    [Parameter] public SizeExpression? Left { get; set; }

    public override IStyleBuilder BuildStyle(IStyleBuilder builder)
    {
        if (!Condition) return builder;

        return builder.Positioned(Position, Top, Right, Bottom, Left);
    }
}

public static class PositionedExtensions
{
    public static IStyleBuilder Positioned(this IStyleBuilder builder,
        Positioned.PositionMode position = Components.Positioned.PositionMode.Absolute,
        SizeExpression? top = null,
        SizeExpression? right = null,
        SizeExpression? bottom = null,
        SizeExpression? left = null)
    {
        return builder
            .Append("position", position.ToCss())
            .AppendIfNotNull("top", top)
            .AppendIfNotNull("right", right)
            .AppendIfNotNull("bottom", bottom)
            .AppendIfNotNull("left", left);
    }
}
