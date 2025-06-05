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

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (!Condition) return;

        StyleBuilder.Append("margin", All);
        StyleBuilder.AppendIf("margin", $"0 {Horizontal}", !string.IsNullOrEmpty(Horizontal));
        StyleBuilder.AppendIf("margin", $"{Vertical} 0", !string.IsNullOrEmpty(Vertical));
        StyleBuilder.Append("margin-top", Top);
        StyleBuilder.Append("margin-right", Right);
        StyleBuilder.Append("margin-bottom", Bottom);
        StyleBuilder.Append("margin-left", Left);
    }
}
