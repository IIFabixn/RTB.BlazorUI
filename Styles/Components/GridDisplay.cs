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
        base.OnParametersSet();
        if (Condition is not null && !Condition.Invoke()) return;
        
        StyleBuilder.Append("display", "grid");
        StyleBuilder.AppendIfNotEmpty("grid-template-columns", TemplateColumns);
        StyleBuilder.AppendIfNotEmpty("grid-template-rows", TemplateRows);
        StyleBuilder.AppendIfNotEmpty("gap", Gap);
    }
}
