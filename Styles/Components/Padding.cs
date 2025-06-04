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

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (Condition is not null && !Condition.Invoke()) return;

        StyleBuilder.AppendIfNotEmpty("padding", All);
        StyleBuilder.AppendIf("padding", $"0 {Horizontal}", !string.IsNullOrEmpty(Horizontal));
        StyleBuilder.AppendIf("padding", $"{Vertical} 0", !string.IsNullOrEmpty(Vertical));
        StyleBuilder.AppendIfNotEmpty("padding-top", Top);
        StyleBuilder.AppendIfNotEmpty("padding-right", Right);
        StyleBuilder.AppendIfNotEmpty("padding-bottom", Bottom);
        StyleBuilder.AppendIfNotEmpty("padding-left", Left);
    }
}
