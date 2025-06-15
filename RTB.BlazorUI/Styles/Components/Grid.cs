using System;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Components;
using RTB.BlazorUI.Styles.Helper;

namespace RTB.BlazorUI.Styles.Components;

public class Grid : RTBStyleBase
{

    [Parameter] public string TemplateColumns { get; set; } = "1fr";
    [Parameter] public string TemplateRows { get; set; } = "1fr";
    [Parameter] public Spacing? Gap { get; set; }
    [Parameter] public Place? ItemPlacement { get; set; }

    protected override StyleBuilder BuildStyle(StyleBuilder builder)
    {
        return builder
            .Append("display", "grid")
            .Append("grid-template-columns", TemplateColumns)
            .Append("grid-template-rows", TemplateRows)
            .AppendIfNotNull("gap", Gap)
            .AppendIfNotNull("place-items", ItemPlacement?.ToCss());
    }
}
