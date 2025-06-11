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

    protected override StyleBuilder BuildStyle(StyleBuilder builder)
    {
        return builder
            .AppendIfNotNull("width", Width)
            .AppendIf("width", "100%", FullWidth)
            .AppendIfNotNull("height", Height)
            .AppendIf("height", "100%", FullHeight)
            .AppendIfNotNull("min-width", MinWidth)
            .AppendIfNotNull("min-height", MinHeight)
            .AppendIfNotNull("max-width", MaxWidth)
            .AppendIfNotNull("max-height", MaxHeight);
    }
}
