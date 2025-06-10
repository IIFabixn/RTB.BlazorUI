using System;
using Microsoft.AspNetCore.Components;

namespace RTB.BlazorUI.Styles.Components;

public class Positioned : RTBStyleBase
{
    [Parameter] public string? Top { get; set; } = null;
    [Parameter] public string? Right { get; set; } = null;
    [Parameter] public string? Bottom { get; set; } = null;
    [Parameter] public string? Left { get; set; } = null;

    protected override StyleBuilder BuildStyle(StyleBuilder builder)
    {
        return builder.Append("top", Top)
            .Append("right", Right)
            .Append("bottom", Bottom)
            .Append("left", Left);
    }
}
