using System;
using Microsoft.AspNetCore.Components;

namespace RTB.BlazorUI.Styles.Components;

public class Size : RTBStyleBase
{
    [Parameter] public string? Width { get; set; }
    [Parameter] public string? Height { get; set; }
    [Parameter] public string? MinWidth { get; set; }
    [Parameter] public string? MinHeight { get; set; }
    [Parameter] public string? MaxWidth { get; set; }
    [Parameter] public string? MaxHeight { get; set; }

    [Parameter] public bool FullWidth { get; set; } = false;
    [Parameter] public bool FullHeight { get; set; } = false;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (!Condition) return;

        StyleBuilder
            .Append("width", Width)
            .AppendIf("width", "100%", FullWidth)
            .Append("height", Height)
            .AppendIf("height", "100%", FullHeight)
            .Append("min-width", MinWidth)
            .Append("min-height", MinHeight)
            .Append("max-width", MaxWidth)
            .Append("max-height", MaxHeight);
    }
}
