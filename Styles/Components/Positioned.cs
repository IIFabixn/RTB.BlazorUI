using System;
using Microsoft.AspNetCore.Components;
using RTB.BlazorUI.Styles.Helper;

namespace RTB.BlazorUI.Styles.Components;

public class Positioned : RTBStyleBase
{
    [Parameter] public string Position { get; set; } = "absolute";
    [Parameter] public SizeUnit? Top { get; set; } = null;
    [Parameter] public SizeUnit? Right { get; set; } = null;
    [Parameter] public SizeUnit? Bottom { get; set; } = null;
    [Parameter] public SizeUnit? Left { get; set; } = null;

    protected override StyleBuilder BuildStyle(StyleBuilder builder)
    {
        return builder
            .Append("position", Position)
            .Append("top", Top)
            .Append("right", Right)
            .Append("bottom", Bottom)
            .Append("left", Left);
    }
}
