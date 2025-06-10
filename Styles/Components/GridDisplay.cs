using System;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Components;

namespace RTB.BlazorUI.Styles.Components;

public class GridDisplay : RTBStyleBase
{
    [Parameter] public string TemplateColumns { get; set; } = "1fr";
    [Parameter] public string TemplateRows { get; set; } = "1fr";
    [Parameter] public string? Gap { get; set; }
    [Parameter] public string? ItemPlacement { get; set; }

    protected override StyleBuilder BuildStyle(StyleBuilder builder)
    {
        return builder
            .Append("display", "grid")
            .Append("grid-template-columns", TemplateColumns)
            .Append("grid-template-rows", TemplateRows)
            .AppendIfNotNull("gap", Gap)
            .AppendIfNotNull("place-items", ItemPlacement);
    }
}
