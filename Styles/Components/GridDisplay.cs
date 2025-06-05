using System;
using Microsoft.AspNetCore.Components;

namespace RTB.BlazorUI.Styles.Components;

public class GridDisplay : RTBStyleBase
{
    [Parameter] public string? TemplateColumns { get; set; }
    [Parameter] public string? TemplateRows { get; set; }
    [Parameter] public string? Gap { get; set; }
    
    protected override void OnParametersSet()
    {
        if (!Condition) return;
        
        StyleBuilder.Append("display", "grid");
        StyleBuilder.Append("grid-template-columns", TemplateColumns);
        StyleBuilder.Append("grid-template-rows", TemplateRows);
        StyleBuilder.Append("gap", Gap);
        
        base.OnParametersSet();
    }
}
