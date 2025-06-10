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

    protected override StyleBuilder BuildStyle(StyleBuilder builder)
    {
        return builder
            .AppendIfNotNull("padding", All)
            .AppendIf("padding", $"{Vertical ?? "0"} {Horizontal ?? "0"}", !string.IsNullOrWhiteSpace(Horizontal) || !string.IsNullOrWhiteSpace(Vertical))
            .AppendIfNotNull("padding-top", Top)
            .AppendIfNotNull("padding-right", Right)
            .AppendIfNotNull("padding-bottom", Bottom)
            .AppendIfNotNull("padding-left", Left);
    }
}
