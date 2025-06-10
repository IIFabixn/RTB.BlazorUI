using System;
using Microsoft.AspNetCore.Components;

namespace RTB.BlazorUI.Styles.Components;

public class Padding : RTBStyleBase
{
    [Parameter] public string? All { get; set; }
    [Parameter] public string? Top { get; set; }
    [Parameter] public string? Right { get; set; }
    [Parameter] public string? Bottom { get; set; }
    [Parameter] public string? Left { get; set; }
    [Parameter] public string? Horizontal { get; set; }
    [Parameter] public string? Vertical { get; set; }

    protected override StyleBuilder BuildStyle()
    {
        return StyleBuilder.Start.Append("padding", All)
            .AppendIf("padding", $"0 {Horizontal}", !string.IsNullOrEmpty(Horizontal))
            .AppendIf("padding", $"{Vertical} 0", !string.IsNullOrEmpty(Vertical))
            .Append("padding-top", Top)
            .Append("padding-right", Right)
            .Append("padding-bottom", Bottom)
            .Append("padding-left", Left);
    }
}
