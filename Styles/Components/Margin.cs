using System;
using Microsoft.AspNetCore.Components;

namespace RTB.BlazorUI.Styles.Components;

public class Margin : RTBStyleBase
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
        return StyleBuilder.Start.Append("margin", All)
            .AppendIf("margin", $"0 {Horizontal}", !string.IsNullOrEmpty(Horizontal))
            .AppendIf("margin", $"{Vertical} 0", !string.IsNullOrEmpty(Vertical))
            .Append("margin-top", Top)
            .Append("margin-right", Right)
            .Append("margin-bottom", Bottom)
            .Append("margin-left", Left);
    }
}
