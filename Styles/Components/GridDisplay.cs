using System;
using Microsoft.AspNetCore.Components;

namespace RTB.BlazorUI.Styles.Components;

public class GridDisplay : RTBStyleBase
{
    [Parameter] public string? TemplateColumns { get; set; }
    [Parameter] public string? TemplateRows { get; set; }
    [Parameter] public string? Gap { get; set; }

    protected override StyleBuilder BuildStyle()
    {
        return StyleBuilder.Start.Append("display", "grid")
            .Append("grid-template-columns", TemplateColumns)
            .Append("grid-template-rows", TemplateRows)
            .Append("gap", Gap);
    }
}
