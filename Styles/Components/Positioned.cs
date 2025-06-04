using System;
using Microsoft.AspNetCore.Components;

namespace RTB.BlazorUI.Styles.Components;

public class Positioned : RTBStyleBase
{
    [Parameter] public string? Top { get; set; } = null;
    [Parameter] public string? Right { get; set; } = null;
    [Parameter] public string? Bottom { get; set; } = null;
    [Parameter] public string? Left { get; set; } = null;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (!Condition) return;

        StyleBuilder.AppendIfNotEmpty("top", Top)
            .AppendIfNotEmpty("right", Right)
            .AppendIfNotEmpty("bottom", Bottom)
            .AppendIfNotEmpty("left", Left);
    }
}
